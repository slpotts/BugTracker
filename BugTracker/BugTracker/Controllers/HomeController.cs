using BugTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataTables.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace BugTracker.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            
            
            if (User.IsInRole("Developer"))
            {
                TempData["Dev"] = db.Tickets.Where(t => t.AssignedUser.UserName == User.Identity.Name)
                    .Take(10);
                
            }
            if (User.IsInRole("Submitter"))
            {
                TempData["Sub"] = db.Tickets.Where(s => s.AssignedUser.UserName == User.Identity.Name)
                    .Take(10);
                
            }

            int count = db.Projects.Count();

            int[] resolved = new int[count];
            int i = 0;
            int countR = 0;
            int countU = 0;
            int[] unResolved = new int[count];
            string[] projectName = new string[count];
            List<Project> projects = db.Projects.Include("Tickets").ToList();
            foreach (var project in projects)
            {
                projectName[i] = project.Name;                
                i++;
            }
            i = 0;
            foreach (var project in projects)
            {
                foreach (var ticket in project.Tickets)
                {
                    if (ticket.TicketStatus.Name == "Resolved")
                        countR++;
                    else if (ticket.TicketStatus.Name != "Resolved")
                        countU++;
                }
                resolved[i] = countR;
                unResolved[i] = countU;
            }
            TempData["Resolved"] = resolved;
            TempData["Unresolved"] = unResolved;
            TempData["ProjectName"] = projectName;

            TempData["Title"] = "Home Page";

            return View();
        }

        public ActionResult Menu()
        {
            var user = db.Users.Find(User.Identity.GetUserId());

            ViewBag.Title = TempData["Title"];

            return View(new MenuViewModel { 
                ticketNots = user.Notifications.Where(tn => tn.Seen == false).Count(),
                projects = user.Projects.Count(),
                resolvedTicks = db.Tickets.Where(t => t.TicketStatus.Name != "Resolved").Count(t=>t.AssignedUserId == user.Id)
            });
        }
        
        public JsonResult GetAjaxData([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel)
        {
            var table = db.Tickets.AsQueryable().OrderByDescending(t=> t.Created);
            string usersId = User.Identity.GetUserId();

            if (User.IsInRole("Developer") || User.IsInRole("Submitter"))
            {

                if (User.IsInRole("Developer"))
                {
                    var result = table.Skip(requestModel.Start).Take(requestModel.Length).Where(t => t.AssignedUserId == usersId).Take(10).ToList().Select(vm => new TicketViewModel(vm));
                    return Json(new DataTablesResponse(requestModel.Draw, result, table.Count(), db.Tickets.Count()), JsonRequestBehavior.AllowGet);
                }
                else if (User.IsInRole("Submitter"))
                {
                    var result = table.Skip(requestModel.Start).Take(requestModel.Length).Where(t => t.SubmitterId == usersId).Take(10).ToList().Select(vm => new TicketViewModel(vm));
                    return Json(new DataTablesResponse(requestModel.Draw, result, table.Count(), db.Tickets.Count()), JsonRequestBehavior.AllowGet);
                }
            }
            else if (User.IsInRole("Project Manager"))
            {
                var result = table.Skip(requestModel.Start).Take(requestModel.Length).Where(t => t.Project.Users.Any(pu => pu.Id == usersId)).Take(10).ToList().Select(vm => new TicketViewModel(vm));
                return Json(new DataTablesResponse(requestModel.Draw, result, table.Count(), db.Tickets.Count()), JsonRequestBehavior.AllowGet);
            }
            else
            {
                var result = table.Skip(requestModel.Start).Take(requestModel.Length).Take(10).ToList().Select(vm => new TicketViewModel(vm));
                return Json(new DataTablesResponse(requestModel.Draw, result, table.Count(), db.Tickets.Count()), JsonRequestBehavior.AllowGet);
            }

            var table2 = db.Tickets.OrderByDescending(t => t.Updated).Take(10).ToList().Select(vm => new TicketViewModel(vm));
            return Json(new DataTablesResponse(requestModel.Draw, table2, table.Count(), db.Tickets.Count()), JsonRequestBehavior.AllowGet);                
        }

        //public ActionResult About()
        //{
        //    ViewBag.Message = "Your application description page.";

        //    return View();
        //}

        //public ActionResult Contact()
        //{
        //    ViewBag.Message = "Your contact page.";

        //    return View();
        //}
    }
}