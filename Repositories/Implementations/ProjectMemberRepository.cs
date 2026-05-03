using Microsoft.EntityFrameworkCore;
using ProjectManagement.Models;
using ProjectManagement.Repositories.Interfaces;

namespace ProjectManagement.Repositories.Implementations
{
    public class ProjectMemberRepository : IProjectMemberRepository
    {
        private readonly ProjectManagementContext _context;

        public ProjectMemberRepository(ProjectManagementContext context)
        {
            _context = context;
        }

        public void Add(TbProjectMember member)
            => _context.TbProjectMembers.Add(member);

        public List<TbProjectMember> GetByProjectId(int projectId)
            => _context.TbProjectMembers
                .Include(m => m.User)
                .Where(m => m.ProjectId == projectId)
                .ToList();

        public TbProjectMember GetByUserAndProject(string userId, int projectId)
            => _context.TbProjectMembers
                .FirstOrDefault(m => m.UserId == userId && m.ProjectId == projectId);

        public List<TbProjectMember> GetByUser(string userId)
            => _context.TbProjectMembers
                .Include(m => m.Project) 
                .Where(m => m.UserId == userId)
                .ToList();

        public void Delete(TbProjectMember member)
            => _context.TbProjectMembers.Remove(member);

        public void Save()
            => _context.SaveChanges();

        public object SearchUserByEmailNotInProject(string email,int projectId)
        {
            // Users already in project
            var projectUserIds =_context.TbProjectMembers
                .Where(pm => pm.ProjectId == projectId)
                .Select(pm => pm.UserId)
                .ToList();

            // Search user by email
            var user =_context.Users
                .Where(u =>u.Email == email &&!projectUserIds.Contains(u.Id))
                .Select(u => new
                {
                    Id = u.Id,
                    Name = u.FirstName + " " + u.LastName,
                    Email = u.Email,
                    Role =_context.UserRoles
                        .Where(ur => ur.UserId == u.Id)
                        .Join(
                            _context.Roles,
                            ur => ur.RoleId,
                            r => r.Id,
                            (ur, r) => r.Name
                        )
                        .FirstOrDefault()
                })
                .FirstOrDefault();

            return user;
        }
    }
}
