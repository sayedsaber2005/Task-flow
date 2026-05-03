using Microsoft.EntityFrameworkCore;
using ProjectManagement.Models;
using ProjectManagement.Repositories.Interfaces;

namespace ProjectManagement.Repositories.Implementations
{
    public class TaskHistoryRepository : ITaskHistoryRepository
    {
        private readonly ProjectManagementContext _context;

        public TaskHistoryRepository(ProjectManagementContext context)
        {
            _context = context;
        }

        public List<TbTaskHistory> GetHistoryByTaskId(int taskId)
        {
            return _context.TbTaskHistories
                .Include(h => h.ChangedByNavigation)
                .Where(h => h.TaskId == taskId)
                .OrderByDescending(h => h.ChangedAt)
                .ToList();
        }

        public void AddHistory(TbTaskHistory history)
        {
            _context.TbTaskHistories.Add(history);
        }

        public void SaveHistory()
        {
            _context.SaveChanges();
        }
    }
}
