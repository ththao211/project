using System.ComponentModel.DataAnnotations;

namespace SWP_BE.DTOs
{
    public class CreateProjectDto
    {
        [Required]
        [MaxLength(200)]
        public string ProjectName { get; set; } = string.Empty;

        public string? Description { get; set; }
        public string? Topic { get; set; }
        public string? ProjectType { get; set; }
    }

    public class UpdateProjectDto
    {
        [Required]
        public string ProjectName { get; set; } = string.Empty;

        public string? Description { get; set; }
        public string? Topic { get; set; }
        public string? ProjectType { get; set; }
    }

    public class ProjectResponseDto
    {
        public int ProjectID { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Topic { get; set; }
        public string? ProjectType { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? GuidelineUrl { get; set; }
    }
}