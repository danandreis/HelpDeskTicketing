using HelpDeskTicketing.Models;

namespace HelpDeskTicketing.Data.Services
{
    public class TicketMessageService : ITicketMessageService
    {
        private readonly AppDbContext _context;

        public TicketMessageService(AppDbContext context)
        {
            _context = context;
        }

        public Task<TicketMessage> AddFilesToTicketMessage(string ticketMessageId)
        {
            throw new NotImplementedException();
        }

        public async Task<TicketMessage> AddTicketMessage(TicketMessage ticketMessage)
        {

            await _context.AddAsync(ticketMessage);

            if(await _context.SaveChangesAsync() > 0) {

                return ticketMessage;
            
            }

            return null;

        }

        public Task<TicketMessage> DeleteTicketMessage(string Id)
        {
            throw new NotImplementedException();
        }

        public Task<TicketMessage> GetTicketMessage(Ticket ticket)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TicketMessage>> GetTicketMessages()
        {
            throw new NotImplementedException();
        }

        public Task<TicketMessage> UpdateTicketMessage(string Id)
        {
            throw new NotImplementedException();
        }
    }
}
