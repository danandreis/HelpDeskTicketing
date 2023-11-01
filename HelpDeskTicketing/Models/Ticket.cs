using System.ComponentModel.DataAnnotations.Schema;

namespace HelpDeskTicketing.Models
{
    public class Ticket
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate{ get; set; }
        public string Description{ get; set; }
        public string Priority{ get; set; }


        public string StatusId{ get; set; }
        public Status Status{ get; set; }


        public string DomainId { get; set; }
        public Domain Domain{ get; set; }


        public string BranchId { get; set; }
        public Branch Branch { get; set; }


        public List<TicketUser> TicketUsers { get; set; }

        public List<Chat> Chats { get; set; }
        public List<TicketFile>TicketFiles { get; set; }

    }
}
