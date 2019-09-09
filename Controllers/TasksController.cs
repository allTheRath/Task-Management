using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TaskAssignment.Models;

namespace TaskAssignment.Controllers
{
    public class TasksController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Tasks/Create
        public ActionResult Create(int? projectId)
        {
            if (projectId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.ProjectName = db.Projects.Find(Convert.ToInt32(projectId)).Title;
            var users = db.Users.Where(x => x.userType == "developer").ToList();
            ViewBag.userId = new SelectList(users, "Id", "Email");
            return View();
        }

        // POST: Tasks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int? projectId, [Bind(Include = "Id,TaskTitle,TaskDetails,priority,DeadLine,ComplitionPercentage,userId")] Task task)
        {
            if (projectId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (ModelState.IsValid)
            {
                task.ProjectId = Convert.ToInt32(projectId);
                task.Project = db.Projects.Find(Convert.ToInt32(projectId));
                task.notes = "";
                task.Completed = false;
                db.Tasks.Add(task);
                db.SaveChanges();
                return RedirectToAction("ProjectDetails","Home",new { projectId = Convert.ToInt32(projectId)});
            }

            ViewBag.ProjectName = db.Projects.Find(Convert.ToInt32(projectId)).Title;
            ViewBag.userId = new SelectList(db.Users, "Id", "Email", task.userId);
            return View(task);
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
