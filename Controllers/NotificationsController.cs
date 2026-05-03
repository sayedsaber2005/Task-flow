using Microsoft.AspNetCore.Mvc;
using ProjectManagement.BL.Interfaces;
using ProjectManagement.Repositories.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProjectManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly INotification _notificationBL;

        public NotificationsController(INotification notificationBL)
        {
            _notificationBL = notificationBL;
        }

        [HttpGet("my")]
        public IActionResult GetMyNotifications()
        {
            var result = _notificationBL.GetMyNotifications();
            return StatusCode(int.Parse(result.StatusCode), result);
        }
    }
}
