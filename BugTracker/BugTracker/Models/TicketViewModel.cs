using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class TicketViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }

        public TicketViewModel(Ticket ticket)
        {
            Title = "<a href='/Tickets/Details/" + ticket.Id + "'>" + ticket.Title + "</a>";
            Description = ticket.Description;
            Priority = ticket.TicketPriority.Name;
            Status = ticket.TicketStatus.Name;
            Type = ticket.TicketType.Name;
        }
    }
}