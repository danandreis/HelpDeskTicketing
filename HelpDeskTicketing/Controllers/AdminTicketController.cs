using AspNetCoreHero.ToastNotification.Abstractions;
using AutoMapper;
using HelpDeskTicketing.Data.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;

namespace HelpDeskTicketing.Controllers
{
    public class AdminTicketController : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly ITicketMessageService _ticketMessageService;
        private readonly IPrincipal _principal;
        private readonly INotyfService _notyfService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public AdminTicketController(ITicketService ticketService, ITicketMessageService ticketMessageService, IPrincipal principal,
            INotyfService notyfService, IUserService userService, IMapper mapper)
        {
            _ticketService = ticketService;
            _ticketMessageService = ticketMessageService;
            _principal = principal;
            _notyfService = notyfService;
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<IActionResult> GetOpenTickets()
        {

            var ticketList = await _ticketService.GetTicketsList(null, "Open", "All");
            ViewBag.TicketsType = "Open";

            return View(ticketList);
        }


    }
}
