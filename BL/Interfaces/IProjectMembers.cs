using ProjectManagement.DTOs;
using ProjectManagement.DTOs.ProjectMembers;
using ProjectManagement.DTOs.Users;

namespace ProjectManagement.BL.Interfaces
{
    public interface IProjectMembers
    {
        ApiResponse AddMember(AddMemberDTO dto);
        ApiResponse GetProjectMembers(int projectId);
        ApiResponse RemoveMember(int projectId, string userId);
        ApiResponse ChangeRole(ProjectRoleDTO dto);
        ApiResponse SearchUserByEmail(SearchUserDTO dto);
    }
}
