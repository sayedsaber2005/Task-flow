using ProjectManagement.BL.Interfaces;
using ProjectManagement.DTOs;
using ProjectManagement.DTOs.ProjectMembers;
using ProjectManagement.DTOs.Projects;
using ProjectManagement.DTOs.Stats;
using ProjectManagement.DTOs.Tasks;
using ProjectManagement.Models;
using ProjectManagement.Repositories.Interfaces;
using System.Diagnostics;
using System.Security.Claims;

namespace ProjectManagement.BL.Implementations
{
    public class ClsProject : IProject
    {
        private readonly IProjectRepository _repo;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IProjectMemberRepository _projectMemberRepo;
        private readonly ITaskRepository _taskRepo;
        private readonly IProjectRepository _projectRepo;
        private readonly IProjectMemberRepository _projectMemberRepository;
        private readonly IActivity _activity;
        public ClsProject(IProjectRepository repo, 
            IHttpContextAccessor httpContext, 
            IProjectMemberRepository projectMemberRepo,
            ITaskRepository taskRepo,
            IProjectRepository projectRepo,
            IProjectMemberRepository projectMemberRepository,
            IActivity activity)
        {
            _repo = repo;
            _httpContext = httpContext;
            _projectMemberRepo = projectMemberRepo;
            _taskRepo = taskRepo;
            _projectRepo = projectRepo;
            _projectMemberRepository = projectMemberRepository;
            _activity = activity;
            _projectRepo = projectRepo;
            _projectMemberRepository = projectMemberRepository;
        }

        public ApiResponse GetAll()
        {
            var projects = _repo.GetAll();

            var result = projects.Select(p => new ProjectsDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                CreatedBy = p.CreatedBy,
                CreatedAt = p.CreatedAt
            });

            return new ApiResponse
            {
                Data = result,
                StatusCode = "200"
            };
        }

        public ApiResponse GetById(int id)
        {
            var project = _repo.GetById(id);

            if (project == null)
                return new ApiResponse { StatusCode = "404" };

            var creater = _projectMemberRepository.GetByUserAndProject(project.CreatedBy, project.Id);

            return new ApiResponse
            {
                Data = new ProjectsDTO
                {
                    Id = project.Id,
                    Name = project.Name,
                    Description = project.Description,
                    CreatedBy = creater.User.FirstName + " " + creater.User.LastName,
                    CreatedAt = project.CreatedAt,
                    TotalTasks = project.TbTasks.Count,
                    TotalMembers = project.TbProjectMembers.Count
                },
                StatusCode = "200"
            };
        }

        public ApiResponse GetMyProjects()
        {
            var userId = _httpContext.HttpContext.User
            .FindFirstValue(ClaimTypes.NameIdentifier);

            var projects = _projectMemberRepo
                .GetByUser(userId)
                .Select(m => m.Project)
                .Where(p => p != null)
                .Distinct()
                .Select(p => new ProjectsDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    CreatedBy = p.CreatedBy,
                    CreatedAt = p.CreatedAt
                });

            return new ApiResponse
            {
                Data = projects,
                StatusCode = "200"
            };
        }

        public ApiResponse GetProjectStats(int projectId)
        {
            // authorize the user
            var user = _httpContext.HttpContext.User;
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            // if the user is a project manager, check if they are a member of the project
            if (user.IsInRole("Project Manager"))
            {
                var isMember = _projectMemberRepo.GetByUserAndProject(userId, projectId);

                if (isMember == null)
                {
                    return new ApiResponse
                    {
                        StatusCode = "403",
                        Errors = new List<object>
                        {
                            new { Message = "You don't have access to this project" }
                        }
                    };
                }
            }

            var result = new ApiResponse();

            var tasks = _taskRepo.GetByProjectId(projectId);

            if (tasks == null || !tasks.Any())
            {
                result.Data = new ProjectStatsDTO
                {
                    TotalTasks = 0,
                    DoneTasks = 0,
                    TodoTasks = 0,
                    OverDueTasks = 0,
                    InProgressTasks = 0,
                    ProgressPercentage = 0
                };
                result.StatusCode = "200";
                return result;
            }

            var total = tasks.Count;
            var done = tasks.Count(t => t.Status.ToLower() == "done");
            var todo = tasks.Count(t => t.Status.ToLower() == "todo");
            var inProgress = tasks.Count(t => t.Status.ToLower() == "in_progress");
            var overDue = tasks.Count(t => t.DueDate < DateTime.UtcNow && t.Status.ToLower() != "done");

            var progress = (double)done / total * 100;

            result.Data = new ProjectStatsDTO
            {
                TotalTasks = total,
                DoneTasks = done,
                TodoTasks = todo,
                OverDueTasks = overDue,
                InProgressTasks = inProgress,
                ProgressPercentage = Math.Round(progress, 2)
            };

            result.StatusCode = "200";
            return result;
        }

        public ApiResponse GetProjectDashboard(int projectId)
        {
            var result = new ApiResponse();

            var user = _httpContext.HttpContext.User;
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            // Authorization
            var isMember = _projectMemberRepo.GetByUserAndProject(userId, projectId);
            if (isMember == null && !user.IsInRole("Admin"))
            {
                result.StatusCode = "403";
                result.Errors.Add(new { Message = "Access denied" });
                return result;
            }

            // Project
            var project = _projectRepo.GetById(projectId);

            // Tasks
            var tasks = _taskRepo.GetByProjectId(projectId);

            // Members
            var members = _projectMemberRepo.GetByProjectId(projectId);

            // Stats
            var total = tasks.Count;
            var done = tasks.Count(t => t.Status == "done");
            var todo = tasks.Count(t => t.Status == "todo");
            var inProgress = tasks.Count(t => t.Status == "in_progress");
            var overDue = tasks.Count(t => t.DueDate < DateTime.UtcNow && t.Status != "done");
            var progress = total == 0 ? 0 : (double)done / total * 100;

            var dashboard = new ProjectDashboardDTO
            {
                Project = new ProjectsDTO
                {
                    Id = project.Id,
                    Name = project.Name,
                    Description = project.Description,
                    CreatedAt = project.CreatedAt
                },

                Tasks = tasks.Select(t => new TasksDTO
                {
                    Id = t.Id,
                    Title = t.Title,
                    Status = t.Status,
                    Priority = t.Priority
                }).ToList(),

                Members = members.Select(m => new ProjectMemberDTO
                {
                    UserId = m.UserId,
                    UserName = m.User?.UserName,
                    Role = m.Role
                }).ToList(),

                Stats = new ProjectStatsDTO
                {
                    TotalTasks = total,
                    DoneTasks = done,
                    TodoTasks = todo,
                    InProgressTasks = inProgress,
                    OverDueTasks = overDue,
                    ProgressPercentage = Math.Round(progress, 2)
                }
            };

            result.Data = dashboard;
            result.StatusCode = "200";

            return result;
        }

        public ApiResponse GetProjectDetails(int projectId)
        {
            var result = new ApiResponse();

            var user = _httpContext.HttpContext.User;
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

            // Authorization
            var isMember = _projectMemberRepo.GetByUserAndProject(userId, projectId);
            if (isMember == null && !user.IsInRole("Admin"))
            {
                result.StatusCode = "403";
                result.Errors.Add(new { Message = "Access denied" });
                return result;
            }

            // get data
            var project = _repo.GetProjectWithDetails(projectId);

            if (project == null)
            {
                result.Errors.Add(new { Message = "Project not found" });
                result.StatusCode = "404";
                return result;
            }

            // map data
            var dto = new ProjectDetailsDTO
            {
                Project = new ProjectsDTO
                {
                    Id = project.Id,
                    Name = project.Name,
                    Description = project.Description,
                    CreatedAt = project.CreatedAt,
                    CreatedBy = project.TbProjectMembers.FirstOrDefault(m => m.Role.ToLower() == "project manager")?.User?.FirstName + " " +
                                project.TbProjectMembers.FirstOrDefault(m => m.Role.ToLower() == "project manager")?.User?.LastName,
                    TotalTasks = project.TbTasks.Count,
                    TotalMembers = project.TbProjectMembers.Count
                },

                //Members = project.TbProjectMembers.Select(m => new ProjectMemberDTO
                //{
                //    UserId = m.UserId,
                //    Role = m.Role,
                //    UserName = m.User?.FirstName + " " + m.User?.LastName
                //}).ToList(),

                //Tasks = project.TbTasks.Select(t => new TasksDTO
                //{
                //    Id = t.Id,
                //    Title = t.Title,
                //    Status = t.Status,
                //    Priority = t.Priority,
                //    AssignedUserName = t.AssignedToNavigation != null
                //        ? t.AssignedToNavigation.FirstName + " " + t.AssignedToNavigation.LastName
                //        : null,
                //    DueDate = t.DueDate
                //}).ToList()
            };

            result.Data = dto;
            result.StatusCode = "200";

            return result;
        }

        public ApiResponse Create(CreateProjectDTO dto)
        {
            var result = new ApiResponse();

            var user = _httpContext.HttpContext.User;
            var userId = _httpContext.HttpContext.User
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = user.FindFirst(ClaimTypes.Name)?.Value;

            if (user.IsInRole("Team Member"))
            {
                result.Errors.Add(new { Message = "You don't have permission to create a project" });
                result.StatusCode = "403";
                return result;
            }

            // check if the project manger status is rejected

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                result.Errors.Add(new { Name = "Name is required" });
                result.StatusCode = "400";
                return result;
            }

            var project = new TbProject
            {
                Name = dto.Name,
                Description = dto.Description,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };

            _repo.Add(project);
            _repo.Save();

            // save in project member table as project manager
            var projectMember = new TbProjectMember
            {
                ProjectId = project.Id,
                UserId = userId,
                Role = "Project Manager",
                JoinedAt = DateTime.UtcNow
            };
            _projectMemberRepo.Add(projectMember);
            _projectMemberRepo.Save();

            

            // save in TbActivity log
            _activity.Log(
                userId,
                "created",
                "Project",
                project.Id.ToString(),
                $"{userName} created project {project.Name}"
            );

            result.Data = new ProjectsDTO
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                CreatedBy = userName,
                CreatedAt = project.CreatedAt
            };
            result.StatusCode = "201";

            return result;
        }

        public ApiResponse Update(UpdateProjectDTO dto)
        {
            var project = _repo.GetById(dto.Id);

            var result = new ApiResponse();

            if (project == null)
            {
                result.Errors.Add(new
                {
                    Field = "Id",
                    Message = "Project not found"
                });
                result.StatusCode = "404";
                return result;
            }

            if (dto.Name != null)
                project.Name = dto.Name;

            if (dto.Description != null)
                project.Description = dto.Description;

            _repo.Update(project);
            _repo.Save();

            return new ApiResponse
            {
                Data = new ProjectsDTO
                {
                    Id = project.Id,
                    Name = project.Name,
                    Description = project.Description,
                    CreatedBy = project.CreatedBy,
                    CreatedAt = project.CreatedAt
                },
                StatusCode = "200"
            };
        }

        public ApiResponse Delete(int id)
        {
            var project = _repo.GetById(id);

            if (project == null)
                return new ApiResponse { StatusCode = "404" };

            _repo.Delete(project);
            _repo.Save();

            var user = _httpContext.HttpContext.User;
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = user.FindFirst(ClaimTypes.Name)?.Value;

            // save in TbActivity log
            _activity.Log(
                userId,
                "deleted",
                "Project",
                project.Id.ToString(),
                $"{userName} deleted project {project.Name}"
            );

            return new ApiResponse
            {
                StatusCode = "200"
            };
        }

        public ApiResponse GetProjectProgress(int projectId)
    
        {
            var response = new ApiResponse();

            var project =_repo.GetProjectWithTasks(projectId);

            if (project == null)
            {
                response.StatusCode = "404";
                response.Errors.Add("Project not found");
                return response;
            }

            if (project.TbTasks == null ||
                project.TbTasks.Count == 0)
            {
                response.Data = new
                {
                    Planned = 0,
                    Actual = 0
                };

                response.StatusCode = "200";
                return response;
            }

            // =========================
            // Actual Progress
            // =========================

            double totalWeight = 0;

            foreach (var task in project.TbTasks)
            {
                var status = task.Status?.ToLower();

                switch (status)
                {
                    case "todo":
                        totalWeight += 0;
                        break;

                    case "in_progress":
                        totalWeight += 0.5;
                        break;

                    case "done":
                        totalWeight += 1;
                        break;
                }
            }

            var actual =(totalWeight /project.TbTasks.Count) * 100;

            // =========================
            // Planned Progress
            // (بدون DueDate)
            // =========================

            // هنا نحسب planned = عدد التاسكات المتوقع إنها تخلص

            var completedTasks =project.TbTasks.Count(t =>t.Status.ToLower() == "done");

            var planned =(completedTasks /(double)project.TbTasks.Count) * 100;

            response.Data = new
            {
                Planned = Math.Round(planned, 0),
                Actual = Math.Round(actual, 0)
            };

            response.StatusCode = "200";

            return response;
        }

        public ApiResponse GetTeamWorkload(int projectId)
        {
            var response = new ApiResponse();

            var tasks =
                _repo.GetTasksWithUsers(projectId);

            if (tasks == null || tasks.Count == 0)
            {
                response.StatusCode = "404";
                response.Errors.Add("No tasks found");
                return response;
            }

            var result =tasks.GroupBy(t => t.AssignedTo).Select(group =>
                {
                    var completed =group.Count(t =>t.Status?.ToLower() == "done");

                    var remaining =group.Count(t =>t.Status?.ToLower() == "todo" || t.Status?.ToLower() == "in_progress");

                    var overdue =group.Count(t =>t.DueDate < DateTime.UtcNow && t.Status?.ToLower() != "done");

                    return new TeamWorkloadDTO
                    {
                        UserName =group.First().AssignedToNavigation?.UserName,

                        Completed = completed,

                        Remaining = remaining,

                        Overdue = overdue
                    };
                })
                .ToList();

            response.Data = result;
            response.StatusCode = "200";

            return response;
        }
    }
}
