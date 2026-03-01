using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SWP_BE.Data;
using SWP_BE.DTOs;
using SWP_BE.Models;
using System.Security.Claims;

namespace SWP_BE.Controllers
{
    [ApiController]
    [Route("api/manager/projects")]
    [Authorize(Roles = "Manager")]
    public class ManagerController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ManagerController(AppDbContext context)
        {
            _context = context;
        }

        private Guid GetManagerId()
        {
            return Guid.Parse(User.FindFirst("id")!.Value);
        }

        // ===============================
        // GET: api/manager/projects
        // ===============================
        [HttpGet]
        public async Task<IActionResult> GetProjects()
        {
            var managerId = GetManagerId();

            var projects = await _context.Projects
                .Where(p => p.ManagerID == managerId)
                .Select(p => new ProjectResponseDto
                {
                    ProjectID = p.ProjectID,
                    ProjectName = p.ProjectName,
                    Description = p.Description,
                    Topic = p.Topic,
                    ProjectType = p.ProjectType,
                    Status = p.Status,
                    CreatedAt = p.CreatedAt,
                    GuidelineUrl = p.GuidelineUrl
                })
                .ToListAsync();

            return Ok(projects);
        }

        // ===============================
        // POST: api/manager/projects
        // ===============================
        [HttpPost]
        public async Task<IActionResult> CreateProject(CreateProjectDto dto)
        {
            var managerId = GetManagerId();

            var project = new Project
            {
                ProjectName = dto.ProjectName,
                Description = dto.Description,
                Topic = dto.Topic,
                ProjectType = dto.ProjectType,
                Status = "Open",
                CreatedAt = DateTime.UtcNow,
                ManagerID = managerId,
                GuidelineUrl = ""
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Project created successfully" });
        }

        // ===============================
        // PUT: api/manager/projects/{id}
        // ===============================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, UpdateProjectDto dto)
        {
            var managerId = GetManagerId();

            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.ProjectID == id && p.ManagerID == managerId);

            if (project == null)
                return NotFound("Project not found");

            project.ProjectName = dto.ProjectName;
            project.Description = dto.Description;
            project.Topic = dto.Topic;
            project.ProjectType = dto.ProjectType;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Project updated successfully" });
        }

        // ===============================
        // PATCH: api/manager/projects/{id}/status
        // ===============================
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> ChangeStatus(int id, [FromQuery] string status)
        {
            var managerId = GetManagerId();

            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.ProjectID == id && p.ManagerID == managerId);

            if (project == null)
                return NotFound("Project not found");

            project.Status = status;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Status updated successfully" });
        }

        // ===============================
        // POST: api/manager/projects/{id}/guideline
        // ===============================
        [HttpPost("{id}/guideline")]
        public async Task<IActionResult> UpdateGuideline(int id, [FromQuery] string url)
        {
            var managerId = GetManagerId();

            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.ProjectID == id && p.ManagerID == managerId);

            if (project == null)
                return NotFound("Project not found");

            project.GuidelineUrl = url;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Guideline updated successfully" });
        }
    }
}