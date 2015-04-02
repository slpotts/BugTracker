using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker.Models
{
    public class TicketHistoryViewModel
    {
        public string Property { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }

        public TicketHistoryViewModel(TicketHistory ticketHistory)
        {
            Property = ticketHistory.Property;
            OldValue = ticketHistory.OldValue;
            NewValue = ticketHistory.NewValue;
        }
    }
}