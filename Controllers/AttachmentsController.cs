using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.BL.Interfaces;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProjectManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttachmentsController : ControllerBase
    {
        private readonly IAttachment _attachmentBL;
        private readonly IHttpContextAccessor _httpContext;

        public AttachmentsController(IAttachment attachmentBL, IHttpContextAccessor httpContext)
        {
            _attachmentBL = attachmentBL;
            _httpContext = httpContext;
        }

        // GET
        [HttpGet]
        public IActionResult Get(int taskId)
        {
            var result = _attachmentBL.GetByTaskId(taskId);
            return StatusCode(int.Parse(result.StatusCode), result);
        }

        // UPLOAD
        [HttpPost]
        public async Task<IActionResult> Upload(int taskId, [FromForm(Name = "file")] IFormFile File)
        {
            var userId = _httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _attachmentBL.Upload(taskId, userId, File);

            return StatusCode(int.Parse(result.StatusCode), result);
        }

        // DELETE
        [Authorize]
        [HttpDelete("{attachmentId}")]
        public IActionResult Delete(int attachmentId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = _attachmentBL.Delete(attachmentId, userId);

            return StatusCode(int.Parse(result.StatusCode), result);
        }
    }
}
