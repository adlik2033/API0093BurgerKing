using System.ComponentModel.DataAnnotations;

namespace API0093BK.DTOs.Schedule
{
    public class ScheduleCreateDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime WeekStartDate { get; set; }

        [Required]
        public DateTime WorkDate { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }
    }
}