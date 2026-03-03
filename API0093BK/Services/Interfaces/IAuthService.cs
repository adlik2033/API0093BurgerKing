using API0093BK.DTOs.Auth;

namespace API0093BK.Services.Interfaces
{
    public interface IAuthService
    {
        Task<TokenDto> AuthenticateAsync(LoginDto loginDto);
    }
}