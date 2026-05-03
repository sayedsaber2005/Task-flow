using ProjectManagement.BL.Interfaces;
using ProjectManagement.DTOs;
using ProjectManagement.Models;
using ProjectManagement.Repositories.Interfaces;

namespace ProjectManagement.BL.Implementations
{
    public class ClsActivity : IActivity
    {
        private readonly IActivityRepository _repo;

        public ClsActivity(IActivityRepository repo)
        {
            _repo = repo;
        }

        public void Log(string userId, string action, string entityType, string entityId, string description)
        {
            var log = new TbActivityLog
            {
                UserId = userId,
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                Description = description,
                CreatedAt = DateTime.UtcNow
            };

            _repo.Add(log);
            _repo.Save();
        }

        public ApiResponse GetRecent(int count)
        {
            var data = _repo.GetRecent(count)
                .Select(x => new
                {
                    userId = x.UserId,
                    description = x.Description,
                    date = x.CreatedAt
                });

            return new ApiResponse
            {
                Data = data,
                StatusCode = "200"
            };
        }

    }
}
