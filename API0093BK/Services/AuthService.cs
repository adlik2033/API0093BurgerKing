using API0093BK.DTOs.Auth;
using API0093BK.DTOs.User;
using API0093BK.Helpers;
using API0093BK.Models;
using API0093BK.Repositories.Interfaces;
using API0093BK.Services.Interfaces;

namespace API0093BK.Services
{
    /// <summary>
    /// Реализация сервиса аутентификации
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtHelper _jwtHelper;

        public AuthService(IUserRepository userRepository, JwtHelper jwtHelper)
        {
            _userRepository = userRepository;
            _jwtHelper = jwtHelper;
        }

        /// <summary>
        /// Аутентификация пользователя и получение JWT токена
        /// </summary>
        /// <param name="loginDto">Данные для входа</param>
        /// <returns>Токен доступа и информация о пользователе</returns>
        /// <exception cref="UnauthorizedAccessException">Если логин/пароль неверны или пользователь неактивен</exception>
        public async Task<TokenDto> AuthenticateAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetUserByUsernameAsync(loginDto.Username);

            // Проверка существования пользователя и пароля
            if (user == null || !PasswordHelper.VerifyPassword(loginDto.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Неверное имя пользователя или пароль");
            }

            // Проверка активности пользователя
            if (!user.IsActive)
            {
                throw new UnauthorizedAccessException("Ваш аккаунт деактивирован. Обратитесь к администратору.");
            }

            // Обновление времени последнего входа
            user.LastLoginAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);

            // Генерация токена
            var token = _jwtHelper.GenerateToken(user);

            return new TokenDto
            {
                AccessToken = token,
                ExpiresAt = DateTime.UtcNow.AddHours(8),
                User = MapToDto(user)
            };
        }

        /// <summary>
        /// Преобразование модели User в DTO
        /// </summary>
        private UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.ToString(),
                PortalEmployeeId = user.PortalEmployeeId,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                CreatedBy = "System"
            };
        }
    }
}