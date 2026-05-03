using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.BL.Interfaces;
using ProjectManagement.DTOs;
using ProjectManagement.DTOs.Projects;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProjectManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectController : ControllerBase
    {
        private readonly IProject _projectBL;

        public ProjectController(IProject projectBL)
        {
            _projectBL = projectBL;
        }

        [HttpGet]
        [Authorize (Roles = "Admin")]
        public IActionResult GetAll()
        {
            var result = _projectBL.GetAll();
            return StatusCode(int.Parse(result.StatusCode), result);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var result = _projectBL.GetById(id);
            return StatusCode(int.Parse(result.StatusCode), result);
        }

        [HttpGet("myProjects")]
        public IActionResult GetMyProjects()
        {
            var result = _projectBL.GetMyProjects();
            return StatusCode(int.Parse(result.StatusCode), result);
        }

        [HttpGet("{id}/details")]
        public IActionResult GetProjectDetails(int id)
        {
            var result = _projectBL.GetProjectDetails(id);
            return StatusCode(int.Parse(result.StatusCode), result);
        }

        [HttpGet("{id}/dashboard")]
        public IActionResult GetDashboard(int id)
        {
            var result = _projectBL.GetProjectDashboard(id);
            return StatusCode(int.Parse(result.StatusCode), result);
        }

        [Authorize(Roles = "Project Manager,Admin")]
        [HttpGet("{projectId}/stats")]
        public IActionResult GetProjectStats(int projectId)
        {
            var result = _projectBL.GetProjectStats(projectId);
            return StatusCode(int.Parse(result.StatusCode), result);
        }

        [Authorize(Roles = "Project Manager")]
        [HttpPost]
        public IActionResult Create([FromBody] CreateProjectDTO dto)
        {
            if (!ModelState.IsValid)
            {
                var result = new ApiResponse
                {
                    Data = null,
                    Errors = new List<object> { ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() },
                    StatusCode = "400"
                };
                return StatusCode(int.Parse(result.StatusCode), result);
            }
            var response = _projectBL.Create(dto);
            return StatusCode(int.Parse(response.StatusCode), response);
        }

        [Authorize(Roles = "Project Manager")]
        [HttpPost("Update")]
        public IActionResult Update([FromBody] UpdateProjectDTO dto)
        {
            if (!ModelState.IsValid)
            {
                var result = new ApiResponse
                {
                    Data = null,
                    Errors = new List<object> { ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList() },
                    StatusCode = "400"
                };
                return StatusCode(int.Parse(result.StatusCode), result);
            }
            var response = _projectBL.Update(dto);
            return StatusCode(int.Parse(response.StatusCode), response);
        }

        [Authorize(Roles = "Admin, Project Manager")]
        [HttpPost("Delete")]
        public IActionResult Delete([FromBody] int id)
        {
            var result = _projectBL.Delete(id);
            return StatusCode(int.Parse(result.StatusCode), result);
        }

        [Authorize(Roles = "Project Manager")]
        [HttpGet("Progress/{projectId}")]
        public IActionResult GetProjectProgress(int projectId)
        {
            var result =
                _projectBL.GetProjectProgress(projectId);

            return StatusCode(
                int.Parse(result.StatusCode),
                result);
        }

        [Authorize(Roles = "Project Manager")]
        [HttpGet("TeamWorkload/{projectId}")]
        
        public IActionResult GetTeamWorkload(int projectId)
        {
            var result =
                _projectBL.GetTeamWorkload(projectId);

            return StatusCode(
                int.Parse(result.StatusCode),
                result);
        }
    }
}
