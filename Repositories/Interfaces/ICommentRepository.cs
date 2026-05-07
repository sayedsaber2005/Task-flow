using ProjectManagement.Models;

namespace ProjectManagement.Repositories.Interfaces
{
    public interface ICommentRepository : IGenericRepository<TbComment>
    {
        List<TbComment> GetByTaskId(int taskId);
        //void Add(TbComment comment);
        //TbComment GetById(int id);
        //void Delete(TbComment comment);
        //void Save();
    }
}
