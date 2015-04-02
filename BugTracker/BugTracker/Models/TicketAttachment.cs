using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BugTracker.Models
{
    public class TicketAttachment
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string FilePath { get; set; }
        public string Description { get; set; }
        public DateTimeOffset Created { get; set; }
        public string UserId { get; set; }
        public string FileUrl { get; set; }

        public Ticket Ticket { get; set; }
        public ApplicationUser User { get; set; }
    }
}
