using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.BL.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProjectManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Project Manager")]
    public class ActivityController : ControllerBase
    {
        private readonly IActivity _service;

        public ActivityController(IActivity service)
        {
            _service = service;
        }

        [HttpGet("recent")]
        public IActionResult GetRecent()
        {
            var result = _service.GetRecent(10);
            return StatusCode(int.Parse(result.StatusCode), result);
        }
    }
}
