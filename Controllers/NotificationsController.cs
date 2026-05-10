using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.BL.Interfaces;
using ProjectManagement.Repositories.Interfaces;
using System.Security.Claims;

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

        [HttpGet]
        public async Task<IActionResult> GetMyNotifications()
        {
            var userId = User
                .FindFirst(ClaimTypes.NameIdentifier)?
                .Value;

            var response = await _notificationBL
                .GetUserNotificationsAsync(userId);

            var notifications = response.Data;

            return StatusCode(int.Parse(response.StatusCode), response);
        }

        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var response = await _notificationBL.MarkAsReadAsync(id);

            return StatusCode(int.Parse(response.StatusCode), response);
        }

        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = User
                .FindFirst(ClaimTypes.NameIdentifier)?
                .Value;

            var response = await _notificationBL
                .GetUnreadCountAsync(userId);

            return StatusCode(int.Parse(response.StatusCode), response);
        }
    }
}
