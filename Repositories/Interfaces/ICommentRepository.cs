using ProjectManagement.Models;

namespace ProjectManagement.Repositories.Interfaces
{
    public interface ICommentRepository
    {
        List<TbComment> GetByTaskId(int taskId);
        void Add(TbComment comment);
        TbComment GetById(int id);
        void Delete(TbComment comment);
        void Save();
    }
}
