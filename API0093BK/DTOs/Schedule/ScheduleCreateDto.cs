using System.ComponentModel.DataAnnotations;

namespace API0093BK.DTOs.Schedule
{
    /// <summary>
    /// DTO для создания/обновления расписания
    /// </summary>
    public class ScheduleCreateDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime WeekStartDate { get; set; }

        [Required]
        public DateTime WorkDate { get; set; }

        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
    }
}