using ProjectManagement.Models;

namespace ProjectManagement.Repositories.Interfaces
{
    public interface IProjectRepository : IGenericRepository<TbProject>
    {
        //List<TbProject> GetAll();
        //TbProject GetById(int id);
        TbProject GetProjectWithDetails(int projectId);
        //void Add(TbProject project);
        //void Update(TbProject project);
        //void Delete(TbProject project);
        //void Save();
        TbProject GetProjectWithTasks(int projectId);
        List<TbTask> GetTasksWithUsers(int projectId);
    }
}
