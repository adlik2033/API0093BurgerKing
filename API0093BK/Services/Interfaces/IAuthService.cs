using API0093BK.DTOs.Auth;

namespace API0093BK.Services.Interfaces
{
    /// <summary>
    /// Сервис аутентификации
    /// </summary>
    public interface IAuthService
    {
        Task<TokenDto> AuthenticateAsync(LoginDto loginDto);
    }
}