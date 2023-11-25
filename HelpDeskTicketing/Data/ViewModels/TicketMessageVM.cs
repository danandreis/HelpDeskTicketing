using HelpDeskTicketing.Models;
using System.ComponentModel.DataAnnotations;

namespace HelpDeskTicketing.Data.ViewModels
{
    public class TicketMessageVM
    {

        [Required]
        public string Message { get; set; }

        public string TicketId { get; set; }

        public List<IFormFile> Files { get; set; }
    }
}
