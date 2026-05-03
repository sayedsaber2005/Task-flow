using ProjectManagement.DTOs;
using ProjectManagement.DTOs.Attachments;

namespace ProjectManagement.BL.Interfaces
{
    public interface IAttachment
    {
        ApiResponse GetByTaskId(int taskId);
        Task<ApiResponse> Upload(int taskId, string userId, IFormFile file);
        ApiResponse Delete(int attachmentId, string userId);
    }
}
