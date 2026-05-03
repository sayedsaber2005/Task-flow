using ProjectManagement.DTOs;
using ProjectManagement.DTOs.Comments;

namespace ProjectManagement.BL.Interfaces
{
    public interface IComment
    {
        ApiResponse GetByTaskId(int taskId);
        Task<ApiResponse> AddCommentAsync(int taskId, string userId, CreateCommentDTO dto);
        ApiResponse DeleteComment(int commentId, string userId, string role);
    }
}
