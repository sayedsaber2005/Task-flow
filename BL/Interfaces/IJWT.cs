using ProjectManagement.Models;

namespace ProjectManagement.BL.Interfaces
{
    public interface IJWT
    {
        Task<string> GenerateToken(ApplicationUser user);
    }
}
