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
        [Column(TypeName = "date")]
        public DateTime WeekStartDate { get; set; }        // Начало недели

        [Required]
        public int UserId { get; set; }                     // ID сотрудника

        [Required]
        [Column(TypeName = "date")]
        public DateTime WorkDate { get; set; }              // Дата работы

        [Required]
        [Column(TypeName = "time")]
        public TimeSpan StartTime { get; set; }             // Время начала

        [Required]
        [Column(TypeName = "time")]
        public TimeSpan EndTime { get; set; }               // Время окончания

        [Required]
        public bool IsFinal { get; set; } = false;          // Утверждено ли

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? ApprovedBy { get; set; }                 // Кто утвердил

        // Навигационные свойства
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("ApprovedBy")]
        public virtual User? Approver { get; set; }
    }
}