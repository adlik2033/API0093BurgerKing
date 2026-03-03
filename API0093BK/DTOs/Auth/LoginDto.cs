using System.ComponentModel.DataAnnotations;

namespace API0093BK.DTOs.Auth
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Табельный номер обязателен")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Пароль обязателен")]
        public string Password { get; set; } = string.Empty;
    }
}