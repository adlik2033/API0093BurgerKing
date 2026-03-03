using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API0093BK.Models
{
    /// <summary>
    /// Связь сотрудника с курсами (прогресс обучения)
    /// </summary>
    public class EmployeeCourse
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }                          // ID сотрудника

        [Required]
        public int CourseId { get; set; }                        // ID курса

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = CourseStatus.NotStarted;  // NotStarted, InProgress, Completed, Expired

        [Column(TypeName = "date")]
        public DateTime? CompletionDate { get; set; }            // Дата завершения

        [Column(TypeName = "date")]
        public DateTime? ExpiryDate { get; set; }                // Дата истечения

        public DateTime? LastSyncDate { get; set; }              // Дата последней синхронизации

        // Навигационные свойства
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("CourseId")]
        public virtual Course? Course { get; set; }
    }

    /// <summary>
    /// Константы для статусов курсов
    /// </summary>
    public static class CourseStatus
    {
        public const string NotStarted = "NotStarted";
        public const string InProgress = "InProgress";
        public const string Completed = "Completed";
        public const string Expired = "Expired";

        public static readonly string[] All = { NotStarted, InProgress, Completed, Expired };
    }
}