using ProjectManagement.Models;

namespace ProjectManagement.Repositories.Interfaces
{
    public interface IProjectMemberRepository : IGenericRepository<TbProjectMember>
    {
        //void Add(TbProjectMember member);
        List<TbProjectMember> GetByProjectId(int projectId);
        List<TbProjectMember> GetByUser(string userId);
        TbProjectMember GetByUserAndProject(string userId, int projectId);
        //void Delete(TbProjectMember member);
        //void Save();
        object SearchUserByEmailNotInProject(string email,int projectId);
    }
}
