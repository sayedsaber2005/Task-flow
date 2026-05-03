using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.BL.Interfaces;
using ProjectManagement.DTOs.ProjectMembers;
using ProjectManagement.DTOs.Users;
using ProjectManagement.Repositories.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProjectManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectMemberController : ControllerBase
    {
        private readonly IProjectMembers _projectMemberBL;

        public ProjectMemberController(IProjectMembers projectMemberBL)
        {
            _projectMemberBL = projectMemberBL;
        }

        [Authorize(Roles = "Project Manager")]
        [HttpPost ("add-member")]
        public IActionResult AddMember([FromBody] AddMemberDTO dto)
        {
            var result = _projectMemberBL.AddMember(dto);
            return StatusCode(int.Parse(result.StatusCode), result);
        }

        [Authorize(Roles = "Project Manager")]
        [HttpGet("{projectId}")]
        public IActionResult GetMembers(int projectId)
        {
            var result = _projectMemberBL.GetProjectMembers(projectId);
            return StatusCode(int.Parse(result.StatusCode), result);
        }


        [Authorize(Roles = "Project Manager")]
        [HttpPost("RemoveMember")]
        public IActionResult RemoveMember([FromBody] RemoveMemberDTO dto)
        {
            var result = _projectMemberBL.RemoveMember(dto.ProjectId, dto.UserId);
            return StatusCode(int.Parse(result.StatusCode), result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("ChangeRole")]
        public IActionResult ChangeRole([FromBody] ProjectRoleDTO dto)
        {
            var result = _projectMemberBL.ChangeRole(dto);
            return StatusCode(int.Parse(result.StatusCode), result);
        }

        [HttpGet("Search-User")]
        [Authorize(Roles = "Project Manager")]
        public IActionResult SearchUser([FromBody] SearchUserDTO dto)
        {
            var result =_projectMemberBL.SearchUserByEmail(dto);
            return StatusCode(int.Parse(result.StatusCode),result);
        }
    }
}
