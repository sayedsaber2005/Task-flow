using ProjectManagement.Models;
using ProjectManagement.Repositories.Interfaces;

namespace ProjectManagement.Repositories.Implementations
{
    public class ActivityRepository : IActivityRepository
    {
        private readonly ProjectManagementContext _context;

        public ActivityRepository(ProjectManagementContext context)
        {
            _context = context;
        }

        public void Add(TbActivityLog log)
        {
            _context.TbActivityLogs.Add(log);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public List<TbActivityLog> GetRecent(int count)
        {
            return _context.TbActivityLogs
                .OrderByDescending(x => x.CreatedAt)
                .Take(count)
                .ToList();
        }

        public List<TbActivityLog> GetByUser(string userId)
        {
            return _context.TbActivityLogs
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();
        }
    }
}
