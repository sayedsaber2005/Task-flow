using ProjectManagement.Models;

namespace ProjectManagement.Repositories.Interfaces
{
    public interface INotificationRepository
    {
        IEnumerable<TbNotification> GetByUserId(string userId);
        void Add(TbNotification notification);
        void Save();
    }
}
