using System.ComponentModel.DataAnnotations;

namespace API0093BK.DTOs.Auth
{
    /// <summary>
    /// DTO для входа в систему
    /// </summary>
    public class LoginDto
    {
        [Required(ErrorMessage = "Имя пользователя обязательно")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Пароль обязателен")]
        public string Password { get; set; } = string.Empty;
    }
}