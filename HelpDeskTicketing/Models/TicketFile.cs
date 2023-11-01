namespace HelpDeskTicketing.Models
{
    public class TicketFile
    {

        public string Id { get; set; }
        public string FileName { get; set; }

        public string TicketId{ get; set; }
        public Ticket Ticket { get; set; }
    }
}
