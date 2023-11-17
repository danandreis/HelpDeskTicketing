using HelpDeskTicketing.Models;
using System.ComponentModel.DataAnnotations;

namespace HelpDeskTicketing.Data.ViewModels
{
    public class TicketVM
    {

        public string Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Display(Name ="Priority")]
        public string PriorityId { get; set; }

        [Required]
        [Display(Name ="Status")]
        public string StatusId { get; set; }

        [Required]
        [Display(Name ="Domain")]
        public string DomainId { get; set; }

 
        public Branch Branch { get; set; }

        public List<TicketUser>ticketUsers { get; set; }

    }
}
