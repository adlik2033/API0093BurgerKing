using API0093BK.DTOs.Common;
using API0093BK.DTOs.User;
using API0093BK.Helpers;
using API0093BK.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API0093BK.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/[controller]")]
    [Authorize(Roles = "Administrator")]
    [Produces("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly IUserManagementService _userManagementService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            IUserManagementService userManagementService,
            ILogger<UsersController> logger)
        {
            _userManagementService = userManagementService;
            _logger = logger;
        }

        /// <summary>
        /// Получение всех пользователей (только для администратора)
        /// </summary>
        /// <returns>Список всех активных пользователей</returns>
        /// <response code="200">Список пользователей</response>
        /// <response code="401">Не авторизован</response>
        /// <response code="403">Недостаточно прав</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userManagementService.GetAllUsersAsync();

                if (users == null || !users.Any())
                {
                    return Ok(new ApiResponse<IEnumerable<UserDto>>(
                        Enumerable.Empty<UserDto>(),
                        "Пользователи не найдены"
                    ));
                }

                return Ok(new ApiResponse<IEnumerable<UserDto>>(users, "Пользователи получены успешно"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении списка пользователей");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Произошла ошибка при получении пользователей"
                });
            }
        }

        /// <summary>
        /// Получение пользователя по ID
        /// </summary>
        /// <param name="id">ID пользователя</param>
        /// <returns>Данные пользователя</returns>
        /// <response code="200">Пользователь найден</response>
        /// <response code="404">Пользователь не найден</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUser(int id)
        {
            try
            {
                var user = await _userManagementService.GetUserByIdAsync(id);
                return Ok(new ApiResponse<UserDto>(user, "Пользователь получен успешно"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении пользователя {UserId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Произошла ошибка при получении пользователя"
                });
            }
        }

        /// <summary>
        /// Создание нового пользователя
        /// </summary>
        /// <param name="userCreateDto">Данные для создания пользователя</param>
        /// <returns>Созданный пользователь</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDto userCreateDto)
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

                var adminId = User.GetUserId();
                var user = await _userManagementService.CreateUserAsync(userCreateDto, adminId);

                return CreatedAtAction(
                    nameof(GetUser),
                    new { id = user.Id },
                    new ApiResponse<UserDto>(user, "Пользователь создан успешно")
                );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании пользователя");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Произошла ошибка при создании пользователя"
                });
            }
        }

        /// <summary>
        /// Обновление данных пользователя
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateDto userUpdateDto)
        {
            try
            {
                var adminId = User.GetUserId();
                var user = await _userManagementService.UpdateUserAsync(id, userUpdateDto, adminId);
                return Ok(new ApiResponse<UserDto>(user, "Пользователь обновлен успешно"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении пользователя {UserId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Произошла ошибка при обновлении пользователя"
                });
            }
        }

        /// <summary>
        /// Удаление (деактивация) пользователя
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var adminId = User.GetUserId();
                await _userManagementService.DeleteUserAsync(id, adminId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении пользователя {UserId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Произошла ошибка при удалении пользователя"
                });
            }
        }

        /// <summary>
        /// Сброс пароля пользователя
        /// </summary>
        [HttpPost("{id}/reset-password")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ResetPassword(int id, [FromBody] string newPassword)
        {
            try
            {
                var adminId = User.GetUserId();
                await _userManagementService.ResetPasswordAsync(id, newPassword, adminId);
                return Ok(new ApiResponse<object>(null, "Пароль успешно сброшен"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при сбросе пароля пользователя {UserId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Произошла ошибка при сбросе пароля"
                });
            }
        }

        /// <summary>
        /// Отправка пользователя на обучение
        /// </summary>
        [HttpPost("{id}/send-to-training")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SendToTraining(int id)
        {
            try
            {
                await _userManagementService.SendTrainingAsync(id);
                return Ok(new ApiResponse<object>(null, "Пользователь отправлен на обучение"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при отправке пользователя {UserId} на обучение", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Произошла ошибка при отправке на обучение"
                });
            }
        }
    }
}