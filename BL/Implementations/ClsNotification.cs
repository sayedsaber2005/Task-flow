using Microsoft.AspNetCore.SignalR;
using ProjectManagement.BL.Interfaces;
using ProjectManagement.DTOs;
using ProjectManagement.DTOs.Notifications;
using ProjectManagement.Hubs;
using ProjectManagement.Models;
using ProjectManagement.Repositories.Implementations;
using ProjectManagement.Repositories.Interfaces;
using System.Security.Claims;

namespace ProjectManagement.BL.Implementations
{
    public class ClsNotification : INotification
    {
        private readonly INotificationRepository _repo;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IHubContext<NotificationHub> _hubContext;

        public ClsNotification(INotificationRepository repo, IHttpContextAccessor httpContext, IHubContext<NotificationHub> hubContext)
        {
            _repo = repo;
            _httpContext = httpContext;
            _hubContext = hubContext;
        }

        //public ApiResponse GetMyNotifications()
        //{
        //    var userId = _httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        //    var notifications = _repo.GetByUserId(userId)
        //        .OrderByDescending(n => n.CreatedAt)
        //        .Select(n => new NotificationDTO
        //        {
        //            Id = n.Id,
        //            Message = n.Message,
        //            Type = n.Type,
        //            CreatedAt = n.CreatedAt
        //        });

        //    return new ApiResponse
        //    {
        //        Data = notifications,
        //        StatusCode = "200"
        //    };
        //}


        public async Task SendNotificationAsync(CreateNotificationDTO dto)
        {
            var notification = new TbNotification
            {
                UserId = dto.UserId,
                Message = dto.Message,
                Type = dto.Type,
                CreatedAt = DateTime.UtcNow
            };

            await _repo.AddAsync(notification);

            await _repo.SaveChangesAsync();

            var NotificationDTO = new NotificationDTO
            {
                Id = notification.Id,
                Message = notification.Message,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt,
                Type = notification.Type,
            };

            // REALTIME SEND
            await _hubContext.Clients
                .Group($"user-{dto.UserId}")
                .SendAsync("ReceiveNotification", NotificationDTO);
        }

        public async Task<ApiResponse> GetUserNotificationsAsync(string userId)
        {
            var notifications = await _repo
                .GetUserNotificationsAsync(userId);

            var data = notifications.Select(n => new NotificationDTO
            {
                Id = n.Id,
                Message = n.Message,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt,
                Type = n.Type
            }).ToList();

            return new ApiResponse
            {
                Data = data,
                StatusCode = "200"
            };
        }

        public async Task<ApiResponse> MarkAsReadAsync(int notificationId)
        {
            var notification = await _repo
                .GetByIdAsync(notificationId);

            if (notification == null)
                return new ApiResponse
                {
                    Data = null,
                    StatusCode = "404",
                    Errors = new List<object> { "Notification not found" }
                };

            notification.IsRead = true;

            await _repo.SaveChangesAsync();

            return new ApiResponse
            {
                StatusCode = "200",
                Data = "Notification marked as read successfully"
            };
        }

        public async Task<ApiResponse> GetUnreadCountAsync(string userId)
        {
            var count = await _repo
                .GetUnreadCountAsync(userId);

            return new ApiResponse
            {
                Data = count,
                StatusCode = "200"
            };
        }
    }
}
