namespace HelpDeskTicketing.Models
{
    public class TicketUser
    {

        public string AppUserId{ get; set; }
        public AppUser AppUser { get; set; }

        public string TicketId { get; set; }
        public Ticket Ticket { get; set; }

        public string UserRole { get; set; }

        public int Active { get; set; } //1-Active; 0 -Inactive
        public DateTime AssignmentDate { get; set; }
        public DateTime ReleaseDate { get; set; } //Date when User has been changed - date of ticket reassignement 
    }
}
