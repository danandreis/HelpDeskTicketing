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
    public class TicketsController : Controller
    {

        private readonly ITicketService _ticketService;
        private readonly IPrincipal _principal;
        private readonly INotyfService _notyfService;
        private readonly ITicketMessageService _ticketMessageService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ITicketFilesService _ticketFilesService;

        public TicketsController(ITicketService ticketService, IPrincipal principal, INotyfService notyfService,
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


        //Return all tickets for a user
        [Authorize]
        public async Task<IActionResult> GetUserTickets() 
        {

            var userTicketsList = await _ticketService.GetTicketsList(_principal.Identity.Name, "All", "All");
            ViewBag.TicketsType = "All"; //Displays all tickets of the logged in user

            return View("GetAllTickets",userTicketsList);

        }


        //Return the tickets for a SystemAdmin
        [Authorize(Roles ="SystemAdmin")]
        public async Task<IActionResult> GetAdminTickets(string type)
        {
            //type - type of tickets : Open/Assigned/Closed

            if (type == null || (!type.Equals("Open") && !type.Equals("Assigned") && !type.Equals("Closed")))
            {

                return RedirectToAction("GetAdminTickets", "Tickets", new { type = "Open" });

            }

            var adminTicketsList = await _ticketService.GetTicketsList(null, type, "All");

            if (type.Equals("Assigned"))
            {

                adminTicketsList = await _ticketService.GetTicketsList(_principal.Identity.Name, type, "All");

            }

            ViewBag.TicketsType = type; 
            ViewBag.TicketsNoByStatus = await _ticketService.GetTicketsNumberGroupedByStatus();
            return View("GetAllTickets", adminTicketsList);

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

            if(User.IsInRole("SystemAdmin"))
            {

                ViewBag.TicketsNoByStatus = await _ticketService.GetTicketsNumberGroupedByStatus();
                ViewBag.AdminUsers = new SelectList(await _userService.GetUserAdmins(),"Id","FullName");

            }

            return View(ticketMV);

        }

        [Authorize]
        public async Task<IActionResult> AssignTicket(string id)
        {

            TicketUserVM ticketUserVM = new TicketUserVM
            {

                TicketId = id,
                UserRole = "SystemAdmin"
            };

            //Asign user to ticket
            if(await _ticketService.AddTicketUser(ticketUserVM) != null)
            {

                //Change status of the ticket to Assigned

                if(await _ticketService.UpdateTicketStatus(id,"Assigned"))
                {

                    return RedirectToAction("GetAdminTickets", "Tickets", new { type = "Assigned" });

                }

            }

            return RedirectToAction("GetAdminTickets", "Tickets", new { type = "Open" });


        }

        [HttpPost]
        public async Task<IActionResult> ReAssignTicket(TicketUserVM ticketUserVM)
        {

            //Asigned a ticket to another SystemAdmin user to resolve it
            await _ticketService.AddTicketUser(ticketUserVM);
            return RedirectToAction("GetAdminTickets", "Tickets", new { type = "Assigned" });

        }

        [Authorize]
        public async Task<IActionResult> AddTicket()
        {

            ViewBag.Domains = new SelectList(await _ticketService.GetDomainsList(), "Id", "Name");
            ViewBag.Priorities = new SelectList(await _ticketService.GetTicketPriorities(), "Id", "Name");

            var ticketVM = new TicketVM
            {
                Branch = await _ticketService.GetUserBranch()
            };

            return View(ticketVM);

        }

        [HttpPost]
        public async Task<IActionResult> AddTicket(TicketVM ticketVM)
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
                    TicketId = ticketVM.Id,
                    UserRole = "User"
                    
                };


                //Add the user to TicketUser table in database (Associate the current user to the tocket
                //as Use who issued thet ticket

                if (await _ticketService.AddTicketUser(ticketUserVM) != null)
                {

                    //Add Message to the TicketMesage Table
                    TicketMessage ticketMessage = new TicketMessage
                    {
                        Id = Guid.NewGuid().ToString(),
                        Message = ticketVM.TicketMessage.Message,
                        TicketId = ticketVM.Id,
                        Date = DateTime.Now,
                        AppUserId = _userService.GetUserByUserName(_principal.Identity.Name).Result.Id
                    };

                    if (await _ticketMessageService.AddTicketMessage(ticketMessage) == null)
                    {

                        _notyfService.Error("An error occured where registering the new ticket to database!");


                    }
                    else
                    {

                        //Add/Save Files to DB/Disk if there are some 
                        if (ticketVM.TicketMessage.Files != null && ticketVM.TicketMessage.Files.Count > 0)
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

            if (!ModelState.IsValid)
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
                    if (ticketMessageVM.Files != null && ticketMessageVM.Files.Count > 0)
                    {

                        if (!await _ticketFilesService.AddFileToDatabase(ticketMessageVM.Files, ticketMessage.Id))
                            _notyfService.Error("There was an error when saving files to disk!");

                    }

                }

            }

            return RedirectToAction("GetTicket", new { Id = ticketMessageVM.TicketId });

        }


       
    }

}