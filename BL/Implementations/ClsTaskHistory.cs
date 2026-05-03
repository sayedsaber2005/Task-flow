using ProjectManagement.BL.Interfaces;
using ProjectManagement.DTOs;
using ProjectManagement.DTOs.TaskHistory;
using ProjectManagement.Models;
using ProjectManagement.Repositories.Interfaces;

namespace ProjectManagement.BL.Implementations
{
    public class ClsTaskHistory : ITaskHistory
    {
        private readonly ITaskHistoryRepository _repo;

        public ClsTaskHistory(ITaskHistoryRepository repo)
        {
            _repo = repo;
        }

        public ApiResponse AddHistory(CreateTaskHistoryDTO history)
        {
            var response = new ApiResponse();

            // validations
            if (history.TaskId <= 0)
                response.Errors.Add(new { TaskId = "Invalid TaskId" });

            if (string.IsNullOrWhiteSpace(history.FieldChanged))
                response.Errors.Add(new { FieldName = "Field name is required" });

            if (response.Errors.Any())
            {
                response.StatusCode = "400";
                return response;
            }

            var taskHistory = new TbTaskHistory
            {
                TaskId = history.TaskId,
                FieldChanged = history.FieldChanged,
                OldValue = history.OldValue,
                NewValue = history.NewValue,
                ChangedBy = history.ChangedBy,
                ChangedAt = DateTime.UtcNow
            };

            _repo.AddHistory(taskHistory);
            _repo.SaveHistory();

            response.Data = new TaskHistoryDTO
            {
                TaskId = taskHistory.TaskId,
                FieldChanged = taskHistory.FieldChanged,
                OldValue = taskHistory.OldValue,
                NewValue = taskHistory.NewValue,
                ChangedAt = taskHistory.ChangedAt
            };
            response.StatusCode = "201"; // Created

            return response;
        }

        public ApiResponse GetHistoryByTaskId(int taskId)
        {
            var history = _repo.GetHistoryByTaskId(taskId);
            var response = new ApiResponse
            {
                Data = new List<TaskHistoryDTO>(history.Select(h => new TaskHistoryDTO
                {
                    Id = h.Id,
                    TaskId = h.TaskId,
                    FieldChanged = h.FieldChanged,
                    OldValue = h.OldValue,
                    NewValue = h.NewValue,
                    ChangedAt = h.ChangedAt
                })),
                StatusCode = "200" // OK
            };

            return response;
        }
    }
}
