using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BugTracker.Models
{
    public class TicketHistory
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string Property { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public DateTimeOffset Changed { get; set; }
        public string UserId { get; set; }

        public Ticket Ticket { get; set; }
        public ApplicationUser User { get; set; }
    }
}
