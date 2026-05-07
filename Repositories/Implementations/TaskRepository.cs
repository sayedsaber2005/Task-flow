using Microsoft.EntityFrameworkCore;
using ProjectManagement.BL.Interfaces;
using ProjectManagement.DTOs.Tasks;
using ProjectManagement.Models;
using ProjectManagement.Repositories.Interfaces;

namespace ProjectManagement.Repositories.Implementations
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ProjectManagementContext _context;
        private readonly IProjectRepository _projectRepo;

        public TaskRepository(ProjectManagementContext context, IProjectRepository projectRepo)
        {
            _context = context;
            _projectRepo = projectRepo;
        }

        public IEnumerable<TbTask> GetAll()
        {
            var tasks = _context.TbTasks
                .Include(t => t.AssignedToNavigation)
                .ToList();

            return tasks;
        }

        public TbTask GetById(int id)
        {
            return _context.TbTasks.Include(t => t.AssignedToNavigation).FirstOrDefault(t => t.Id == id);
        }

        public List<TbTask> GetByProjectId(int id)
        {
            return _context.TbTasks.Where(t => t.ProjectId == id).Include(t => t.AssignedToNavigation).ToList();
        }

        public List<TbTask> GetByUser(string userId)
        {
            return _context.TbTasks
                .Where(t => t.AssignedTo == userId)
                .OrderByDescending(t => t.DueDate)
                .ToList();
        }

        public void Add(TbTask task)
        {
            _context.TbTasks.Add(task);
        }

        public void Update(TbTask task)
        {
            _context.TbTasks.Update(task);
        }

        public void Delete(TbTask task)
        {
            _context.TbTasks.Remove(task);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public List<TbTask> GetTasksByUserId(string userId)
        {
            return _context.TbTasks
                .Where(t => t.AssignedTo == userId)
                .Include(t => t.AssignedToNavigation)
                .ToList();
        }

        public TbTask GetById(string id)
        {
            throw new NotImplementedException();
        }
    }
}
