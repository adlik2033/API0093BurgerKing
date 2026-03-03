using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API0093BK.Models
{
    /// <summary>
    /// Модель пожелания сотрудника
    /// </summary>
    public class Wish
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }                    // ID сотрудника

        [Required]
        [Column(TypeName = "date")]
        public DateTime RequestedDate { get; set; }        // Дата запроса

        [Column(TypeName = "time")]
        public TimeSpan? StartTime { get; set; }           // Желаемое время начала

        [Column(TypeName = "time")]
        public TimeSpan? EndTime { get; set; }             // Желаемое время окончания

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = WishStatus.Pending;    // Pending, Approved, Rejected

        [StringLength(500)]
        public string? Comment { get; set; }               // Комментарий

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // Навигационные свойства
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }

    /// <summary>
    /// Константы для статусов пожеланий
    /// </summary>
    public static class WishStatus
    {
        public const string Pending = "Pending";      // Ожидает рассмотрения
        public const string Approved = "Approved";    // Одобрено
        public const string Rejected = "Rejected";    // Отклонено

        public static readonly string[] All = { Pending, Approved, Rejected };
    }
}