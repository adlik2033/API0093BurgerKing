namespace API0093BK.DTOs.Course
{
    public class CourseDto
    {
        public int Id { get; set; }
        public string ExternalId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public bool IsMandatory { get; set; }
        public DateTime? LastSyncDate { get; set; }
    }
}