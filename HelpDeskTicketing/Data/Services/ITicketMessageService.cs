using HelpDeskTicketing.Data.ViewModels;
using HelpDeskTicketing.Models;

namespace HelpDeskTicketing.Data.Services
{
    public interface ITicketMessageService
    {

        Task<IEnumerable<TicketMessage>> GetTicketMessages();
        Task<TicketMessage> GetTicketMessage(Ticket ticket);
        Task<TicketMessage> AddTicketMessage(TicketMessage ticketMessage);
        Task<TicketMessage> DeleteTicketMessage(string Id);
        Task<TicketMessage> UpdateTicketMessage(string Id);
        Task<TicketMessage> AddFilesToTicketMessage(string ticketMessageId);

    }
}
