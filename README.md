# Task-Management System

## This project is build on C# .Net MVC.

## Below is the given requirenments for this project.

### A Task Management System is to help a company track the performance in their projects. “Project” contains many “Tasks”. There are multiple roles in this system. A Project Manager can create projects and tasks, and he can assign tasks to Developers.
###	Create the necessary classes using Entity Framework Code First and Authentication.
###	Create a User Manager class that has functions to add, delete and update users and roles, assign roles to users.
###	Create a Projects Manager class that contains functions to add, delete, update projects, along with any other helper functions. Those functions can only be accessed by project managers.
###	Create a Tasks Manager class that contains functions to add, delete, update tasks, and assign a task to a number of developers, those functions can only be accessed by project managers.
###	A developer can view all his tasks (can't view other developers’ tasks), also a developer can update a task to change the completed percentage of the task, or mark it totally completed, when the task is marked completed, the developer can leave a comment to describe any notes or hints.
###	A project manager can view a project and all the tasks for that project, ordered by the completion percentage (the completed ones appear first).
###	We need to add a new field (using migrations) to determine the priority of the task or a project (enum), now the previous page will have the option to order the tasks based on priority.
###	A Project manager will have a dashboard page which shows all the projects with their related tasks and assigned developers, the projects with high priorities appear first.
###	A developer will get a notification when the task is only one day to pass the deadline.
###	The project manager can see a list of the tasks that are not finished yet and passed their deadline. 
###	The project manager will get a notification if a project passed a deadline with any unfinished tasks.
###	A project manager gets a notification whenever a task or a project are completed.
###	Add a new property to the notifications to determine if it is new or opened (unread).
###	Add a link called “Notifications” to the project manager dashboard which will take the manager to see all his notifications, this link also shows the number of current unopened notifications.
