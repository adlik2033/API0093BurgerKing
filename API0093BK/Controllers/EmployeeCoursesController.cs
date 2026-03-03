using API0093BK.DTOs.Common;
using API0093BK.DTOs.EmployeeCourse;
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
    public class EmployeeCoursesController : ControllerBase
    {
        private readonly IEmployeeCourseService _employeeCourseService;
        private readonly ILogger<EmployeeCoursesController> _logger;

        public EmployeeCoursesController(
            IEmployeeCourseService employeeCourseService,
            ILogger<EmployeeCoursesController> logger)
        {
            _employeeCourseService = employeeCourseService;
            _logger = logger;
        }

        /// <summary>
        /// Получение своих курсов (для сотрудника)
        /// </summary>
        [HttpGet("my")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<EmployeeCourseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyCourses()
        {
            try
            {
                var userId = User.GetUserId();
                var courses = await _employeeCourseService.GetUserCoursesAsync(userId);

                return Ok(new ApiResponse<IEnumerable<EmployeeCourseDto>>(courses,
                    courses.Any() ? "Курсы получены" : "У вас нет курсов"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении курсов пользователя");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Произошла ошибка при получении курсов"
                });
            }
        }

        /// <summary>
        /// Получение курсов сотрудника (для менеджера)
        /// </summary>
        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Administrator,Manager")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<EmployeeCourseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserCourses(int userId)
        {
            try
            {
                var courses = await _employeeCourseService.GetUserCoursesAsync(userId);

                return Ok(new ApiResponse<IEnumerable<EmployeeCourseDto>>(courses,
                    courses.Any() ? "Курсы получены" : "У пользователя нет курсов"));
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
                _logger.LogError(ex, "Ошибка при получении курсов пользователя {UserId}", userId);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Произошла ошибка при получении курсов"
                });
            }
        }

        /// <summary>
        /// Получение всех курсов всех сотрудников (для администратора)
        /// </summary>
        [HttpGet("all")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<EmployeeCourseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllUserCourses()
        {
            try
            {
                var courses = await _employeeCourseService.GetAllUserCoursesAsync();

                return Ok(new ApiResponse<IEnumerable<EmployeeCourseDto>>(courses,
                    courses.Any() ? "Курсы получены" : "Курсы не найдены"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении всех курсов");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Произошла ошибка при получении курсов"
                });
            }
        }

        /// <summary>
        /// Получение сотрудников с невыполненными обязательными курсами
        /// </summary>
        [HttpGet("incomplete-mandatory")]
        [Authorize(Roles = "Administrator,Manager")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<EmployeeCourseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetIncompleteMandatoryCourses()
        {
            try
            {
                var courses = await _employeeCourseService.GetIncompleteMandatoryCoursesAsync();

                return Ok(new ApiResponse<IEnumerable<EmployeeCourseDto>>(courses,
                    courses.Any() ? "Данные получены" : "Нет невыполненных обязательных курсов"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении невыполненных обязательных курсов");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Произошла ошибка при получении данных"
                });
            }
        }

        /// <summary>
        /// Получение истекающих курсов
        /// </summary>
        [HttpGet("expiring")]
        [Authorize(Roles = "Administrator,Manager")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<EmployeeCourseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetExpiringCourses([FromQuery] int daysThreshold = 30)
        {
            try
            {
                var courses = await _employeeCourseService.GetExpiringCoursesAsync(daysThreshold);

                return Ok(new ApiResponse<IEnumerable<EmployeeCourseDto>>(courses,
                    courses.Any() ? "Данные получены" : "Нет истекающих курсов"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении истекающих курсов");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Произошла ошибка при получении данных"
                });
            }
        }

        /// <summary>
        /// Обновление статуса курса (для сотрудника или менеджера)
        /// </summary>
        [HttpPut("status")]
        [ProducesResponseType(typeof(ApiResponse<EmployeeCourseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCourseStatus([FromBody] UpdateCourseStatusDto statusDto)
        {
            try
            {
                var userId = User.GetUserId();
                var userRole = User.GetUserRole();

                // Проверка прав: сотрудник может обновлять только свои курсы
                if (userRole != "Administrator" && userRole != "Manager")
                {
                    // Для сотрудника проверяем, что это его курс
                    // Здесь нужна дополнительная логика
                }

                var course = await _employeeCourseService.UpdateCourseStatusAsync(userId, statusDto);

                return Ok(new ApiResponse<EmployeeCourseDto>(course, "Статус курса обновлен"));
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
                _logger.LogError(ex, "Ошибка при обновлении статуса курса");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Произошла ошибка при обновлении статуса"
                });
            }
        }

        /// <summary>
        /// Назначение курса сотруднику (для менеджера)
        /// </summary>
        [HttpPost("assign")]
        [Authorize(Roles = "Administrator,Manager")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AssignCourseToUser(int userId, int courseId)
        {
            try
            {
                var result = await _employeeCourseService.AssignCourseToUserAsync(userId, courseId);

                return Ok(new ApiResponse<bool>(result, "Курс назначен пользователю"));
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
                _logger.LogError(ex, "Ошибка при назначении курса пользователю {UserId}", userId);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Произошла ошибка при назначении курса"
                });
            }
        }

        /// <summary>
        /// Синхронизация курсов сотрудника с порталом
        /// </summary>
        [HttpPost("sync/{userId}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SyncEmployeeCourses(int userId, [FromBody] IEnumerable<UpdateCourseStatusDto> externalCourses)
        {
            try
            {
                await _employeeCourseService.SyncEmployeeCoursesAsync(userId, externalCourses);
                return Ok(new ApiResponse<object>(null, "Синхронизация курсов завершена"));
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
                _logger.LogError(ex, "Ошибка при синхронизации курсов пользователя {UserId}", userId);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Произошла ошибка при синхронизации"
                });
            }
        }
    }
}