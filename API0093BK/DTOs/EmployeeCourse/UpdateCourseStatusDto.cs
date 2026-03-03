using System.ComponentModel.DataAnnotations;

namespace API0093BK.DTOs.EmployeeCourse
{
    public class UpdateCourseStatusDto
    {
        [Required]
        public int CourseId { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = string.Empty;

        public DateTime? CompletionDate { get; set; }

        public DateTime? ExpiryDate { get; set; }
    }
}