using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWP_BE.DTOs;
using SWP_BE.Services;

namespace SWP_BE.Controllers
{
    [ApiController]
    [Route("api/manager/projects")]
    [Authorize(Roles = "Manager")]
    public class ManagerController : ControllerBase
    {
        private readonly IProjectService _projectService;
        public ManagerController(IProjectService projectService) { _projectService = projectService; }

        private Guid GetManagerId()
        {
            var claim = User.FindFirst("id");
            if (claim == null) throw new UnauthorizedAccessException("Phiên đăng nhập hết hạn hoặc thiếu ID.");
            return Guid.Parse(claim.Value);
        }

        [HttpGet]
        public async Task<IActionResult> GetProjects()
        {
            var managerId = GetManagerId();
            return Ok(await _projectService.GetProjectsAsync(managerId));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProject(Guid id) // int -> Guid
        {
            var managerId = GetManagerId();
            var result = await _projectService.GetProjectByIdAsync(id, managerId);
            return result == null ? NotFound("Project không tồn tại.") : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject(CreateProjectDto dto)
        {
            var managerId = GetManagerId();
            var projectId = await _projectService.CreateProjectAsync(dto, managerId);
            return Ok(new { message = "Project created successfully", projectId = projectId });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(Guid id, UpdateProjectDto dto) // int -> Guid
        {
            var managerId = GetManagerId();
            var success = await _projectService.UpdateProjectAsync(id, dto, managerId);
            return success ? Ok(new { message = "Updated" }) : NotFound();
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> ChangeStatus(Guid id, [FromQuery] string status) // int -> Guid
        {
            var managerId = GetManagerId();
            var success = await _projectService.ChangeStatusAsync(id, status, managerId);
            return success ? Ok(new { message = "Status updated" }) : NotFound();
        }

        [HttpPost("{id}/guideline")]
        public async Task<IActionResult> UpdateGuideline(Guid id, [FromQuery] string url) // int -> Guid
        {
            var managerId = GetManagerId();
            var success = await _projectService.UpdateGuidelineAsync(id, url, managerId);
            return success ? Ok(new { message = "Guideline updated" }) : NotFound();
        }

        [HttpPost("{id}/data")]
        public async Task<IActionResult> UploadData(Guid id, [FromBody] UploadDataDto dto) // int -> Guid
        {
            if (dto.FileUrls == null || !dto.FileUrls.Any()) return BadRequest("Links không được để trống.");
            var managerId = GetManagerId();
            var success = await _projectService.UploadDataAsync(id, dto, managerId);
            return success ? Ok(new { message = "Data added" }) : NotFound();
        }

        [HttpPost("{id}/split-tasks")]
        public async Task<IActionResult> SplitTasks(Guid id, [FromBody] SplitTaskDto dto) // int -> Guid
        {
            var managerId = GetManagerId();
            try
            {
                var result = await _projectService.SplitTasksAsync(id, dto, managerId);
                return result == null ? NotFound() : Ok(result);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
    }
}