using System.ComponentModel.DataAnnotations;

namespace API0093BK.DTOs.Course
{
    public class CourseCreateDto
    {
        [Required]
        [StringLength(100)]
        public string ExternalId { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public bool IsMandatory { get; set; }
    }
}