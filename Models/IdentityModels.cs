using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace TaskAssignment.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class UserManager
    {
        private protected UserManager<IdentityUser> userManager;
        private protected RoleManager<IdentityRole> roleManager;

        public UserManager()
        {
            userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>());
            roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>());
        }

        public bool isRoleExist(string roleName)
        {
            return roleManager.RoleExists(roleName);
        }

        public bool createRole(string roleName)
        {
            roleName = roleName.ToLower();
            bool result = false;
            if (roleManager.RoleExists(roleName))
            {
                return true;
            }
            else
            {
                result = roleManager.Create(new IdentityRole(roleName)).Succeeded;
            }

            return result;
        }

        public bool deleteRole(string roleName)
        {
            roleName = roleName.ToLower();
            bool result = false;
            if (roleManager.RoleExists(roleName))
            {
                var users = roleManager.FindByName(roleName).Users.ToList().Select(x => x.UserId).ToList();
                users.ForEach(uId =>
                {
                    userManager.RemoveFromRole(uId, roleName);
                });
                result = roleManager.Delete(roleManager.FindByName(roleName)).Succeeded;
            }
            return result;
        }

        public bool addUserToRole(string roleName, string userId)
        {
            roleName = roleName.ToLower();
            bool result = false;
            if (roleManager.RoleExists(roleName) && userManager.FindById(userId) != null)
            {
                result = userManager.IsInRole(userId, roleName);
                if (!result)
                {
                    result = userManager.AddToRole(userId, roleName).Succeeded;
                }
            }
            return result;
        }

        public bool deleteUserFromRole(string roleName, string userId)
        {
            roleName = roleName.ToLower();
            bool result = false;
            if (roleManager.RoleExists(roleName) && userManager.FindById(userId) != null)
            {
                if (userManager.IsInRole(userId, roleName))
                {
                    result = userManager.RemoveFromRole(userId, roleName).Succeeded;
                }
            }
            return result;
        }
    }




    public class ApplicationUser : IdentityUser
    {
        public string userType { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
        public virtual ICollection<UserProject> UserProjects { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public enum userRoles
    {
        developer,
        projectmanager
    }


    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Project> Projects { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<UserProject> userProjects { get; set; }
        public DbSet<Notification> notifications { get; set; }
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }

    public class UserProject
    {
        public int id { get; set; }
        public string userId { get; set; }
        public virtual ApplicationUser user { get; set; }
        public int projectId { get; set; }
        public virtual Project project { get; set; }
    }

    public class Project
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(30), MinLength(2)]
        public string Title { get; set; }
        [Required]
        [MinLength(10)]
        public string Discription { get; set; }
        public DateTime DeadLine { get; set; }
        public Priority priority { get; set; }
        public virtual ICollection<Task> tasks { get; set; }
        public virtual ICollection<UserProject> UserProjects { get; set; }
    }

    public class ProjectViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Discription { get; set; }
        public DateTime DeadLine { get; set; }
        public Priority priority { get; set; }
        public List<TaskWithUser> Tasks { get; set; }
        public int? notificationNumber { get; set; }
    }

    public class TaskWithUser
    {
        public int Id { get; set; }
        public string TaskTitle { get; set; }
        public string developer { get; set; }
        public string TaskDetails { get; set; }
        public Priority priority { get; set; }
        public DateTime DeadLine { get; set; }
        public int ComplitionPercentage { get; set; }
        public bool Completed { get; set; }
        public int ProjectId { get; set; }
        public string userId { get; set; }
        public string notes { get; set; }
    }

    public class Task
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(30), MinLength(2)]
        public string TaskTitle { get; set; }
        [Required]
        [MinLength(10)]
        public string TaskDetails { get; set; }
        public Priority priority { get; set; }
        public DateTime DeadLine { get; set; }
        [Range(0, 100)]
        public int ComplitionPercentage { get; set; }
        public bool Completed { get; set; }
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }
        public string userId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public string notes { get; set; }
    }

    public class NotificationViewModel
    {
        public int Id { get; set; }
        public bool Opened { get; set; }
        public string Title { get; set; }
        public string Details { get; set; }
        public Priority priority { get; set; }
        public DateTime DeadLine { get; set; }
        public string notificationOptions { get; set; }
        public int NotificationUpdateId { get; set; }
    }

    public class Notification
    {
        public int Id { get; set; }
        public int NotificationUpdateId { get; set; }
        public bool Opened { get; set; }
        public string notificationOptions { get; set; }
    }

    public enum NotificationOptions
    {
        TaskCompletion,
        ProjectCompletion,
        ProjectPassedDeadline
    }

    public enum Priority
    {
        higest,
        high,
        medium,
        low,
        lowest
    }
    [Authorize(Roles = "projectmanager")]
    public class TaskManager
    {
        private protected ApplicationDbContext db;
        public TaskManager()
        {
            db = new ApplicationDbContext();
        }

        public ICollection<Task> getAllTasksOfUser(string userId)
        {
            var taskReturning = db.Tasks.Where(x => x.userId == userId).ToList();
            if (taskReturning == null || taskReturning.Count == 0)
            {
                return new List<Task>();
            }
            else
            {
                return taskReturning;
            }
        }

        public Task CreateTask(Task t)
        {

            Task dbTask = new Task();
            dbTask.TaskDetails = t.TaskDetails;
            dbTask.TaskTitle = t.TaskTitle;
            dbTask.ComplitionPercentage = t.ComplitionPercentage;
            db.Tasks.Add(dbTask);
            db.SaveChanges();
            return db.Tasks.Where(x => x.TaskTitle == dbTask.TaskTitle).FirstOrDefault();
        }

        public Task GetTaskById(int id)
        {
            return db.Tasks.Where(x => x.Id == id).FirstOrDefault();
        }

        public ICollection<Task> GetAllTaskOfProject(string projectTitle)
        {
            var a = db.Projects.Where(x => x.Title == projectTitle).Include(x => x.tasks).FirstOrDefault();
            return a.tasks.ToList();
        }

        public Task RemoveTask(Task t)
        {
            Task dbTask = db.Tasks.Where(x => x.Id == t.Id).Include(x => x.User).FirstOrDefault();
            db.Tasks.Remove(dbTask);
            db.SaveChanges();
            return db.Tasks.Where(x => x.Id == t.Id).FirstOrDefault();
        }

        public Task UpdateTask(Task t)
        {

            Task dbTask = db.Tasks.Where(x => x.Id == t.Id).FirstOrDefault();
            dbTask.TaskDetails = t.TaskDetails;
            dbTask.TaskTitle = t.TaskTitle;
            dbTask.ComplitionPercentage = t.ComplitionPercentage;
            db.SaveChanges();
            return db.Tasks.Where(x => x.Id == t.Id).FirstOrDefault();

        }

        public List<int> GetAllCompletedTaskId()
        {
            List<int> completedTasks = new List<int>();

            completedTasks.AddRange(db.Tasks.Where(x => x.Completed == true).Select(x => x.Id).ToList());

            return completedTasks;
        }
    }

    [Authorize(Roles = "projectmanager")]
    public class ProjectManager
    {
        public ApplicationDbContext db;
        public ProjectManager()
        {
            db = new ApplicationDbContext();
        }

        public List<NotificationViewModel> GetAllNotificationDetails()
        {
            List<NotificationViewModel> notificationsList = new List<NotificationViewModel>();
            var notificationList = db.notifications.ToList().OrderBy(x => x.Opened == false).ToList();
            notificationList.ForEach(e =>
            {
                NotificationViewModel nvm = new NotificationViewModel();
                nvm.notificationOptions = e.notificationOptions;
                nvm.NotificationUpdateId = e.NotificationUpdateId;
                nvm.Opened = e.Opened;
                if (e.notificationOptions == "TaskCompletion")
                {
                    Task t = db.Tasks.Find(e.NotificationUpdateId);
                    nvm.priority = t.priority;
                    nvm.Title = t.TaskTitle;
                    nvm.DeadLine = t.DeadLine;
                    nvm.Details = t.TaskDetails;
                }
                else if (e.notificationOptions == "ProjectCompletion" || e.notificationOptions == "ProjectPassedDeadline")
                {
                    Project proj = db.Projects.Find(e.NotificationUpdateId);
                    nvm.priority = proj.priority;
                    nvm.Title = proj.Title;
                    nvm.DeadLine = proj.DeadLine;
                    nvm.Details = proj.Discription;
                }
                notificationsList.Add(nvm);
            });
            db.notifications.ToList().ForEach(x => x.Opened = true);
            db.SaveChanges();
            return notificationsList;
        }
        public int NumberOfUpdates()
        {
            return db.notifications.Where(x => x.Opened == true).Count();
        }

        public bool UpdateNotificationTable(int NotificationUpdateId, string notificationOptions, bool opened)
        {
            if (db.notifications.Where(x => x.NotificationUpdateId == NotificationUpdateId).FirstOrDefault() == null)
            {
                Notification nt = new Notification();
                nt.notificationOptions = notificationOptions;
                nt.NotificationUpdateId = NotificationUpdateId;
                nt.Opened = opened;
                db.notifications.Add(nt);
                db.SaveChanges();
            }
            return true;
        }

        public List<int> GetAllCompletedProjectsId()
        {
            List<int> AllCompletedProjectId = new List<int>();

            var projects = db.Projects.ToList();
            foreach (var p in projects)
            {
                bool temp = true;
                foreach (var t in p.tasks)
                {
                    if (t.Completed == false)
                    {
                        temp = false;
                        break;
                    }
                }
                if (temp)
                {
                    AllCompletedProjectId.Add(p.Id);
                }
            }
            return AllCompletedProjectId;
        }

        public List<int> GetAllProjectsPassedDeadlineId()
        {
            List<int> projectIds = new List<int>();
            projectIds.AddRange(db.Projects.Where(x => x.DeadLine <= DateTime.Now).Select(x => x.Id));
            return projectIds;
        }

        public List<Project> GetAllProjectsPassedDeadline()
        {
            List<Project> projects = db.Projects.Where(x => x.DeadLine <= DateTime.Now).ToList();
            projects.ForEach(p =>
            {
                p.tasks = p.tasks.Where(x => x.Completed == false).ToList();
            });

            return projects;
        }
        public string GetUserEmail(string userID)
        {
            return db.Users.Find(userID).Email.ToString();
        }

        public Project AddProject(Project p)
        {
            if (p.Title != null && p.Discription != null)
            {
                var title = p.Title;
                db.Projects.Add(p);
                db.SaveChanges();
                return db.Projects.Where(x => x.Title == title).FirstOrDefault();

            }
            return new Project();
        }

        public Project RemoveProject(Project p)
        {

            if (p.Title != null && p.Discription != null)
            {

                var id = p.Id;
                Project dbProject = db.Projects.Find(p.Id);
                var tasks = db.Projects.Find(dbProject.Id).tasks.ToList();
                var uP = db.userProjects.Where(x => x.projectId == p.Id).ToList();

                tasks.ForEach(t =>
                {
                    ApplicationUser u = db.Users.Find(t.userId);
                    db.Tasks.Remove(t);
                    u.Tasks.Remove(t);
                });
                uP.ForEach(up =>
                {
                    db.userProjects.Remove(up);
                });
                db.Projects.Remove(dbProject);
                db.SaveChanges();
                return db.Projects.Where(x => x.Id == id).FirstOrDefault();
            }

            return p;
        }

        public Project GetProject(int id)
        {
            Project p = db.Projects.Where(x => x.Id == id).FirstOrDefault();
            p.tasks.ToList();
            p.UserProjects.ToList();
            return p;
        }

        public Project GetProjectWithUnfinishedTasks(int id)
        {
            Project p = db.Projects.Where(x => x.Id == id).FirstOrDefault();
            p.tasks = p.tasks.Where(x => x.Completed == false).ToList();
            p.UserProjects.ToList();
            return p;
        }

        public List<Project> GetAllProjectsWithUnfinishedTasks()
        {
            List<Project> projects = db.Projects.ToList();
            projects.ForEach(p =>
            {
                p.tasks = p.tasks.Where(x => x.Completed == false).ToList();
            });
            return projects;
        }




        public List<Project> GetAllProjects()
        {
            List<Project> projects = db.Projects.ToList();
            if (projects.Count == 0 || projects == null)
            {
                projects = db.Projects.ToList();
            }
            return projects;
        }

        public List<Project> GetAllProjectsByDeadline()
        {
            List<Project> projects = db.Projects.ToList().OrderByDescending(x => x.DeadLine.Ticks).ToList();
            return projects;
        }



        public Project UpdateProject(Project p)
        {

            Project dbProject = db.Projects.Where(x => x.Id == p.Id).FirstOrDefault();
            dbProject.tasks.ToList();
            dbProject.UserProjects.ToList();
            dbProject.Title = p.Title;
            dbProject.Discription = p.Discription;

            dbProject.tasks.ToList().ForEach(x =>
            {
                dbProject.tasks.Remove(x);
            });
            p.tasks.ToList().ForEach(x =>
            {
                dbProject.tasks.Add(x);
            });

            dbProject.UserProjects.ToList().ForEach(x =>
            {
                dbProject.UserProjects.Remove(x);
            });
            p.UserProjects.ToList().ForEach(x =>
            {
                dbProject.UserProjects.Add(x);
            });

            db.SaveChanges();
            return dbProject;
        }

    }

}


