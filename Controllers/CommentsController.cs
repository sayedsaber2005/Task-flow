using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.BL.Interfaces;
using ProjectManagement.DTOs.Comments;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProjectManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly IComment _commentBL;
        private readonly IHttpContextAccessor _httpContext;

        public CommentsController(IComment commentBL, IHttpContextAccessor httpContext)
        {
            _commentBL = commentBL;
            _httpContext = httpContext;
        }

        // GET
        [HttpGet]
        public IActionResult GetComments(int taskId)
        {
            var result = _commentBL.GetByTaskId(taskId);
            return StatusCode(int.Parse(result.StatusCode), result);
        }

        // POST
        [HttpPost]
        public async Task<IActionResult> AddCommentAsync(int taskId, CreateCommentDTO dto)
        {
            var userId = _httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _commentBL.AddCommentAsync(taskId, userId, dto);

            return StatusCode(int.Parse(result.StatusCode), result);
        }

        [Authorize]
        [HttpDelete("{commentId}")]
        public IActionResult DeleteComment(int commentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);

            var result = _commentBL.DeleteComment(commentId, userId, role);

            return StatusCode(int.Parse(result.StatusCode), result);
        }
    }
}
