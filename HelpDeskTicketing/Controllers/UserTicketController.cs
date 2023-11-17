using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Notyf;
using HelpDeskTicketing.Data.Services;
using HelpDeskTicketing.Data.ViewModels;
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

        public UserTicketController(ITicketService ticketService, IPrincipal principal, INotyfService notyfService)
        {

            _ticketService = ticketService;
            _principal = principal;
            _notyfService = notyfService;
        }

        [Authorize]
        public async Task<IActionResult> GetAllTickets()
        {

            var userTicketsList = await _ticketService.GetTicketsList(User.Identity.Name);

            return View(userTicketsList);
        }

        [Authorize]
        public async Task<IActionResult>GetTicket(string id)
        {

            var ticketDB = await _ticketService.GetTicket(id);

            return View(ticketDB);

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

                //Add the user to TicketUser table in database (Associate the current user to the tocket
                //as Use who issued thet ticket

                TicketUserVM ticketUserVM = new TicketUserVM();
                ticketUserVM.TicketId = ticketVM.Id;

                if(await _ticketService.AddTicketUser(ticketUserVM) != null)
                {

                    _notyfService.Success("The ticket has been successfully registered to database!");

                    return RedirectToAction("GetAllTickets");

                }

            }

            _notyfService.Error("An error occured where registering the new ticket to database!");
            return View(ticketVM);

        }
    }
}
