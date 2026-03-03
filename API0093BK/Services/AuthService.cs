using API0093BK.DTOs.Auth;
using API0093BK.DTOs.User;
using API0093BK.Helpers;
using API0093BK.Models;
using API0093BK.Repositories.Interfaces;
using API0093BK.Services.Interfaces;

namespace API0093BK.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtHelper _jwtHelper;

        public AuthService(IUserRepository userRepository, JwtHelper jwtHelper)
        {
            _userRepository = userRepository;
            _jwtHelper = jwtHelper;
        }

        public async Task<TokenDto> AuthenticateAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetUserByEmployeeNumberAsync(loginDto.Username);

            if (user == null || !PasswordHelper.VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Неверный табельный номер или пароль");
            }

            if (!user.IsActive)
            {
                throw new UnauthorizedAccessException("Ваш аккаунт деактивирован. Обратитесь к администратору.");
            }

            await _userRepository.UpdateAsync(user);

            var token = _jwtHelper.GenerateToken(user);

            return new TokenDto
            {
                AccessToken = token,
                ExpiresAt = DateTime.UtcNow.AddHours(8),
                User = MapToDto(user)
            };
        }

        private UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                EmployeeNumber = user.EmployeeNumber,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                LastSyncDate = user.LastSyncDate,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
            };
        }
    }
}