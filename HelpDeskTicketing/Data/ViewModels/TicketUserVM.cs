using HelpDeskTicketing.Models;

namespace HelpDeskTicketing.Data.ViewModels
{
    public class TicketUserVM
    {

            public string AppUserId{ get; set; }

            public string TicketId { get; set; }

            public string UserRole { get; set; }

            public int Active { get; set; } //1-Active; 0 -Inactive

            public DateTime AssignmentDate { get; set; }

            public DateTime ReleaseDate { get; set; }

    }
}
