using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace BugTracker.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set
            {
                _userManager = value;
            }
        }

        // GET: Projects
        public ActionResult Index()
        {
            TempData["Title"] = "Projects";
            return View(db.Projects.ToList());
        }

        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // GET: Projects/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles="Administrator, Project Manager")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(project);
        }

        // GET: Projects/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }

            Users userGroup = new Users();
            var userListA = project.Users.ToList();
            var userListB = db.Users
                .Where(m => (!m.Projects.Any(r => r.Id == project.Id)))
                .ToList();

            userGroup.ProjectId = project.Id;
            userGroup.assignedUsers = new MultiSelectList(userListA, "Id", "FirstName");
            userGroup.unassignedUsers = new MultiSelectList(userListB, "Id", "FirstName");

            TempData["projectUsers"] = userGroup;

            TempData["assignedUsers"] = userGroup.assignedUsers;
            TempData["unassignedUsers"] = userGroup.unassignedUsers;

            ViewBag.AssignedUsers = userGroup.assignedUsers;
            ViewBag.UnassignedUsers = userGroup.unassignedUsers;

            return View(project);
        }

        public ActionResult ProjectUsers(int id)
        {
            Users userGroup = new Users();
            Project thisProject = db.Projects.Find(id);
            var userListA = thisProject.Users.ToList();
            var userListB = db.Users
                .Where(m => (!m.Projects.Any(r => r.Id == thisProject.Id)))
                .ToList();

            userGroup.ProjectId = thisProject.Id;
            userGroup.assignedUsers = new MultiSelectList(userListA, "Id", "FirstName");
            userGroup.unassignedUsers = new MultiSelectList(userListB, "Id", "FirstName");

            return View(userGroup);
        }

        [HttpPost, ActionName("ProjectUsers")]
        [ValidateAntiForgeryToken]
        public ActionResult ProjectUsers(Users model)
        {
            var model2 = TempData["projectusers"] as Users;
            model.ProjectId = model2.ProjectId;
            if (ModelState.IsValid)
            {
                Project theProject = db.Projects.Find(model.ProjectId);
                if (model.selectedUsers1 != null)
                {
                    foreach (string userId in model.selectedUsers1)
                    {
                        theProject.Users.Remove(db.Users.Find(userId));
                        db.Entry(theProject).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                if (model.selectedUsers2 != null)
                {
                    foreach (string userId2 in model.selectedUsers2)
                    {
                        theProject.Users.Add(db.Users.Find(userId2));
                        db.Entry(theProject).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }

        

        // GET: Projects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = db.Projects.Find(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Project project = db.Projects.Find(id);
            db.Projects.Remove(project);
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
