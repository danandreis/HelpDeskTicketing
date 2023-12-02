namespace HelpDeskTicketing.Data.ViewModels
{
    public class TicketEvidenceVM
    {

        //Evidence of how many tickets of one type are

        public int OpenTickets { get; set; }
        public int AssignedTickets { get; set; }
        public int ClosedTickets { get; set; }
    }
}
