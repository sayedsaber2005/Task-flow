using ProjectManagement.Models;

namespace ProjectManagement.Repositories.Interfaces
{
    public interface IActivityRepository
    {
        void Add(TbActivityLog log);
        void Save();
        List<TbActivityLog> GetRecent(int count);
        List<TbActivityLog> GetByUser(string userId);
    }
}
