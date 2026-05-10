using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagement.BL.Implementations;
using ProjectManagement.BL.Interfaces;
using ProjectManagement.DTOs;
using ProjectManagement.DTOs.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProjectManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly ITask _taskBL;

        public TaskController(ITask taskBL)
        {
            _taskBL = taskBL;
        }

        // GET: api/<TaskController>
        [HttpGet]
        public IActionResult Get([FromQuery] TaskQueryDTO query)
        {
            var response = _taskBL.GetAllTasks(query);
            return StatusCode(int.Parse(response.StatusCode), response);
        }

        // GET api/<TaskController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var response = _taskBL.GetTaskById(id);
            return StatusCode(int.Parse(response.StatusCode), response);
        }

        // GET api/<TaskController>/5
        [HttpGet("GetByProjectId/{id}")]
        public IActionResult GetByProjectId(int id)
        {
            var tasks = _taskBL.GetTasksByProjectId(id);
            return Ok(tasks);
        }

        // POST api/<TaskController>
        [HttpPost]
        [Authorize(Roles = "Admin, Project Manager")]
        public async Task<IActionResult> Post([FromBody] CreateTaskDTO task)
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
            var response = await _taskBL.CreateTaskAsync(task);
            return StatusCode(int.Parse(response.StatusCode), response);
        }

        // POST api/<TaskController>
        [HttpPost]
        [Route("Update")]
        public async Task<IActionResult> UpdateAsync([FromBody] UpdateTaskDTO task)
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
            var response = await _taskBL.UpdateTaskAsync(task);
            return StatusCode(int.Parse(response.StatusCode), response);
        }

        // POST api/<TaskController>
        [HttpPost("delete")]
        [Authorize(Roles = "Project Manager")]
        public IActionResult Delete([FromBody] int id)
        {
            var response = _taskBL.DeleteTask(id);
            return StatusCode(int.Parse(response.StatusCode), response);
        }

        [Authorize(Roles = "Admin, Project Manager")]
        [HttpPost("assign")]
        public async Task<IActionResult> AssignTask([FromBody] AssignTaskDTO dto)
        {
            var result = await _taskBL.AssignTaskAsync(dto);
            return StatusCode(int.Parse(result.StatusCode), result);
        }

        [HttpPost("change-priority")]
        [Authorize(Roles = "Project Manager")]
        public async Task<IActionResult>ChangePriority([FromBody] ChangePriorityDTO dto)
        {
            var result = await _taskBL.ChangePriorityAsync(dto);
            return StatusCode( int.Parse(result.StatusCode),result);
        }

        [HttpGet("MyTasks")]
        public IActionResult GetMyTasks()
        {
            var response =_taskBL.GetMyTasks();
            return StatusCode(int.Parse(response.StatusCode),response);
        }
    }
}
