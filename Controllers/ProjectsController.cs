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
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        // GET: Projects/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Discription,DeadLine,priority")] Project p)
        {
            if (ModelState.IsValid)
            {
                var title = p.Title;
                db.Projects.Add(p);
                db.SaveChanges();
                int projectId = db.Projects.Where(x => x.Title == title).FirstOrDefault().Id;
                return RedirectToAction("AddTaskToProject", "Home", new { projectId = projectId });
            }

            return View(p);
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
