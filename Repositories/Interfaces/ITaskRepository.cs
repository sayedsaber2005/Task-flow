using ProjectManagement.DTOs.Tasks;
using ProjectManagement.Models;

namespace ProjectManagement.Repositories.Interfaces
{
    public interface ITaskRepository
    {
        List<TbTask> GetAll(TaskQueryDTO query);
        TbTask GetById(int id);
        public List<TbTask> GetByProjectId(int id);
        List<TbTask> GetByUser(string userId);
        List<TbTask> GetTasksByUserId(string userId);
        void Add(TbTask task);
        void Update(TbTask task);
        void Delete(TbTask task);
        void Save();
    }
}
