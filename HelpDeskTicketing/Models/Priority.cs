namespace HelpDeskTicketing.Models
{
    public class Priority
    {

        public string Id { get; set; }

        public string Name { get; set; }

        public List<Ticket> Tickets { get; set; }

    }
}
