using System.ComponentModel.DataAnnotations;

namespace API0093BK.Models
{
    /// <summary>
    /// Модель курса с портала обучения
    /// </summary>
    public class Course
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string ExternalId { get; set; } = string.Empty;  // ID курса из внешней системы

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;        // Название курса

        [Required]
        public bool IsMandatory { get; set; }                    // Обязательный ли курс

        public DateTime? LastSyncDate { get; set; }              // Дата последней синхронизации

        // Навигационные свойства
        public virtual ICollection<EmployeeCourse> EmployeeCourses { get; set; } = new List<EmployeeCourse>();
    }
}