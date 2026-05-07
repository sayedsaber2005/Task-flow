using ProjectManagement.Models;
using ProjectManagement.Repositories.Interfaces;

namespace ProjectManagement.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly ProjectManagementContext _context;

        public UserRepository(ProjectManagementContext context)
        {
            _context = context;
        }

        // get all users
        public IEnumerable<ApplicationUser> GetAll()
        {
            return _context.Users.ToList();
        }

        public ApplicationUser GetById(string id)
        {
            return _context.Users.FirstOrDefault(u => u.Id == id);
        }

        public ApplicationUser GetByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }

        public ApplicationUser GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void Add(ApplicationUser entity)
        {
            throw new NotImplementedException();
        }

        public void Update(ApplicationUser entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(ApplicationUser entity)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }
    }
}
