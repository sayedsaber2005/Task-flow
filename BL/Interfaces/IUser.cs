using ProjectManagement.DTOs;
using ProjectManagement.DTOs.Users;

namespace ProjectManagement.BL.Interfaces
{
    public interface IUser
    {
        Task<ApiResponse> Register(RegisterDTO dto);
        Task<ApiResponse> Login(LoginDTO dto);
        Task<ApiResponse> GetAllUsers();
        Task<ApiResponse> GetById(string id);
        Task<ApiResponse> GetCurrentUser(string userId);
        Task<ApiResponse> ApproveProjectManager(string userId);
        Task<ApiResponse> RejectProjectManager(string userId);
        Task<ApiResponse> GetPendingProjectManagers();
        Task<ApiResponse> GetMyProfile();
        Task<ApiResponse> UpdateProfile(UpdateProfileDTO dto);
        Task<ApiResponse> ChangePassword(ChangePasswordDTO dto);
        Task<ApiResponse> UploadProfileImage(IFormFile file);
        ApiResponse GetUserDashboard();
        Task<ApiResponse> GetAdminDashboard();
        ApiResponse GetActivityTrend();
        Task<ApiResponse> AssignRole(RoleDTO dto);
        Task<ApiResponse> ChangeRole(RoleDTO dto);
        Task<ApiResponse> RemoveRole(RoleDTO dto);
        Task<ApiResponse> CreateUser(AdminCreateUserDTO dto);
        Task<ApiResponse> DeleteUser(string userId);
    }
}
