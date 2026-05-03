using ProjectManagement.DTOs;
using ProjectManagement.DTOs.TaskHistory;

namespace ProjectManagement.BL.Interfaces
{
    public interface ITaskHistory
    {
        public ApiResponse GetHistoryByTaskId(int taskId);
        public ApiResponse AddHistory(CreateTaskHistoryDTO history);
    }
}
