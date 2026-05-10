using ProjectManagement.Models;

namespace ProjectManagement.Repositories.Interfaces
{
    public interface INotificationRepository
    {
        Task AddAsync(TbNotification notification);

        Task<List<TbNotification>> GetUserNotificationsAsync(string userId);

        Task<TbNotification?> GetByIdAsync(int id);

        Task<int> GetUnreadCountAsync(string userId);

        Task SaveChangesAsync();
    }
}
