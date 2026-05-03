using ProjectManagement.DTOs.Tasks;
using ProjectManagement.Models;

namespace ProjectManagement.Repositories.Interfaces
{
    public interface ITaskHistoryRepository
    {
        void AddHistory(TbTaskHistory history);
        List<TbTaskHistory> GetHistoryByTaskId(int taskId);
        void SaveHistory();
    }
}
