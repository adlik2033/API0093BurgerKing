using System.ComponentModel.DataAnnotations;

namespace API0093BK.Models
{
    /// <summary>
    /// Модель пользователя системы
    /// </summary>
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public UserRole Role { get; set; }

        // ID сотрудника на портале обучения
        public int? PortalEmployeeId { get; set; }

        public DateTime CreatedAt { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public int? UpdatedBy { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? LastLoginAt { get; set; }

        // Навигационные свойства
        public virtual ICollection<Wish> Wishes { get; set; } = new List<Wish>();
        public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    }

    /// <summary>
    /// Роли пользователей
    /// </summary>
    public enum UserRole
    {
        Administrator = 1, // Суперпользователь
        Manager = 2,        // Менеджер
        Employee = 3        // Сотрудник
    }
}