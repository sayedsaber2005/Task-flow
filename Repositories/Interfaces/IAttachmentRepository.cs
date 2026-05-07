using ProjectManagement.Models;

namespace ProjectManagement.Repositories.Interfaces
{
    public interface IAttachmentRepository : IGenericRepository<TbAttachment>
    {
        List<TbAttachment> GetByTaskId(int taskId);
        //TbAttachment GetById(int id);
        //void Add(TbAttachment attachment);
        //void Delete(TbAttachment attachment);
        //void Save();
    }
}
