using HelpDeskTicketing.Data.ViewModels;

namespace HelpDeskTicketing.Data.Services
{
    public interface ITicketFilesService
    {

        Task<IEnumerable<IFormFile>> GetFilesFormDatabase(string ticketId);
        Task<bool>AddFileToDatabase(IEnumerable<IFormFile> files, string ticketId);

    }
}
