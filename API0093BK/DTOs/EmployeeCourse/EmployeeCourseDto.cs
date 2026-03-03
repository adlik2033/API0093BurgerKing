namespace API0093BK.DTOs.EmployeeCourse
{
    public class EmployeeCourseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string EmployeeNumber { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public string CourseExternalId { get; set; } = string.Empty;
        public bool IsMandatory { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? CompletionDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? LastSyncDate { get; set; }
    }
}