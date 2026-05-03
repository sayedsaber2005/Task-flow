using ProjectManagement.Models;
using ProjectManagement.Repositories.Interfaces;

namespace ProjectManagement.Repositories.Implementations
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ProjectManagementContext _context;

        public NotificationRepository(ProjectManagementContext context)
        {
            _context = context;
        }

        public void Add(TbNotification notification)
        {
            _context.TbNotifications.Add(notification);
        }

        public IEnumerable<TbNotification> GetByUserId(string userId)
        {
            return _context.TbNotifications
                .Where(n => n.UserId == userId)
                .ToList();
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
