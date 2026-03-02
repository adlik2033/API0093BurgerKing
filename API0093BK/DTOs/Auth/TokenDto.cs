using API0093BK.DTOs.User;

namespace API0093BK.DTOs.Auth
{
    /// <summary>
    /// DTO с токеном доступа
    /// </summary>
    public class TokenDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string TokenType { get; set; } = "Bearer";
        public DateTime ExpiresAt { get; set; }
        public UserDto? User { get; set; }
    }
}