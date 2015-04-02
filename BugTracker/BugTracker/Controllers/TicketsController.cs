using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using DataTables.Mvc;
using DataTables;
using System.IO;
using System.Web.Security;

namespace BugTracker.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tickets
        public ActionResult Index()
        {

            TempData["Title"] = "Tickets";

            return View();
        }

        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }

            TempData["TicketDetail"] = ticket;
            return View(ticket);
        }

        // GET: Tickets/Create
        public ActionResult Create()
        {

            var roles = db.Roles
                          .Where(m => m.Name == "Developer")
                          .Select(m => new SelectListItem { Value = m.Id })
                          .FirstOrDefault();

            var userList = db.Users
                             .Include("Roles")
                             .Where(m => (m.Roles.Any(r => r.RoleId == roles.Value )))
                             .OrderBy(u => u.UserName)
                             .ToList();

            var userId = db.Users.Single(u => u.UserName == User.Identity.Name).Id;

            if (User.IsInRole("Submitter") || User.IsInRole("Developer"))
            {
                var projects = db.Projects
                    .Where(m => m.Users.Any(u => u.Id == userId))
                    .ToList();

                ViewBag.ProjectId = new SelectList(projects, "Id", "Name");
            }

            else
            {
                ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name");
            }
            

            ViewBag.AssignedUserId = new SelectList(userList, "Id", "UserName");
            
            ViewBag.SubmitterId = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name");
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses, "Id", "Name");
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitComment([Bind(Include = "Comment,TicketId")] TicketComment tComment)
        {
            if (ModelState.IsValid)
            {
                tComment.Created = System.DateTimeOffset.Now;
                tComment.UserId = User.Identity.GetUserId();
                db.TicketComments.Add(tComment);
                db.SaveChanges();
                return RedirectToAction("Details", new { id = tComment.TicketId });
            }
            return View(tComment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TicketAttachment([Bind(Include = "Description,TicketId")] HttpPostedFileBase attachment)
        {
            if (attachment != null)
            {
                var tAttach = new TicketAttachment();
                if (attachment.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(attachment.FileName);
                    var fileExtension = Path.GetExtension(fileName);
                    var path = Server.MapPath("~/attachments/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    tAttach.FileUrl = "/attachments/" + fileName;
                    tAttach.Created = System.DateTimeOffset.Now;
                    tAttach.UserId = User.Identity.GetUserId();

                    Ticket ticketDetails = TempData["TicketDetail"] as Ticket;

                    if (tAttach.TicketId == 0)
                    {
                        tAttach.TicketId = ticketDetails.Id;
                    }

                    if (tAttach.Description == null)
                    {
                        tAttach.Description = fileName;
                    }

                    db.TicketAttachments.Add(tAttach);
                    db.SaveChanges();
                    attachment.SaveAs(Path.Combine(path, fileName));
                    return RedirectToAction("Details", new { id = tAttach.TicketId });
                }
                else
                {
                    ModelState.AddModelError("image", "Invalid image extension.  Only .gif, .jpg, and .png are allowed.");
                    return View(attachment);
                }
            }
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Description,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,SubmitterId,AssignedUserId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                ticket.Created = System.DateTimeOffset.Now;
                ticket.Updated = System.DateTimeOffset.Now;
                ticket.SubmitterId = User.Identity.GetUserId();
                ticket.TicketStatusId = db.TicketStatuses.Single(ts => ts.Name == "New").Id;
                db.Tickets.Add(ticket);

                if (ticket.AssignedUserId != null)
                {
                    db.TicketNotifications.Add(new TicketNotification
                    {
                        TicketId = ticket.Id,
                        UserId = ticket.AssignedUserId
                    });
                }

                db.SaveChanges();


                return RedirectToAction("Index");
            }

            var roles = db.Roles
                          .Where(m => m.Name == "Developer")
                          .Select(m => new SelectListItem { Value = m.Id })
                          .FirstOrDefault();

            var userList = db.Users
                             .Include("Roles")
                             .Where(m => (m.Roles.Any(r => r.RoleId == roles.Value)))
                             .OrderBy(u => u.UserName)
                             .ToList();


            ViewBag.AssignedUserId = new SelectList(userList, "Id", "UserName", ticket.AssignedUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.SubmitterId = new SelectList(db.Users, "Id", "FirstName", ticket.SubmitterId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);

            
        }

        // GET: Tickets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            TempData["oldTicket"] = ticket;
            if (ticket == null)
            {
                return HttpNotFound();
            }

            var roles = db.Roles
                          .Where(m => m.Name == "Developer")
                          .Select(m => new SelectListItem { Value = m.Id })
                          .FirstOrDefault();

            var userList = db.Users
                             .Include("Roles")
                             .Where(m => (m.Roles.Any(r => r.RoleId == roles.Value)))
                             .OrderBy(u => u.UserName)
                             .ToList();

            ViewBag.AssignedUserId = new SelectList(userList, "Id", "UserName", ticket.AssignedUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.SubmitterId = new SelectList(db.Users, "Id", "FirstName", ticket.SubmitterId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses.Where(ts => ts.Name != "New"), "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,Created,Updated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,SubmitterId,AssignedUserId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                var oldTicket = (Ticket)TempData["oldTicket"];
                var userId = db.Users.Single(u => u.UserName == User.Identity.Name).Id;
                var ticketPriority = db.TicketPriorities.Single(tp => tp.Id == ticket.TicketPriorityId);
                var ticketStatus = db.TicketStatuses.Single(ts => ts.Id == ticket.TicketStatusId);
                var ticketType = db.TicketTypes.Single(tt => tt.Id == ticket.TicketTypeId);

                if (oldTicket.AssignedUserId == null)
                {
                    db.TicketNotifications.Add(new TicketNotification
                    {
                        TicketId = ticket.Id,
                        UserId = ticket.AssignedUserId
                    });
                }
                else if (ticket.AssignedUserId != oldTicket.AssignedUserId)
                {
                    db.TicketNotifications.Add(new TicketNotification
                    {
                        TicketId = ticket.Id,
                        UserId = ticket.AssignedUserId
                    });
                }

                if (oldTicket.Description != ticket.Description)
                {
                    db.TicketHistories.Add(new TicketHistory
                    {
                        Property = "Description",
                        Changed = System.DateTimeOffset.Now,
                        OldValue = oldTicket.Description,
                        NewValue = ticket.Description,
                        TicketId = ticket.Id,
                        UserId = userId
                    });
                }

                if (oldTicket.TicketPriorityId != ticket.TicketPriorityId)
                {
                    db.TicketHistories.Add(new TicketHistory
                    {
                            Property = "Ticket Priority",
                            Changed = System.DateTimeOffset.Now,
                            OldValue = oldTicket.TicketPriority.Name,
                            NewValue = ticketPriority.Name,
                            TicketId = ticket.Id,
                            UserId = userId
                     });
                }

                if (oldTicket.TicketStatusId != ticket.TicketStatusId)
                {
                    if (oldTicket.TicketStatusId == 1)
                    {
                        db.TicketHistories.Add(new TicketHistory
                        {
                            Property = "Ticket Status",
                            Changed = System.DateTimeOffset.Now,
                            OldValue = "New",
                            NewValue = ticketStatus.Name,
                            TicketId = ticket.Id,
                            UserId = userId
                        });
                    }
                    else
                    {
                        db.TicketHistories.Add(new TicketHistory
                        {
                            Property = "Ticket Status",
                            Changed = System.DateTimeOffset.Now,
                            OldValue = oldTicket.TicketStatus.Name,
                            NewValue = ticketStatus.Name,
                            TicketId = ticket.Id,
                            UserId = userId
                        });
                    }
                }
                
                if (oldTicket.TicketTypeId != ticket.TicketTypeId)
                {
                    db.TicketHistories.Add(new TicketHistory
                    {
                        Property = "Ticket Type",
                        Changed = System.DateTimeOffset.Now,
                        OldValue = oldTicket.TicketType.Name,
                        NewValue = ticketType.Name,
                        TicketId = ticket.Id,
                        UserId = userId
                    });
                }

                ticket.Updated = System.DateTimeOffset.Now;
                db.Entry(ticket).State = EntityState.Modified;
                if(!User.IsInRole("Administrator") && !User.IsInRole("Project Manager"))
                    db.Entry(ticket).Property(p => p.AssignedUserId).IsModified = false;

                db.Entry(ticket).Property(p => p.Created).IsModified = false;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AssignedUserId = new SelectList(db.Users, "Id", "FirstName", ticket.AssignedUserId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Name", ticket.ProjectId);
            ViewBag.SubmitterId = new SelectList(db.Users, "Id", "FirstName", ticket.SubmitterId);
            ViewBag.TicketPriorityId = new SelectList(db.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewBag.TicketStatusId = new SelectList(db.TicketStatuses.Where(ts => ts.Name != "New"), "Id", "Name", ticket.TicketStatusId);
            ViewBag.TicketTypeId = new SelectList(db.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        public JsonResult GetTAjaxData([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel)
        {
           
            var search = requestModel.Search.Value;
            var table = db.Tickets.AsQueryable();
            string usersId = User.Identity.GetUserId();

            if (!string.IsNullOrWhiteSpace(search))
            {
                table = table.Where(t => t.Title.Contains(search) || t.Description.Contains(search)
                                    || t.TicketPriority.Name.Contains(search) || t.TicketStatus.Name.Contains(search)
                                    || t.TicketType.Name.Contains(search));
            }

            var column = requestModel.Columns.FirstOrDefault(r => r.IsOrdered == true);
            if (column != null)
            {
                if (column.SortDirection == Column.OrderDirection.Descendant)
                {
                    switch (column.Data)
                    {
                        case "Title":
                            table = table.OrderByDescending(t => t.Title);
                            break;
                        case "Description":
                            table = table.OrderByDescending(t => t.Description);
                            break;
                    }
                }
                else
                {
                    switch (column.Data)
                    {
                        case "Title":
                            table = table.OrderBy(t => t.Title);
                            break;
                        case "Description":
                            table = table.OrderBy(t => t.Description);
                            break;
                    }
                }
            }
            else
                table = table.OrderByDescending(t => t.Created);



            if (User.IsInRole("Developer") || User.IsInRole("Submitter"))
            {
                
                if (User.IsInRole("Developer"))
                {
                    var result = table.Skip(requestModel.Start).Take(requestModel.Length).Where(t=>t.AssignedUserId == usersId).ToList().Select(vm => new TicketViewModel(vm));
                    return Json(new DataTablesResponse(requestModel.Draw, result, table.Count(), db.Tickets.Count()), JsonRequestBehavior.AllowGet);
                }
                else if (User.IsInRole("Submitter"))
                {
                    var result = table.Skip(requestModel.Start).Take(requestModel.Length).Where(t => t.SubmitterId == usersId).ToList().Select(vm => new TicketViewModel(vm));
                    return Json(new DataTablesResponse(requestModel.Draw, result, table.Count(), db.Tickets.Count()), JsonRequestBehavior.AllowGet);
                }
            }
            else if (User.IsInRole("Project Manager"))
            {
                var result = table.Skip(requestModel.Start).Take(requestModel.Length).Where(t => t.Project.Users.Any(pu => pu.Id == usersId)).ToList().Select(vm => new TicketViewModel(vm));
                return Json(new DataTablesResponse(requestModel.Draw, result, table.Count(), db.Tickets.Count()), JsonRequestBehavior.AllowGet);
            }
            else
            {
                var result = table.Skip(requestModel.Start).Take(requestModel.Length).ToList().Select(vm => new TicketViewModel(vm));
                return Json(new DataTablesResponse(requestModel.Draw, result, table.Count(), db.Tickets.Count()), JsonRequestBehavior.AllowGet);
            }

            return Json(new DataTablesResponse(requestModel.Draw, table, table.Count(), db.Tickets.Count()), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTHAjaxData([ModelBinder(typeof(DataTablesBinder))] IDataTablesRequest requestModel)
        {

            var search = requestModel.Search.Value;
            var table = db.TicketHistories.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                table = table.Where(t => t.Property.Contains(search) || t.OldValue.Contains(search)
                                    || t.NewValue.Contains(search));
            }

            var column = requestModel.Columns.FirstOrDefault(r => r.IsOrdered == true);
            if (column != null)
            {
                if (column.SortDirection == Column.OrderDirection.Descendant)
                {
                    switch (column.Data)
                    {
                        case "Property":
                            table = table.OrderByDescending(t => t.Property);
                            break;
                        case "OldValue":
                            table = table.OrderByDescending(t => t.OldValue);
                            break;
                        case "NewValue":
                            table = table.OrderByDescending(t => t.NewValue);
                            break;
                    }
                }
                else
                {
                    switch (column.Data)
                    {
                        case "Property":
                            table = table.OrderBy(t => t.Property);
                            break;
                        case "OldValue":
                            table = table.OrderBy(t => t.OldValue);
                            break;
                        case "NewValue":
                            table = table.OrderBy(t => t.NewValue);
                            break;
                    }
                }
            }
            else
                table = table.OrderByDescending(t => t.Changed);

            var result = table.Skip(requestModel.Start).Take(requestModel.Length).ToList().Select(vm => new TicketHistoryViewModel(vm));

            return Json(new DataTablesResponse(requestModel.Draw, result, table.Count(), db.TicketHistories.Count()), JsonRequestBehavior.AllowGet);
        }

        // GET: Tickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Ticket ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Ticket ticket = db.Tickets.Find(id);
            db.Tickets.Remove(ticket);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
