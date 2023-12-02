using HelpDeskTicketing.Data.ViewModels;
using HelpDeskTicketing.Models;

namespace HelpDeskTicketing.Data.Services
{
    public interface ITicketService
    {

        Task<IEnumerable<Ticket>> GetTicketsList(string userName, string status, string priority);
        Task<TicketEvidenceVM> GetTicketsNumberGroupedByStatus();
        Task<Ticket> GetTicket(string Id);
        Task<Ticket> AddTicket(TicketVM ticketVM);
        Task<TicketUser> AddTicketUser(TicketUserVM ticketUserVM);
        Task<AssignNewUserTicketVM> UpdateTicketUser(AssignNewUserTicketVM assignNewUserTicket);
        Task<TicketUser> GetAssignedUser(TicketUserVM ticketUserVM); //Returns the assigned user if there is one
        Task<Ticket> UpdateTicket(string Id);
        Task<bool> UpdateTicketStatus(string ticketId, string status);
        Task<IEnumerable<Domain>> GetDomainsList();
        Task<Domain> GetDomainByName(string domainName);
        Task<Status> GetStatusByName(string statusName);
        Task<Branch> GetUserBranch();
        Task<Branch> GetBranchByName(string branchName);
        Task<IEnumerable<Priority>> GetTicketPriorities();
        Task<AppUser> GetUserByUserName(string userName);

    }
}
