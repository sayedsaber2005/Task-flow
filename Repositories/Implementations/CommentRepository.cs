using Microsoft.EntityFrameworkCore;
using ProjectManagement.Models;
using ProjectManagement.Repositories.Interfaces;

namespace ProjectManagement.Repositories.Implementations
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ProjectManagementContext _context;

        public CommentRepository(ProjectManagementContext context)
        {
            _context = context;
        }

        public List<TbComment> GetByTaskId(int taskId)
        {
            return _context.TbComments
                .Include(c => c.User)
                .Where(c => c.TaskId == taskId)
                .ToList();
        }

        public TbComment GetById(int id)
        {
            return _context.TbComments.FirstOrDefault(c => c.Id == id);
        }

        public void Add(TbComment comment)
        {
            _context.TbComments.Add(comment);
        }

        public void Delete(TbComment comment)
        {
            _context.TbComments.Remove(comment);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
