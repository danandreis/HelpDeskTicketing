namespace HelpDeskTicketing.Models
{
    public class Domain
    {

        public string Id { get; set; }
        public string Name { get; set; }
        public List<Ticket> Tickets { get; set; }
    }
}
