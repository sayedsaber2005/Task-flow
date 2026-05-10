using ProjectManagement.DTOs;
using ProjectManagement.DTOs.Notifications;

namespace ProjectManagement.BL.Interfaces
{
    public interface INotification
    {
        Task SendNotificationAsync(CreateNotificationDTO dto);

        Task<ApiResponse> GetUserNotificationsAsync(string userId);

        Task<ApiResponse> MarkAsReadAsync(int notificationId);

        Task<ApiResponse> GetUnreadCountAsync(string userId);
    }
}
