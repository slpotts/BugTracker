using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BugTracker.Models
{
    public class TicketComment
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public DateTimeOffset Created { get; set; }
        public int TicketId { get; set; }
        public string UserId { get; set; }

        public Ticket Ticket { get; set; }
        public ApplicationUser User { get; set; }
    }
}
