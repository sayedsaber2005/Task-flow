using ProjectManagement.DTOs;

namespace ProjectManagement.BL.Interfaces
{
    public interface IActivity
    {
        void Log(string userId, string action, string entityType, string entityId, string description);

        ApiResponse GetRecent(int count);
    }
}
