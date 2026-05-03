using Microsoft.EntityFrameworkCore;
using ProjectManagement.DTOs.Tasks;
using ProjectManagement.Models;
using ProjectManagement.Repositories.Interfaces;

namespace ProjectManagement.Repositories.Implementations
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ProjectManagementContext _context;

        public ProjectRepository(ProjectManagementContext context)
        {
            _context = context;
        }

        public List<TbProject> GetAll()
        {
            return _context.TbProjects
                .Include(p => p.TbProjectMembers)
                    .ThenInclude(m => m.User)
                .ToList();
        }

        public TbProject GetById(int id)
        {
            return _context.TbProjects
                .Include(p => p.TbTasks)
                    .ThenInclude(t => t.AssignedToNavigation)
                .Include(p => p.TbProjectMembers)
                    .ThenInclude(m => m.User)
                .FirstOrDefault(t => t.Id == id);
        }

        public TbProject GetProjectWithDetails(int projectId)
        {
            return _context.TbProjects
                .Include(p => p.TbTasks)
                    .ThenInclude(t => t.AssignedToNavigation)
                .Include(p => p.TbProjectMembers)
                    .ThenInclude(m => m.User)
                .FirstOrDefault(p => p.Id == projectId);
        }

        public void Add(TbProject project)
        {
            _context.TbProjects.Add(project);
        }

        public void Update(TbProject project)
        {
            _context.TbProjects.Update(project);
        }

        public void Delete(TbProject project)
        {
            _context.TbProjects.Remove(project);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public TbProject GetProjectWithTasks(int projectId)
    
        {
            return _context.TbProjects
                .Include(p => p.TbTasks)
                .FirstOrDefault(p => p.Id == projectId);
        }


        public List<TbTask> GetTasksWithUsers(int projectId)
        {
            return _context.TbTasks
                .Include(t => t.AssignedToNavigation)
                .Where(t => t.ProjectId == projectId)
                .ToList();
        }
    }
}
