using Microsoft.AspNetCore.SignalR;
using ProjectManagement.BL.Interfaces;
using ProjectManagement.DTOs;
using ProjectManagement.DTOs.TaskHistory;
using ProjectManagement.DTOs.Tasks;
using ProjectManagement.Hubs;
using ProjectManagement.Models;
using ProjectManagement.Repositories.Interfaces;
using System.Security.Claims;

namespace ProjectManagement.BL.Implementations
{
    public class ClsTask : ITask
    {
        private readonly ITaskRepository _repo;
        private readonly ITaskHistory _history;
        private readonly IProjectMemberRepository _projectMemberRepo;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IHubContext<NotificationHub> _hub;
        private readonly INotificationRepository _notificationRepo;
        private readonly IActivity _activity;
        private readonly IUserRepository _userRepo;
        public ClsTask(ITaskRepository repo, ITaskHistory history,
            IHttpContextAccessor httpContext,
            IProjectMemberRepository projectMemberRepo,
            IHubContext<NotificationHub> hub,
            INotificationRepository notificationRepo,
            IActivity activity)
        {
            _repo = repo;
            _history = history;
            _httpContext = httpContext;
            _projectMemberRepo = projectMemberRepo;
            _hub = hub;
            _notificationRepo = notificationRepo;
            _activity = activity;
        }

        public ApiResponse GetAllTasks(TaskQueryDTO query)
        {
            var result = new ApiResponse();
            try
            {
                var tasks = _repo.GetAll(query);

                result.Data = tasks.Select(t => new TasksDTO
                {
                    Id = t.Id,
                    Title = t.Title,
                    Status = t.Status,
                    Priority = t.Priority,
                    AssignedUserName = t.AssignedToNavigation?.UserName,
                    DueDate = t.DueDate,
                    CreatedAt = t.CreatedAt,
                    CreatedBy = t.CreatedBy,
                }).ToList();

                result.StatusCode = "200";

                return result;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Errors.Add(new { Exception = ex.Message });
                result.StatusCode = "500";
                return result;
            }

        }

        public ApiResponse GetTaskById(int id)
        {
            var result = new ApiResponse();
            try
            {
                var task = _repo.GetById(id);

                if (task == null)
                {
                    result.Data = null;
                    result.Errors.Add(new { Message = "Task not found" });
                    result.StatusCode = "404";
                    return result;
                }

                result.Data = new TasksDTO
                {
                    Id = task.Id,
                    Title = task.Title,
                    Status = task.Status,
                    Description=task.Description,
                    Priority = task.Priority,
                    AssignedUserName = task.AssignedToNavigation?.UserName,
                    DueDate = task.DueDate
                };

                result.StatusCode = "200";
                return result;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Errors.Add(new { Exception = ex.Message });
                result.StatusCode = "500";
                return result;
            }

        }

        public List<TasksDTO> GetTasksByProjectId(int id)
        {
            try
            {
                var tasks = _repo.GetByProjectId(id);

                return tasks.Select(t => new TasksDTO
                {
                    Id = t.Id,
                    Title = t.Title,
                    Status = t.Status,
                    Priority = t.Priority,
                    AssignedUserName = t.AssignedToNavigation?.UserName,
                    DueDate = t.DueDate
                }).ToList();
            }
            catch (Exception ex)
            {
                return new List<TasksDTO>();
            }
        }

        public async Task<ApiResponse> CreateTaskAsync(CreateTaskDTO dto)
        {
            var result = new ApiResponse();
            try
            {
                // validations

                var user = _httpContext.HttpContext.User;
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
                var userName = user.FindFirst(ClaimTypes.Name)?.Value;

                // a team member cannot create a task
                if (user.IsInRole("TeamMember"))
                {
                    result.Errors.Add(new
                    {
                        Field = "User",
                        Message = "You are not allowed to create tasks"
                    });
                    result.StatusCode = "403";
                    return result;
                }

                // a project manager can only create tasks in their projects
                if(user.IsInRole("ProjectManager"))
                {
                    var isMember = _projectMemberRepo.GetByUserAndProject(userId, dto.ProjectId);
                    if (isMember == null)
                    {
                        result.Errors.Add(new
                        {
                            Field = "User",
                            Message = "You can only create tasks in your projects"
                        });
                        result.StatusCode = "403";
                        return result;
                    }
                }

                // Title required
                if (string.IsNullOrWhiteSpace(dto.Title))
                {
                    result.Errors.Add(new
                    {
                        Field = "Title",
                        Message = "Title is required"
                    });
                }

                // DueDate must be in the future
                if (dto.DueDate < DateTime.UtcNow)
                {
                    result.Errors.Add(new
                    {
                        Field = "DueDate",
                        Message = "Due date must be in the future"
                    });
                }

                // Priority check
                var allowedPriorities = new[] { "low", "medium", "high" };
                if (string.IsNullOrWhiteSpace(dto.Priority))
                {
                    result.Errors.Add(new
                    {
                        Field = "Priority",
                        Message = "Priority is required. Allowed values: low, medium, high"
                    });
                }
                else if (!allowedPriorities.Contains(dto.Priority.ToLower()))
                {
                    result.Errors.Add(new
                    {
                        Field = "Priority",
                        Message = "Invalid priority. Allowed values: low, medium, high"
                    });
                }

                // Project must exist
                var projectTasks = _repo.GetByProjectId(dto.ProjectId);
                if (projectTasks == null)
                {
                    result.Errors.Add(new
                    {
                        Field = "ProjectId",
                        Message = "Project not found"
                    });
                }

                if (result.Errors.Count > 0)
                {
                    result.Data = null;
                    result.StatusCode = "400";
                    return result;
                }

                // logic
                var task = new TbTask
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    ProjectId = dto.ProjectId,
                    DueDate = dto.DueDate,
                    Priority = dto.Priority,
                    Status = "todo",
                    CreatedBy = userId,
                    CreatedAt = DateTime.UtcNow
                };

                foreach (var claim in user.Claims)
                {
                    Console.WriteLine($"{claim.Type} : {claim.Value}");
                }

                _repo.Add(task);
                _repo.Save();

                

                // save in TbActivity log
                _activity.Log(userId,"created","Task",task.Id.ToString(),$"{userName} created task {task.Title}");

                result.Data = new TasksDTO
                {
                    Id = task.Id,
                    Title = task.Title,
                    Status = task.Status,
                    Priority = task.Priority,
                    DueDate = task.DueDate,
                    CreatedAt = task.CreatedAt,
                    CreatedBy = userName
                };
                result.StatusCode = "201";
                return result;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Errors.Add(new { Exception = ex.Message });
                result.StatusCode = "500";
                return result;
            }
        }

        private void ValidateTaskExists(TbTask task, ApiResponse result)
        {
            if (task == null)
            {
                result.Errors.Add(new
                {
                    Field = "Id",
                    Message = "Task not found"
                });
                result.StatusCode = "404";
            }
        }

        private void ValidatePermissions(ClaimsPrincipal user, string userId, TbTask task, ApiResponse result)
        {
            // TeamMember → فقط assigned task
            if (user.IsInRole("Team Member") && task.AssignedTo != userId)
            {
                result.Errors.Add(new
                {
                    Field = "User",
                    Message = "You can only update tasks assigned to you"
                });
                result.StatusCode = "403";
                return;
            }

            // ProjectManager → لازم يكون عضو في المشروع
            if (user.IsInRole("ProjectManager"))
            {
                var isMember = _projectMemberRepo.GetByUserAndProject(userId, (int)task.ProjectId);

                if (isMember == null)
                {
                    result.Errors.Add(new
                    {
                        Field = "User",
                        Message = "You can only update tasks in your projects"
                    });
                    result.StatusCode = "403";
                }
            }
        }

        private void ValidateBusinessRules(ClaimsPrincipal user, UpdateTaskDTO dto, TbTask task, ApiResponse result)
        {
            // Title
            if (dto.Title != null && string.IsNullOrWhiteSpace(dto.Title))
            {
                result.Errors.Add(new { Field = "Title", Message = "Title cannot be empty" });
            }

            // Priority
            if (dto.Priority != null && dto.Priority != task.Priority)
            {
                if (user.IsInRole("Team Member") && user.IsInRole("Admin"))
                {
                    result.Errors.Add(new { Field = "Priority", Message = "Not allowed" });
                    return;
                }

                if (!IsValidPriority(dto.Priority))
                {
                    result.Errors.Add(new { Field = "Priority", Message = "Invalid priority" });
                }
            }

            // DueDate
            if (dto.DueDate.HasValue)
            {
                if (user.IsInRole("Team Member") && dto.DueDate != task.DueDate)
                {
                    result.Errors.Add(new { Field = "DueDate", Message = "Not allowed" });
                }

                if (dto.DueDate < DateTime.UtcNow)
                {
                    result.Errors.Add(new { Field = "DueDate", Message = "Must be future" });
                }
            }

            // Status
            if (!string.IsNullOrEmpty(dto.Status) && dto.Status != task.Status)
            {
                if (!IsValidStatusTransition(task.Status, dto.Status))
                {
                    result.Errors.Add(new
                    {
                        Field = "Status",
                        Message = $"Invalid transition from {task.Status} to {dto.Status}"
                    });
                }
            }
        }

        private List<CreateTaskHistoryDTO> BuildHistory(TbTask task, UpdateTaskDTO dto, string userId)
        {
            var history = new List<CreateTaskHistoryDTO>();

            void Add(string field, string oldVal, string newVal)
            {
                history.Add(new CreateTaskHistoryDTO
                {
                    TaskId = task.Id,
                    FieldChanged = field,
                    OldValue = oldVal,
                    NewValue = newVal,
                    ChangedBy = userId
                });
            }

            if (dto.Priority != null && dto.Priority != task.Priority)
                Add("priority", task.Priority, dto.Priority);

            if (dto.Status != null && dto.Status != task.Status)
                Add("status", task.Status, dto.Status);

            if (dto.Title != null && dto.Title != task.Title)
                Add("title", task.Title, dto.Title);

            return history;
        }

        private void ApplyUpdates(TbTask task, UpdateTaskDTO dto)
        {
            if (dto.Title != null) task.Title = dto.Title;
            if (dto.Description != null) task.Description = dto.Description;
            if (dto.Status != null) task.Status = dto.Status;
            if (dto.Priority != null) task.Priority = dto.Priority;
            if (dto.DueDate.HasValue) task.DueDate = dto.DueDate;

            task.UpdatedAt = DateTime.UtcNow;
        }

        private void SaveHistory(List<CreateTaskHistoryDTO> changes)
        {
            foreach (var change in changes)
            {
                _history.AddHistory(change);
            }
        }

        private bool IsValidPriority(string p)
        {
            return new[] { "low", "medium", "high" }.Contains(p.ToLower());
        }

        private bool IsValidStatusTransition(string current, string next)
        {
            var transitions = new Dictionary<string, string[]>
            {
                { "todo", new[] { "in_progress" } },
                { "in_progress", new[] { "done" } },
                { "done", new string[] { } }
            };

            return transitions.ContainsKey(current.ToLower()) &&
                   transitions[current.ToLower()].Contains(next.ToLower());
        }

        private TasksDTO MapToDTO(TbTask task)
        {
            return new TasksDTO
            {
                Id = task.Id,
                Title = task.Title,
                Status = task.Status,
                Priority = task.Priority,
                AssignedUserName = task.AssignedToNavigation?.UserName,
                DueDate = task.DueDate
            };
        }

        private TbNotification BuildNotification(TbTask task, string message)
        {
            return new TbNotification
            {
                UserId = task.AssignedTo,
                Message = message,
                Type = "task",
                CreatedAt = DateTime.UtcNow
            };
        }

        private async Task SendRealtimeNotification(string userId, string message, int taskId, int projectId)
        {
            //await _hub.Clients.User(userId)
            //    .SendAsync("ReceiveNotification", new
            //    {
            //        Message = message,
            //        TaskId = taskId
            //    });

            var members = _projectMemberRepo.GetByProjectId(projectId);

            foreach (var member in members)
            {
                if (member.UserId == userId) continue;

                var notification = new TbNotification
                {
                    UserId = member.UserId,
                    Message = message,
                    Type = NotificationTypes.Task,
                    CreatedAt = DateTime.UtcNow
                };

                _notificationRepo.Add(notification);

                await _hub.Clients.User(member.UserId)
                    .SendAsync("ReceiveNotification", new
                    {
                        Message = message,
                        Type = "task",
                        TaskId = taskId
                    });
            }

            _notificationRepo.Save();
        }

        private string BuildUpdateMessage(dynamic task, UpdateTaskDTO dto)
        {
            if (dto.Status != null && dto.Status != task.Status)
                return $"Task '{task.Title}' status updated to {dto.Status}";

            if (dto.Priority != null && dto.Priority != task.Priority)
                return $"Task '{task.Title}' priority changed to {dto.Priority}";

            return $"Task '{task.Title}' has been updated";
        }

        public async Task<ApiResponse> UpdateTaskAsync(UpdateTaskDTO dto)
        {
            var result = new ApiResponse();

            try
            {
                var task = _repo.GetById(dto.Id);

                ValidateTaskExists(task, result);
                if (result.Errors.Any()) return result;

                var user = _httpContext.HttpContext.User;

                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

                ValidatePermissions(user, userId, task, result);
                ValidateBusinessRules(user, dto, task, result);

                if (result.Errors.Any())
                {
                    result.StatusCode = "400";
                    return result;
                }

                var history = BuildHistory(task, dto, userId);

                var oldTask = new
                {
                    task.Status,
                    task.Priority,
                    task.Title
                };

                ApplyUpdates(task, dto);

                SaveHistory(history);

                _repo.Update(task);
                _repo.Save();

                // message ديناميك
                var message = BuildUpdateMessage(oldTask, dto);

                //// save notification in DB
                //var notification = BuildNotification(task, message);
                //_notificationRepo.Add(notification);
                //_notificationRepo.Save();

                // send real-time
                await SendRealtimeNotification(userId, message, task.Id, (int)task.ProjectId);

                result.Data = MapToDTO(task);
                result.StatusCode = "200";

                return result;
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Data = null,
                    Errors = new List<object> { new { Exception = ex.Message } },
                    StatusCode = "500"
                };
            }
        }

        public ApiResponse DeleteTask(int id)
        {
            var result = new ApiResponse();
            try
            {
                // Task must exist
                var task = _repo.GetById(id);
                ValidateTaskExists(task, result);
                if(result.Errors.Any()) return result;

                // Only Admin or PM can delete
                var user = _httpContext.HttpContext.User;
                var userId = _httpContext.HttpContext.User
                    .FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (user.IsInRole("TeamMember"))
                {
                    result.Errors.Add(new
                    {
                        Field = "User",
                        Message = "You are not allowed to delete tasks"
                    });
                    result.StatusCode = "403";
                    return result;
                }

                if(user.IsInRole("ProjectManager"))
                {
                    var isMember = _projectMemberRepo.GetByUserAndProject(userId, (int)task.ProjectId);
                    if (isMember == null)
                    {
                        result.Errors.Add(new
                        {
                            Field = "User",
                            Message = "You can only delete tasks in your projects"
                        });
                        result.StatusCode = "403";
                        return result;
                    }
                }

                _repo.Delete(task);
                _repo.Save();

                var userName = user.FindFirst(ClaimTypes.Name)?.Value;

                // save in TbActivity log
                _activity.Log(
                    userId,
                    "deleted",
                    "Task",
                    task.Id.ToString(),
                    // how to get the username here?
                    $"{userName} deleted task {task.Title}"
                );

                result.StatusCode = "200";

                return result;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Errors.Add(new { Exception = ex.Message });
                result.StatusCode = "500";
                return result;
            }
        }

        public async Task<ApiResponse> AssignTaskAsync(AssignTaskDTO dto)
        {
            var result = new ApiResponse();
            try
            {
                // check task exists
                var task = _repo.GetById(dto.TaskId);

                ValidateTaskExists(task, result);
                if (result.Errors.Any()) return result;

                var user = _httpContext.HttpContext.User;
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // check current user is member in project
                var currentMember = _projectMemberRepo
                    .GetByUserAndProject(userId, (int)task.ProjectId);

                if (currentMember == null)
                {
                    result.StatusCode = "403";
                    result.Errors.Add(new { Message = "Not part of project" });
                    return result;
                }

                // check permission (only PM or Admin)

                if (!user.IsInRole("Project Manager"))
                {
                    result.StatusCode = "403";
                    result.Errors.Add(new { Message = "Not allowed to assign tasks" });
                    return result;
                }

                // check assigned user is member
                var assignedMember = _projectMemberRepo
                    .GetByUserAndProject(dto.AssignedTo, (int)task.ProjectId);

                if (assignedMember == null)
                {
                    result.StatusCode = "400";
                    result.Errors.Add(new { Message = "User not in project" });
                    return result;
                }

                // cannot assign if task is done
                if (task.Status == "done")
                {
                    result.StatusCode = "400";
                    result.Errors.Add(new { Message = "Cannot assign task that is done" });
                    return result;
                }

                //  assign
                var oldAssigned = task.AssignedTo;
                task.AssignedTo = dto.AssignedTo;

                // auto move status
                if (task.Status == "todo")
                    task.Status = "in_progress";

                // add history
                var changes = new List<CreateTaskHistoryDTO>();
                changes.Add(new CreateTaskHistoryDTO
                {
                    TaskId = task.Id,
                    FieldChanged = "assigned_to",
                    OldValue = oldAssigned,
                    NewValue = dto.AssignedTo,
                    ChangedBy = userId
                });
                SaveHistory(changes);

                // save notification in DB
                var message = $"You have been assigned a new task: {task.Title}";

                var notification = BuildNotification(task, message);
                _notificationRepo.Add(notification);
                _notificationRepo.Save();

                await SendRealtimeNotification(dto.AssignedTo, message, task.Id, (int)task.ProjectId);

                _repo.Update(task);
                _repo.Save();

                var userName = user.FindFirst(ClaimTypes.Name)?.Value;

                // save in TbActivity log
                _activity.Log(
                    userId,
                    "assigned",
                    "Task",
                    task.Id.ToString(),
                    $"{userName} assigned task {task.Title} to {assignedMember.User.FirstName} {assignedMember.User.LastName}"
                );

                result.StatusCode = "200";
                result.Data = new
                {
                    TaskId = task.Id,
                    AssignedTo = dto.AssignedTo,
                    Status = task.Status
                };

                return result;
            } 
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Data = null,
                    Errors = new List<object> { new { Exception = ex.Message } },
                    StatusCode = "500"
                };
            }
        }

        public async Task<ApiResponse> ChangePriorityAsync(ChangePriorityDTO dto)
        {
            var result = new ApiResponse();

            try
            {
                var task = _repo.GetById(dto.TaskId);

                // Check exists
                ValidateTaskExists(task, result);
                if (result.Errors.Any())
                    return result;

                var user = _httpContext.HttpContext.User;

                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

                // Only PM or Admin
                if (!user.IsInRole("Project Manager") && !user.IsInRole("Admin"))
                {
                    result.StatusCode = "403";
                    result.Errors.Add(new{ Message = "Not allowed to change priority"});
                    return result;
                }

                // Validate Priority
                var allowed = new[] { "low", "medium", "high" };

                if (!allowed.Contains(dto.Priority.ToLower()))
                {
                    result.StatusCode = "400";result.Errors.Add( new{ Message = "Invalid priority"});
                    return result;
                }

                var oldPriority = task.Priority;
                task.Priority = dto.Priority;

                // Save History
                var history = new List<CreateTaskHistoryDTO>();

                history.Add(new CreateTaskHistoryDTO
                {
                    TaskId = task.Id,
                    FieldChanged = "priority",
                    OldValue = oldPriority,
                    NewValue = dto.Priority,
                    ChangedBy = userId
                });

                SaveHistory(history);

                _repo.Update(task);
                _repo.Save();

                // Activity log
                var userName = user.FindFirst(ClaimTypes.Name)?.Value;

                _activity.Log( userId,"updated","Task",task.Id.ToString(),$"{userName} changed priority of task {task.Title} to {dto.Priority}");

                result.StatusCode = "200";

                result.Data = new
                {
                    TaskId = task.Id,
                    Priority = task.Priority
                };

                return result;
            }
            catch (Exception ex)
            {
                return new ApiResponse
                {
                    Data = null,
                    Errors =
                        new List<object>
                        {
                            new { Exception = ex.Message }
                        },
                    StatusCode = "500"
                };
            }
        }

        public ApiResponse GetMyTasks()
        {
            var result = new ApiResponse();

            try
            {
                var user =
                    _httpContext.HttpContext.User;

                var userId =
                    user.FindFirst(
                        ClaimTypes.NameIdentifier)?.Value;

                var tasks =
                    _repo.GetTasksByUserId(userId);

                result.Data =
                    tasks.Select(t => new TasksDTO
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Status = t.Status,
                        Description = t.Description,
                        Priority = t.Priority,
                        AssignedUserName =
                            t.AssignedToNavigation?.UserName,
                        DueDate = t.DueDate,
                        CreatedAt = t.CreatedAt
                    }).ToList();

                result.StatusCode = "200";

                return result;
            }
            catch (Exception ex)
            {
                result.Data = null;

                result.Errors.Add(
                    new { Exception = ex.Message });

                result.StatusCode = "500";

                return result;
            }
        }
    }
}
