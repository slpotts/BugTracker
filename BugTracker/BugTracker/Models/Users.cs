using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker.Models
{
    public class Users
    {
        public int ProjectId { get; set; }
        public MultiSelectList assignedUsers { get; set; }
        public MultiSelectList unassignedUsers { get; set; }
        public string[] selectedUsers1 { get; set; }
        public string[] selectedUsers2 { get; set; }
    }
}