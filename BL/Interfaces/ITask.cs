using ProjectManagement.DTOs;
using ProjectManagement.DTOs.Tasks;

namespace ProjectManagement.BL.Interfaces
{
    public interface ITask
    {
        public ApiResponse GetAllTasks(TaskQueryDTO query);
        public ApiResponse GetTaskById(int id);
        public List<TasksDTO> GetTasksByProjectId(int id);
        public Task<ApiResponse> CreateTaskAsync(CreateTaskDTO dto);
        public Task<ApiResponse> UpdateTaskAsync(UpdateTaskDTO dto);
        public ApiResponse DeleteTask(int id);
        public Task<ApiResponse> AssignTaskAsync(AssignTaskDTO dto);
        public Task<ApiResponse> ChangePriorityAsync(ChangePriorityDTO dto);
        public ApiResponse GetMyTasks();
    }
}
