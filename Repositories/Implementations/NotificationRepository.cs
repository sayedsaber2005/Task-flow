using ProjectManagement.Models;
using ProjectManagement.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ProjectManagement.Repositories.Implementations
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ProjectManagementContext _context;

        public NotificationRepository(ProjectManagementContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TbNotification notification)
        {
            await _context.TbNotifications.AddAsync(notification);
        }

        public async Task<List<TbNotification>> GetUserNotificationsAsync(string userId)
        {
            return await _context.TbNotifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<TbNotification?> GetByIdAsync(int id)
        {
            return await _context.TbNotifications
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            return await _context.TbNotifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

    }
}
