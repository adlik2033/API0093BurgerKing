using API0093BK.Models;
using System.ComponentModel.DataAnnotations;

namespace API0093BK.DTOs.User
{
    public class UserCreateDto
    {
        [Required(ErrorMessage = "Табельный номер обязателен")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Табельный номер должен быть от 3 до 50 символов")]
        public string EmployeeNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Пароль обязателен")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль должен быть не менее 6 символов")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный email адрес")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Полное имя обязательно")]
        [StringLength(200, ErrorMessage = "Полное имя не может превышать 200 символов")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Роль обязательна")]
        [StringLength(20)]
        public string Role { get; set; } = UserRoles.Employee;
    }
}