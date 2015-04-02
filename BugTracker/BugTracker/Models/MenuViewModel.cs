using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace BugTracker.Models
{
    public class MenuViewModel
    {
        public int ticketNots { get; set; }
        public int projects { get; set; }
        public int resolvedTicks { get; set; }
    }
}