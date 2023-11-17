using Microsoft.AspNetCore.Identity;
using System.ComponentModel;

namespace HelpDeskTicketing.Models
{
    public class AppUser: IdentityUser
    {

        [Description("Full Name")]
        public string FullName { get; set; }


        public string BranchId { get; set; }
        public Branch Branch{ get; set; }

        public List<TicketUser> TicketUsers { get; set; }

        public List<TicketMessage>Chats { get; set; }

    }
}
