using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TaskAssignment.Models;
namespace TaskAssignment.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserManager manager = new UserManager();
        private TaskManager taskManager = new TaskManager();
        private ProjectManager p = new ProjectManager();

        [Authorize]
        public ActionResult Index()
        {
            bool a = User.IsInRole("developer");
            bool b = User.IsInRole("projectmanager");
            if (!a && !b)
            {
                ViewBag.UserRole = "Please Select atleast one work description";
                ViewBag.HaveRole = false;
            }
            else
            {
                if (a && !b)
                {
                    ViewBag.UserRole = "developer";
                }
                else if (b && !a)
                {
                    ViewBag.UserRole = "projectmanager";
                }
                else if (a && b)
                {
                    ViewBag.UserRole = "Developer/ProjectManager";
                }
                ViewBag.HaveRole = true;
            }


            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Authorize]
        public ActionResult ChoseRole(string role)
        {
            bool result = false;
            if (role == "developer" || role == "projectmanager")
            {

                if (!manager.isRoleExist(role))
                {
                    result = manager.createRole(role);
                }
                manager.addUserToRole(role, User.Identity.GetUserId());
                var u = db.Users.Find(User.Identity.GetUserId());
                if (role == "developer")
                {
                    u.userType = userRoles.developer.ToString();
                    db.SaveChanges();
                    return RedirectToAction("DeveloperDashBoard");
                }
                else if (role == "projectmanager")
                {
                    u.userType = userRoles.projectmanager.ToString();
                    db.SaveChanges();
                    return RedirectToAction("ProjectmanagerDashboard");
                }

            }

            return View();
        }

        [Authorize(Roles = "developer")]
        public ActionResult DeveloperDashBoard()
        {

            if (!User.IsInRole("developer"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var developer = db.Users.Find(User.Identity.GetUserId());
            List<Task> tasks = taskManager.getAllTasksOfUser(developer.Id).OrderBy(x => x.priority).ToList();
            return View(tasks);

        }

        public ActionResult UpdateTask(int? taskId)
        {

            if (taskId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.taskUpdated = false;
            Task t = db.Tasks.Find(Convert.ToInt32(taskId));
            ViewBag.TaskFound = true;
            return View(t);

        }

        [HttpPost]
        public ActionResult UpdateTask(int? taskId, int ComplitionPercentage, bool Completed, string notes = "")
        {
            if (taskId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Task t = db.Tasks.Find(Convert.ToInt32(taskId));
            t.ComplitionPercentage = ComplitionPercentage;
            t.Completed = Completed;
            if (Completed == true)
            {
                p.UpdateNotificationTable(t.Id, NotificationOptions.TaskCompletion.ToString(), false);
            }
            t.notes = notes;
            db.SaveChanges();
            ViewBag.taskUpdated = true;
            return View(t);
        }

        [Authorize(Roles = "projectmanager")]
        public ActionResult ProjectmanagerDashboard()
        {
            if (!User.IsInRole("projectmanager"))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var projects = p.GetAllProjects();
            List<ProjectViewModel> ProjectForView = new List<ProjectViewModel>();
            projects.ForEach(proj =>
            {
                ProjectViewModel projectView = new ProjectViewModel();
                projectView.Id = proj.Id;
                projectView.Title = proj.Title;
                projectView.Discription = proj.Discription;
                projectView.DeadLine = proj.DeadLine;
                projectView.priority = proj.priority;
                if (proj.tasks != null && proj.tasks.Count != 0)
                {
                    projectView.Tasks = new List<TaskWithUser>();
                    proj.tasks.ToList().ForEach(t =>
                    {
                        TaskWithUser taskWithUser = new TaskWithUser();
                        taskWithUser.Id = t.Id;
                        taskWithUser.notes = t.notes;
                        taskWithUser.priority = t.priority;
                        taskWithUser.ProjectId = t.ProjectId;
                        taskWithUser.TaskDetails = t.TaskDetails;
                        taskWithUser.TaskTitle = t.TaskTitle;
                        taskWithUser.userId = t.userId;
                        taskWithUser.developer = p.GetUserEmail(t.userId);
                        projectView.Tasks.Add(taskWithUser);
                    });

                }
                else
                {
                    projectView.Tasks = new List<TaskWithUser>();
                }
                ProjectForView.Add(projectView);
            });
            ViewBag.Notification = "notifications";

            p.GetAllProjectsPassedDeadlineId().ForEach(id =>
            {
                p.UpdateNotificationTable(id, NotificationOptions.ProjectPassedDeadline.ToString(), false);
            });
            taskManager.GetAllCompletedTaskId().ForEach(id =>
            {
                p.UpdateNotificationTable(id, NotificationOptions.TaskCompletion.ToString(), false);
            });
            p.GetAllCompletedProjectsId().ForEach(id =>
            {
                p.UpdateNotificationTable(id, NotificationOptions.TaskCompletion.ToString(), false);
            });

            ViewBag.Notification = "notifications" + "(" + p.NumberOfUpdates().ToString() + ")";

            return View(ProjectForView);
            
        }

        public ActionResult ViewNotifications()
        {
            List<NotificationViewModel> notifications = p.GetAllNotificationDetails();
            return View(notifications);
        }

        public ActionResult ProjectsByDeadLine()
        {

            List<Project> projects = p.GetAllProjectsByDeadline();
            List<ProjectViewModel> ProjectForView = new List<ProjectViewModel>();
            projects.ForEach(proj =>
            {
                ProjectViewModel projectView = new ProjectViewModel();
                projectView.Id = proj.Id;
                projectView.Title = proj.Title;
                projectView.Discription = proj.Discription;
                projectView.DeadLine = proj.DeadLine;
                projectView.priority = proj.priority;
                if (proj.tasks != null && proj.tasks.Count != 0)
                {
                    projectView.Tasks = new List<TaskWithUser>();
                    proj.tasks.ToList().ForEach(t =>
                    {
                        TaskWithUser taskWithUser = new TaskWithUser();
                        taskWithUser.Id = t.Id;
                        taskWithUser.notes = t.notes;
                        taskWithUser.priority = t.priority;
                        taskWithUser.ProjectId = t.ProjectId;
                        taskWithUser.TaskDetails = t.TaskDetails;
                        taskWithUser.TaskTitle = t.TaskTitle;
                        taskWithUser.userId = t.userId;
                        taskWithUser.developer = p.GetUserEmail(t.userId);
                        projectView.Tasks.Add(taskWithUser);
                    });

                }
                else
                {
                    projectView.Tasks = new List<TaskWithUser>();
                }
                ProjectForView.Add(projectView);
            });

            return View(ProjectForView);
        }


        public ActionResult GetProjectsPassedDeadline()
        {
            var projects = p.GetAllProjectsPassedDeadline();
            List<ProjectViewModel> ProjectForView = new List<ProjectViewModel>();
            projects.ForEach(proj =>
            {
                ProjectViewModel projectView = new ProjectViewModel();
                projectView.Id = proj.Id;
                projectView.Title = proj.Title;
                projectView.Discription = proj.Discription;
                projectView.DeadLine = proj.DeadLine;
                projectView.priority = proj.priority;
                if (proj.tasks != null && proj.tasks.Count != 0)
                {
                    projectView.Tasks = new List<TaskWithUser>();
                    proj.tasks.ToList().ForEach(t =>
                    {
                        TaskWithUser taskWithUser = new TaskWithUser();
                        taskWithUser.Id = t.Id;
                        taskWithUser.notes = t.notes;
                        taskWithUser.priority = t.priority;
                        taskWithUser.ProjectId = t.ProjectId;
                        taskWithUser.TaskDetails = t.TaskDetails;
                        taskWithUser.TaskTitle = t.TaskTitle;
                        taskWithUser.userId = t.userId;
                        taskWithUser.developer = p.GetUserEmail(t.userId);
                        projectView.Tasks.Add(taskWithUser);
                    });

                }
                else
                {
                    projectView.Tasks = new List<TaskWithUser>();
                }
                ProjectForView.Add(projectView);
            });

            return View(ProjectForView);
        }

        public ActionResult UnfinishedTasks(int? projectId)
        {
            if (projectId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Project project = p.GetProjectWithUnfinishedTasks(Convert.ToInt32(projectId));
            return View(project);
        }

        public ActionResult GetAllProjectsWithUnfinishedTasks()
        {
            List<Project> projects = p.GetAllProjectsWithUnfinishedTasks();
            List<ProjectViewModel> ProjectForView = new List<ProjectViewModel>();
            projects.ForEach(proj =>
            {
                ProjectViewModel projectView = new ProjectViewModel();
                projectView.Id = proj.Id;
                projectView.Title = proj.Title;
                projectView.Discription = proj.Discription;
                projectView.DeadLine = proj.DeadLine;
                projectView.priority = proj.priority;
                if (proj.tasks != null && proj.tasks.Count != 0)
                {
                    projectView.Tasks = new List<TaskWithUser>();
                    proj.tasks.ToList().ForEach(t =>
                    {
                        TaskWithUser taskWithUser = new TaskWithUser();
                        taskWithUser.Id = t.Id;
                        taskWithUser.notes = t.notes;
                        taskWithUser.priority = t.priority;
                        taskWithUser.ProjectId = t.ProjectId;
                        taskWithUser.TaskDetails = t.TaskDetails;
                        taskWithUser.TaskTitle = t.TaskTitle;
                        taskWithUser.userId = t.userId;
                        taskWithUser.developer = p.GetUserEmail(t.userId);
                        projectView.Tasks.Add(taskWithUser);
                    });

                }
                else
                {
                    projectView.Tasks = new List<TaskWithUser>();
                }
                ProjectForView.Add(projectView);
            });

            return View(ProjectForView);
        }

        public ActionResult TaskDetails(int? taskId)
        {
            if (taskId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Task task = taskManager.GetTaskById(Convert.ToInt32(taskId));
            return View(task);
        }

        public ActionResult ProjectDetails(int? projectId)
        {
            if (projectId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Project project = p.GetProject(Convert.ToInt32(projectId));
            return View(project);
        }

        public ActionResult DeleteProject(int? projectId)
        {
            if (projectId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Project project = p.GetProject(Convert.ToInt32(projectId));
            var a = p.RemoveProject(project);
            return View(a);
        }

        public ActionResult CreateProject()
        {
            return RedirectToAction("Create", "Projects");
        }

        public ActionResult AddTaskToProject(int? projectId)
        {
            if (projectId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return RedirectToAction("Create", "Tasks", new { projectId = Convert.ToInt32(projectId) });
        }
    }
}













