namespace HelpDeskTicketing.Models
{
    public class TicketFile
    {

        public string Id { get; set; }
        public string FileName { get; set; }  //Name of the file saved on PC
        public string DisplayName { get; set; }  //Name to by displayed to user

        public string TicketMessageId { get; set; }
        public TicketMessage TicketMessage { get; set; }
    }
}
