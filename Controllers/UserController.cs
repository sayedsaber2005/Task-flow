using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.BL.Interfaces;
using ProjectManagement.DTOs.Users;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProjectManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUser _userBL;
        public UserController(IUser userBL)
        {
            _userBL = userBL;
        }

        [Authorize(Roles = "Admin,Project Manager")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _userBL.GetAllUsers();
            return StatusCode(int.Parse(result.StatusCode), result);
        }

        [Authorize(Roles = "Admin,Project Manager")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _userBL.GetById(id);
            return StatusCode(int.Parse(result.StatusCode), result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            var result = await _userBL.Register(dto);
            return StatusCode(int.Parse(result.StatusCode), result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            var result = await _userBL.Login(dto);
            return StatusCode(int.Parse(result.StatusCode), result);
        }

        // [HttpGet("refresh-token")]
        // public async Task<IActionResult> RefreshToken()
        // {
        //     var refreshToken = Request.Cookies["refreshToken"];

        //     var result = await _userBL.RefreshTokenAsync(refreshToken);

        //     return StatusCode(int.Parse(result.StatusCode), result);
        // }

        // [HttpPost("revoke-token")]
        // public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenDTO dto)
        // {
        //     var token = dto.Token ?? Request.Cookies["refreshToken"];

        //     if (string.IsNullOrEmpty(token))
        //     {
        //         var response = new ApiResponse
        //         {
        //             Data = null,
        //             Errors = new List<object> { "Token is required" },
        //             StatusCode = "400"
        //         };
        //         return StatusCode(int.Parse(response.StatusCode), response);
        //     }

        //     var result = await _userBL.RevokeToken(token);
        //     if (!result)
        //     {
        //         var response = new ApiResponse
        //         {
        //             Data = null,
        //             Errors = new List<object> { "Token not found" },
        //             StatusCode = "404"
        //         };
        //         return StatusCode(int.Parse(response.StatusCode), response);
        //     }

        //     var successResponse = new ApiResponse
        //     {
        //         Data = null,
        //         Errors = null,
        //         StatusCode = "200"
        //     };

        //     return StatusCode(int.Parse(successResponse.StatusCode), successResponse);
        // }
        
        [Authorize(Roles = "Admin")]
        [HttpPost("approve/{userId}")]
        public async Task<IActionResult> Approve(string userId)
        {
            var result = await _userBL.ApproveProjectManager(userId);
            return StatusCode(int.Parse(result.StatusCode), result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("reject/{userId}")]
        public async Task<IActionResult> Reject(string userId)
        {
            var result = await _userBL.RejectProjectManager(userId);
            return StatusCode(int.Parse(result.StatusCode), result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingProjectManagers()
        {
            var result = await _userBL.GetPendingProjectManagers();
            return StatusCode(int.Parse(result.StatusCode), result);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var result = await _userBL.GetMyProfile();
            return StatusCode(int.Parse(result.StatusCode), result);
        }

        [Authorize]
        [HttpPost("me")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDTO dto)
        {
            var result = await _userBL.UpdateProfile(dto);
            return StatusCode(int.Parse(result.StatusCode), result);
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
        {
            var result = await _userBL.ChangePassword(dto);
            return StatusCode(int.Parse(result.StatusCode), result);
        }

        [Authorize]
        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile file)
        {
            var result = await _userBL.UploadProfileImage(file);
            return StatusCode(int.Parse(result.StatusCode), result);
        }

        [Authorize]
        [HttpGet("dashboard")]
        public IActionResult GetDashboard()
        {
            var result = _userBL.GetUserDashboard();
            return StatusCode(int.Parse(result.StatusCode), result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("adminDashboard")]
        public async Task<IActionResult> GetAdminDashboard()
        {
            var result = await _userBL.GetAdminDashboard();
            return StatusCode(int.Parse(result.StatusCode), result);
        }

        [Authorize(Roles = "Admin,Project Manager")]
        [HttpGet("activity-trend")]
        public IActionResult GetActivityTrend()
        {
            var result = _userBL.GetActivityTrend();
            return StatusCode(int.Parse(result.StatusCode), result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole(RoleDTO dto)
        {
            var result = await _userBL.AssignRole(dto);
            return StatusCode(int.Parse(result.StatusCode), result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("change-role")]
        public async Task<IActionResult> ChangeRole([FromBody] RoleDTO dto)
        {
            var result = await _userBL.ChangeRole(dto);
            return StatusCode(int.Parse(result.StatusCode), result);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("remove-role")]
        public async Task<IActionResult> RemoveRole(RoleDTO dto)
        {
            var result = await _userBL.RemoveRole(dto);
            return StatusCode(int.Parse(result.StatusCode), result);
        }

        // Create user
        [Authorize(Roles = "Admin")]
        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser(AdminCreateUserDTO dto)
        {
            var result = await _userBL.CreateUser(dto);
            return StatusCode(int.Parse(result.StatusCode), result);
        }

        // Delete user
        [Authorize(Roles = "Admin")]
        [HttpDelete("delete-user/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var result = await _userBL.DeleteUser(id);

            return StatusCode(int.Parse(result.StatusCode), result);
        }
    }
}
