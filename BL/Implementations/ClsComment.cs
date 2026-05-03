using Microsoft.AspNetCore.SignalR;
using ProjectManagement.BL.Interfaces;
using ProjectManagement.DTOs;
using ProjectManagement.DTOs.Comments;
using ProjectManagement.Hubs;
using ProjectManagement.Models;
using ProjectManagement.Repositories.Interfaces;
using System.Security.Claims;

namespace ProjectManagement.BL.Implementations
{
    public class ClsComment : IComment
    {
        private readonly ICommentRepository _repo;
        private readonly IHubContext<NotificationHub> _hub;
        private readonly INotificationRepository _notificationRepo;
        private readonly IHttpContextAccessor _httpContext;
        private readonly ITaskRepository _taskRepo;
        private readonly IProjectMemberRepository _projectMemberRepo;

        public ClsComment(ICommentRepository repo, 
            IHubContext<NotificationHub> hub, 
            INotificationRepository notificationRepo,
            IHttpContextAccessor httpContext,
            ITaskRepository taskRepo,
            IProjectMemberRepository projectMemberRepo)
        {
            _repo = repo;
            _hub = hub;
            _notificationRepo = notificationRepo;
            _httpContext = httpContext;
            _projectMemberRepo = projectMemberRepo;
            _taskRepo = taskRepo;
        }

        public ApiResponse GetByTaskId(int taskId)
        {
            var comments = _repo.GetByTaskId(taskId);

            var result = comments.Select(c => new CommentDTO
            {
                Id = c.Id,
                Content = c.Content,
                UserName = c.User?.UserName,
                CreatedAt = c.CreatedAt
            });

            return new ApiResponse
            {
                Data = result,
                StatusCode = "200"
            };
        }

        public async Task<ApiResponse> AddCommentAsync(int taskId, string userId, CreateCommentDTO dto)
        {
            var result = new ApiResponse();

            try
            {
                //var CurrUserId = _httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                // check task exists
                var task = _taskRepo.GetById(taskId);
                if (task == null)

                {
                    result.Errors.Add(new { Message = "Task not found" });
                    result.StatusCode = "404";
                    return result;
                }

                // create comment
                var comment = new TbComment
                {
                    TaskId = taskId,
                    Content = dto.Content,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };

                _repo.Add(comment);
                _repo.Save();

                // Build notification
                var message = $"{userId} commented on task '{task.Title}'";

                var members = _projectMemberRepo.GetByProjectId((int)task.ProjectId);

                foreach (var member in members)
                {
                    if (member.UserId == userId) continue;

                    var notification = new TbNotification
                    {
                        UserId = member.UserId,
                        Message = message,
                        Type = NotificationTypes.Comment,
                        CreatedAt = DateTime.UtcNow
                    };

                    _notificationRepo.Add(notification);

                    await _hub.Clients.User(member.UserId)
                        .SendAsync("ReceiveNotification", new
                        {
                            Message = message,
                            Type = "comment",
                            TaskId = task.Id
                        });
                }

                _notificationRepo.Save();

                result.Data = new CommentDTO
                {
                    Id = comment.Id,
                    Content = dto.Content,
                    UserName = comment.User?.UserName,
                    CreatedAt = comment.CreatedAt
                };
                result.StatusCode = "201";

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

        public ApiResponse DeleteComment(int commentId, string userId, string role)
        {
            var result = new ApiResponse();

            var comment = _repo.GetById(commentId);

            if (comment == null)
            {
                result.Errors.Add(new { Message = "Comment not found" });
                result.StatusCode = "404";
                return result;
            }

            // 🔥 authorization
            if (role != "Admin" && comment.UserId != userId)
            {
                result.Errors.Add(new { Message = "Not allowed" });
                result.StatusCode = "403";
                return result;
            }

            _repo.Delete(comment);
            _repo.Save();

            result.StatusCode = "200";
            return result;
        }
    }
}
