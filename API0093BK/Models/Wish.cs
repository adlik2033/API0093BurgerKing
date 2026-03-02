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
        public int UserId { get; set; }

        [Required]
        public DateTime WishDate { get; set; }

        [Required]
        public WishType Type { get; set; }

        [StringLength(500)]
        public string? Comment { get; set; }

        [Required]
        public WishStatus Status { get; set; } = WishStatus.Pending;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // Навигационные свойства
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }

    /// <summary>
    /// Типы пожеланий
    /// </summary>
    public enum WishType
    {
        DayOff = 1,          // Выходной
        SpecificHours = 2,   // Конкретные часы
        PreferNotToWork = 3  // Предпочитаю не работать
    }

    /// <summary>
    /// Статусы пожеланий
    /// </summary>
    public enum WishStatus
    {
        Pending = 1,  // Ожидает рассмотрения
        Approved = 2, // Одобрено
        Rejected = 3  // Отклонено
    }
}