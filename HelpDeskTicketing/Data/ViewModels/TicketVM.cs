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
        [Display(Name ="Priority")]
        public string PriorityId { get; set; }
        public Priority Priority { get; set; }

        [Required]
        [Display(Name ="Status")]
        public string StatusId { get; set; }
        public Status Status { get; set; }

        [Required]
        [Display(Name ="Domain")]
        public string DomainId { get; set; }

        public Branch Branch { get; set; }

        public TicketMessageVM TicketMessage{ get; set; }

        public List<TicketUser>TicketUsers { get; set; }

        public List<TicketMessage> Messages { get; set; }

    }
}
