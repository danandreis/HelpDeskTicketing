﻿namespace HelpDeskTicketing.Models
{
    public class Chat
    {

        public string Id { get; set; }
        public DateTime Date { get; set; }
        public string Message { get; set; }


        public string AppUserId { get; set; }
        public AppUser AppUser{ get; set; }


        public string TicketId { get; set; }
        public Ticket Ticket{ get; set; }
    }
}
