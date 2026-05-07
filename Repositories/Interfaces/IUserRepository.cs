using ProjectManagement.Models;

namespace ProjectManagement.Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<ApplicationUser>
    {
        //List<ApplicationUser> GetAll();
        ApplicationUser GetByEmail(string email);
    }
}
