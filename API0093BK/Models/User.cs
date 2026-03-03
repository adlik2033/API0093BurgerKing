using System.ComponentModel.DataAnnotations;

namespace API0093BK.Models
{
    /// <summary>
    /// Модель пользователя (сотрудника)
    /// </summary>
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string EmployeeNumber { get; set; } = string.Empty;  // Табельный номер

        [Required]
        [StringLength(200)]
        public string FullName { get; set; } = string.Empty;        // Полное имя

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Role { get; set; } = "Employee";               // Employee, Manager, Administrator

        public DateTime? LastSyncDate { get; set; }                  // Дата последней синхронизации с порталом

        public DateTime CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
        public bool IsActive { get; set; } = true;

        // Навигационные свойства
        public virtual ICollection<Wish> Wishes { get; set; } = new List<Wish>();
        public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
        public virtual ICollection<EmployeeCourse> EmployeeCourses { get; set; } = new List<EmployeeCourse>();
    }
}