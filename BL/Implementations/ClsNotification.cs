using ProjectManagement.BL.Interfaces;
using ProjectManagement.DTOs;
using ProjectManagement.DTOs.Notifications;
using ProjectManagement.Repositories.Interfaces;
using System.Security.Claims;

namespace ProjectManagement.BL.Implementations
{
    public class ClsNotification : INotification
    {
        private readonly INotificationRepository _repo;
        private readonly IHttpContextAccessor _httpContext;

        public ClsNotification(INotificationRepository repo, IHttpContextAccessor httpContext)
        {
            _repo = repo;
            _httpContext = httpContext;
        }

        public ApiResponse GetMyNotifications()
        {
            var userId = _httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var notifications = _repo.GetByUserId(userId)
                .OrderByDescending(n => n.CreatedAt)
                .Select(n => new NotificationDTO
                {
                    Id = n.Id,
                    Message = n.Message,
                    Type = n.Type,
                    CreatedAt = n.CreatedAt
                });

            return new ApiResponse
            {
                Data = notifications,
                StatusCode = "200"
            };
        }
    }
}
