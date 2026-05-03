using ProjectManagement.Models;

namespace ProjectManagement.Repositories.Interfaces
{
    public interface IAttachmentRepository
    {
        List<TbAttachment> GetByTaskId(int taskId);
        TbAttachment GetById(int id);
        void Add(TbAttachment attachment);
        void Delete(TbAttachment attachment);
        void Save();
    }
}
