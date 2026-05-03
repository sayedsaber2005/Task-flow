using ProjectManagement.Models;
using ProjectManagement.Repositories.Interfaces;

namespace ProjectManagement.Repositories.Implementations
{
    public class AttachmentRepository : IAttachmentRepository
    {
        private readonly ProjectManagementContext _context;

        public AttachmentRepository(ProjectManagementContext context)
        {
            _context = context;
        }

        public List<TbAttachment> GetByTaskId(int taskId)
        {
            return _context.TbAttachments
                .Where(a => a.TaskId == taskId)
                .ToList();
        }

        public TbAttachment GetById(int id)
        {
            return _context.TbAttachments.FirstOrDefault(a => a.Id == id);
        }

        public void Add(TbAttachment attachment)
        {
            _context.TbAttachments.Add(attachment);
        }

        public void Delete(TbAttachment attachment)
        {
            _context.TbAttachments.Remove(attachment);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

    }
}
