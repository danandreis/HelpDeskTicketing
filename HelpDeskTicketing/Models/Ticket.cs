using System.ComponentModel.DataAnnotations.Schema;

namespace HelpDeskTicketing.Models
{
    public class Ticket
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate{ get; set; }

        public Priority Priority{ get; set; }
        public string PriorityId{ get; set; }


        public string StatusId{ get; set; }
        public Status Status{ get; set; }


        public string DomainId { get; set; }
        public Domain Domain{ get; set; }


        public string BranchId { get; set; }
        public Branch Branch { get; set; }


        public List<TicketUser> TicketUsers { get; set; }

        public List<TicketMessage> Messages { get; set; }

    }
}
