using AspNetCoreHero.ToastNotification.Abstractions;
using AutoMapper;
using HelpDeskTicketing.Data.Services;
using HelpDeskTicketing.Data.ViewModels;
using HelpDeskTicketing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Principal;

namespace HelpDeskTicketing.Controllers
{
    public class UserTicketController : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly IPrincipal _principal;
        private readonly INotyfService _notyfService;
        private readonly ITicketMessageService _ticketMessageService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ITicketFilesService _ticketFilesService;

        public UserTicketController(ITicketService ticketService, IPrincipal principal, INotyfService notyfService, 
                ITicketMessageService ticketMessageService, IUserService userService, IMapper mapper,
                ITicketFilesService ticketFilesService)
        {

            _ticketService = ticketService;
            _principal = principal;
            _notyfService = notyfService;
            _ticketMessageService = ticketMessageService;
            _userService = userService;
            _mapper = mapper;
            _ticketFilesService = ticketFilesService;
        }

        [Authorize]
        public async Task<IActionResult> GetAllTickets()
        {

            var userTicketsList = await _ticketService.GetTicketsList(User.Identity.Name);

            return View(userTicketsList);
        }

        [Authorize]
        public async Task<IActionResult> GetTicket(string id)
        {

            var ticketDB = await _ticketService.GetTicket(id);

            TicketVM ticketMV = new TicketVM();
            _mapper.Map(ticketDB, ticketMV);

            ticketMV.TicketMessage = new TicketMessageVM
            {

                TicketId = ticketDB.Id

            };

            return View(ticketMV);

        }

        [Authorize]
        public async Task<IActionResult>AddTicket()
        {

            ViewBag.Domains = new SelectList(await _ticketService.GetDomainsList(), "Id", "Name");
            ViewBag.Priorities = new SelectList(await _ticketService.GetTicketPriorities(),"Id", "Name");

            var ticketVM = new TicketVM();

            ticketVM.Branch = await _ticketService.GetUserBranch();

            return View(ticketVM);

        }

        [HttpPost]
        public async Task<IActionResult>AddTicket(TicketVM ticketVM)
        {

            //ModelState["Branch"].ValidationState = ModelValidationState.Valid;
            ModelState["StatusId"].ValidationState = ModelValidationState.Valid;


            if (!ModelState.IsValid)
            {

                ticketVM.Branch = await _ticketService.GetUserBranch();
                ViewBag.Domains = new SelectList(await _ticketService.GetDomainsList(), "Id", "Name");
                ViewBag.Priorities = new SelectList(await _ticketService.GetTicketPriorities(), "Id", "Name");
                
                return View(ticketVM);

            }

            ticketVM.Branch = await _ticketService.GetUserBranch();
            ticketVM.StatusId = _ticketService.GetStatusByName("Open").Result.Id;
            ticketVM.StartDate = DateTime.Now;
            ticketVM.EndDate = null;
            ticketVM.Id = Guid.NewGuid().ToString();


            //Add new Ticket to Database
            if (await _ticketService.AddTicket(ticketVM) != null)
            {

                TicketUserVM ticketUserVM = new TicketUserVM
                {
                    TicketId = ticketVM.Id
                };


                //Add the user to TicketUser table in database (Associate the current user to the tocket
                //as Use who issued thet ticket

                if (await _ticketService.AddTicketUser(ticketUserVM) != null)
                {

                    //Add Message to the TicketMesage Table
                    TicketMessage ticketMessage = new TicketMessage();
                    ticketMessage.Id = Guid.NewGuid().ToString();
                    ticketMessage.Message = ticketVM.TicketMessage.Message;
                    ticketMessage.TicketId = ticketVM.Id;
                    ticketMessage.Date = DateTime.Now;
                    ticketMessage.AppUserId = _userService.GetUserByUserName(_principal.Identity.Name).Result.Id;

                    if(await _ticketMessageService.AddTicketMessage(ticketMessage) == null)
                    {

                        _notyfService.Error("An error occured where registering the new ticket to database!");


                    }
                    else
                    {

                        //Add/Save Files to DB/Disk if there are some 
                        if (ticketVM.TicketMessage.Files.Count > 0)
                        {

                            if (!await _ticketFilesService.AddFileToDatabase(ticketVM.TicketMessage.Files, ticketMessage.Id))
                            {

                                _notyfService.Error("There was an error when saving files to disk!");

                            }
                            else
                            {

                                _notyfService.Success("The ticket has been successfully registered to database!");

                                return RedirectToAction("GetAllTickets");


                            }
                                

                        }
                        else
                        {

                            return RedirectToAction("GetAllTickets");

                        }

                    }

                }

            }

            return View(ticketVM);

        }

        [HttpPost]
        public async Task<IActionResult> AddTicketMessage(TicketMessageVM ticketMessageVM)
        {

            if(!ModelState.IsValid)
            {

                _notyfService.Error("You must provide a message!");
                

            }
            else
            {

                TicketMessage ticketMessage = new TicketMessage();
                _mapper.Map(ticketMessageVM, ticketMessage);

                ticketMessage.Id = Guid.NewGuid().ToString();
                ticketMessage.AppUserId = _ticketService.GetUserByUserName(_principal.Identity?.Name).Result.Id;
                ticketMessage.Date = DateTime.Now;


                if (await _ticketMessageService.AddTicketMessage(ticketMessage) == null)
                {

                    _notyfService.Error("There was an error when saving message to database!");



                }
                else
                {

                    //Add/Save Files to DB/Disk if there are some 
                    if(ticketMessageVM.Files.Count > 0)
                    {

                        if(!await _ticketFilesService.AddFileToDatabase(ticketMessageVM.Files,ticketMessage.Id))
                            _notyfService.Error("There was an error when saving files to disk!");

                    }

                }

            }

            return RedirectToAction("GetTicket", new { Id = ticketMessageVM.TicketId });

        }
    }
}
