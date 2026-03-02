using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API0093BK.Models
{
    /// <summary>
    /// Модель расписания
    /// </summary>
    public class Schedule
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public DateTime WeekStartDate { get; set; }

        [Required]
        public DateTime WeekEndDate { get; set; }

        [Required]
        public DateTime WorkDate { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        [Required]
        public ScheduleStatus Status { get; set; } = ScheduleStatus.Draft;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? ApprovedBy { get; set; }

        // Навигационные свойства
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("ApprovedBy")]
        public virtual User? Approver { get; set; }
    }

    /// <summary>
    /// Статусы расписания
    /// </summary>
    public enum ScheduleStatus
    {
        Draft = 1,    // Черновик
        Final = 2,    // Окончательный вариант
        Approved = 3  // Утверждено
    }
}