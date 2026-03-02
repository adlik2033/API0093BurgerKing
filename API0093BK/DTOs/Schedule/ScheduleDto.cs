namespace API0093BK.DTOs.Schedule
{
    /// <summary>
    /// DTO для отображения расписания
    /// </summary>
    public class ScheduleDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime WorkDate { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}