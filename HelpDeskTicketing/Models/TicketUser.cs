namespace HelpDeskTicketing.Models
{
    public class TicketUser
    {

        public string AppUserId{ get; set; }
        public AppUser AppUser { get; set; }


        public string TicketId { get; set; }
        public Ticket Ticket { get; set; }

        public string UserRole { get; set; }
    }
}
