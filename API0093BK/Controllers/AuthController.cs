using API0093BK.DTOs.Auth;
using API0093BK.DTOs.Common;
using API0093BK.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API0093BK.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Аутентификация пользователя и получение JWT токена
        /// </summary>
        /// <param name="loginDto">Данные для входа (логин и пароль)</param>
        /// <returns>JWT токен и информация о пользователе</returns>
        /// <response code="200">Успешная аутентификация</response>
        /// <response code="400">Ошибка валидации данных</response>
        /// <response code="401">Неверный логин/пароль или аккаунт деактивирован</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<TokenDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Ошибка валидации",
                        Errors = errors
                    });
                }

                var token = await _authService.AuthenticateAsync(loginDto);
                return Ok(new ApiResponse<TokenDto>(token, "Вход выполнен успешно"));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Неудачная попытка входа для пользователя: {Username}", loginDto.Username);
                return Unauthorized(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при входе пользователя: {Username}", loginDto.Username);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Произошла ошибка при входе. Пожалуйста, попробуйте позже."
                });
            }
        }

        /// <summary>
        /// Проверка валидности токена
        /// </summary>
        /// <returns>Статус токена</returns>
        [HttpGet("validate")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult ValidateToken()
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                return Ok(new ApiResponse<bool>(true, "Токен действителен"));
            }

            return Unauthorized(new ApiResponse<object>
            {
                Success = false,
                Message = "Недействительный или истекший токен"
            });
        }
    }
}