using API0093BK.DTOs.Common;
using API0093BK.DTOs.Schedule;
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
    public class SchedulesController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;
        private readonly ILogger<SchedulesController> _logger;

        public SchedulesController(IScheduleService scheduleService, ILogger<SchedulesController> logger)
        {
            _scheduleService = scheduleService;
            _logger = logger;
        }

        /// <summary>
        /// Получение своего расписания на неделю (для сотрудника)
        /// </summary>
        /// <param name="weekStart">Дата начала недели</param>
        [HttpGet("my")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ScheduleDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMySchedule([FromQuery] DateTime weekStart)
        {
            try
            {
                var userId = User.GetUserId();
                var schedules = await _scheduleService.GetUserScheduleAsync(userId, weekStart);

                return Ok(new ApiResponse<IEnumerable<ScheduleDto>>(schedules,
                    schedules.Any() ? "Расписание получено" : "Расписание не найдено"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении расписания пользователя");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Произошла ошибка при получении расписания"
                });
            }
        }

        /// <summary>
        /// Получение расписания на неделю для всех сотрудников (для менеджера и администратора)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Administrator,Manager")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ScheduleDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetWeekSchedule([FromQuery] DateTime weekStart)
        {
            try
            {
                var schedules = await _scheduleService.GetWeekScheduleAsync(weekStart);

                return Ok(new ApiResponse<IEnumerable<ScheduleDto>>(schedules,
                    schedules.Any() ? "Расписание получено" : "Расписание не найдено"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении расписания на неделю");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Произошла ошибка при получении расписания"
                });
            }
        }

        /// <summary>
        /// Создание или обновление записи в расписании (для менеджера и администратора)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Administrator,Manager")]
        [ProducesResponseType(typeof(ApiResponse<ScheduleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateOrUpdateSchedule([FromBody] ScheduleCreateDto scheduleDto)
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

                var managerId = User.GetUserId();
                var schedule = await _scheduleService.CreateOrUpdateScheduleAsync(scheduleDto, managerId);

                return Ok(new ApiResponse<ScheduleDto>(schedule, "Запись в расписании сохранена"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при сохранении расписания");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Произошла ошибка при сохранении расписания"
                });
            }
        }

        /// <summary>
        /// Утверждение расписания на неделю (для менеджера и администратора)
        /// </summary>
        [HttpPost("approve-week")]
        [Authorize(Roles = "Administrator,Manager")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ApproveWeekSchedule([FromQuery] DateTime weekStart)
        {
            try
            {
                var managerId = User.GetUserId();
                await _scheduleService.ApproveWeekScheduleAsync(weekStart, managerId);

                return Ok(new ApiResponse<object>(null, "Расписание утверждено успешно"));
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
                _logger.LogError(ex, "Ошибка при утверждении расписания");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Произошла ошибка при утверждении расписания"
                });
            }
        }
    }
}