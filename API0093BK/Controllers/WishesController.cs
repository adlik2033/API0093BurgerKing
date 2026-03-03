using API0093BK.DTOs.Common;
using API0093BK.DTOs.Wish;
using API0093BK.Helpers;
using API0093BK.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API0093BK.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class WishesController : ControllerBase
    {
        private readonly IWishService _wishService;
        private readonly ILogger<WishesController> _logger;

        public WishesController(IWishService wishService, ILogger<WishesController> logger)
        {
            _wishService = wishService;
            _logger = logger;
        }

        /// <summary>
        /// Получение своих пожеланий
        /// </summary>
        [HttpGet("my")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<WishDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyWishes()
        {
            try
            {
                var userId = User.GetUserId();
                var wishes = await _wishService.GetUserWishesAsync(userId);

                return Ok(new ApiResponse<IEnumerable<WishDto>>(wishes,
                    wishes.Any() ? "Пожелания получены" : "У вас нет пожеланий"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении пожеланий");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Произошла ошибка при получении пожеланий"
                });
            }
        }

        /// <summary>
        /// Получение всех пожеланий (для менеджера и администратора)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Administrator,Manager")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<WishDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllWishes([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                IEnumerable<WishDto> wishes;

                if (startDate.HasValue && endDate.HasValue)
                {
                    wishes = await _wishService.GetWishesByDateRangeAsync(startDate.Value, endDate.Value);
                }
                else
                {
                    wishes = await _wishService.GetAllWishesAsync();
                }

                return Ok(new ApiResponse<IEnumerable<WishDto>>(wishes,
                    wishes.Any() ? "Пожелания получены" : "Пожелания не найдены"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении всех пожеланий");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Произошла ошибка при получении пожеланий"
                });
            }
        }

        /// <summary>
        /// Получение ожидающих пожеланий (для менеджера)
        /// </summary>
        [HttpGet("pending")]
        [Authorize(Roles = "Administrator,Manager")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<WishDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPendingWishes()
        {
            try
            {
                var wishes = await _wishService.GetPendingWishesAsync();

                return Ok(new ApiResponse<IEnumerable<WishDto>>(wishes,
                    wishes.Any() ? "Ожидающие пожелания получены" : "Нет ожидающих пожеланий"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении ожидающих пожеланий");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Произошла ошибка при получении пожеланий"
                });
            }
        }

        /// <summary>
        /// Создание нового пожелания
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<WishDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateWish([FromBody] WishCreateDto wishDto)
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

                var userId = User.GetUserId();
                var wish = await _wishService.CreateWishAsync(userId, wishDto);

                return Ok(new ApiResponse<WishDto>(wish, "Пожелание создано успешно"));
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
                _logger.LogError(ex, "Ошибка при создании пожелания");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Произошла ошибка при создании пожелания"
                });
            }
        }

        /// <summary>
        /// Удаление пожелания
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteWish(int id)
        {
            try
            {
                var userId = User.GetUserId();
                await _wishService.DeleteWishAsync(id, userId);

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
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new ApiResponse<object>
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
                _logger.LogError(ex, "Ошибка при удалении пожелания {WishId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Произошла ошибка при удалении пожелания"
                });
            }
        }

        /// <summary>
        /// Обновление статуса пожелания (для менеджера)
        /// </summary>
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Administrator,Manager")]
        [ProducesResponseType(typeof(ApiResponse<WishDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateWishStatus(int id, [FromBody] UpdateWishStatusDto statusDto)
        {
            try
            {
                var managerId = User.GetUserId();
                var wish = await _wishService.UpdateWishStatusAsync(id, statusDto.Status, managerId);

                return Ok(new ApiResponse<WishDto>(wish, "Статус пожелания обновлен"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении статуса пожелания {WishId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Произошла ошибка при обновлении статуса"
                });
            }
        }

        /// <summary>
        /// Проверка наличия пожелания на дату
        /// </summary>
        [HttpGet("check-date")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckWishForDate([FromQuery] DateTime date)
        {
            try
            {
                var userId = User.GetUserId();
                var hasWish = await _wishService.HasWishForDateAsync(userId, date);

                return Ok(new ApiResponse<bool>(hasWish, hasWish ? "Пожелание существует" : "Пожелание отсутствует"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при проверке пожелания на дату");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Произошла ошибка при проверке"
                });
            }
        }
    }
}