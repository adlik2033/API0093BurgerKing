using API0093BK.DTOs.Common;
using API0093BK.DTOs.Course;
using API0093BK.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API0093BK.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrator,Manager")]
    [Produces("application/json")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly ILogger<CoursesController> _logger;

        public CoursesController(ICourseService courseService, ILogger<CoursesController> logger)
        {
            _courseService = courseService;
            _logger = logger;
        }

        /// <summary>
        /// Получение всех курсов
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CourseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllCourses()
        {
            try
            {
                var courses = await _courseService.GetAllCoursesAsync();

                return Ok(new ApiResponse<IEnumerable<CourseDto>>(courses,
                    courses.Any() ? "Курсы получены" : "Курсы не найдены"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении курсов");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Произошла ошибка при получении курсов"
                });
            }
        }

        /// <summary>
        /// Получение курса по ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<CourseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCourse(int id)
        {
            try
            {
                var course = await _courseService.GetCourseByIdAsync(id);
                return Ok(new ApiResponse<CourseDto>(course, "Курс получен"));
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
                _logger.LogError(ex, "Ошибка при получении курса {CourseId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Произошла ошибка при получении курса"
                });
            }
        }

        /// <summary>
        /// Получение курса по ExternalId
        /// </summary>
        [HttpGet("external/{externalId}")]
        [ProducesResponseType(typeof(ApiResponse<CourseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCourseByExternalId(string externalId)
        {
            try
            {
                var course = await _courseService.GetCourseByExternalIdAsync(externalId);
                return Ok(new ApiResponse<CourseDto>(course, "Курс получен"));
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
                _logger.LogError(ex, "Ошибка при получении курса {ExternalId}", externalId);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Произошла ошибка при получении курса"
                });
            }
        }

        /// <summary>
        /// Получение обязательных курсов
        /// </summary>
        [HttpGet("mandatory")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CourseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMandatoryCourses()
        {
            try
            {
                var courses = await _courseService.GetMandatoryCoursesAsync();

                return Ok(new ApiResponse<IEnumerable<CourseDto>>(courses,
                    courses.Any() ? "Обязательные курсы получены" : "Обязательные курсы не найдены"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении обязательных курсов");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Произошла ошибка при получении обязательных курсов"
                });
            }
        }

        /// <summary>
        /// Создание нового курса
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(ApiResponse<CourseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateCourse([FromBody] CourseCreateDto courseDto)
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

                var course = await _courseService.CreateCourseAsync(courseDto);

                return CreatedAtAction(
                    nameof(GetCourse),
                    new { id = course.Id },
                    new ApiResponse<CourseDto>(course, "Курс создан успешно")
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
                _logger.LogError(ex, "Ошибка при создании курса");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Произошла ошибка при создании курса"
                });
            }
        }

        /// <summary>
        /// Обновление курса
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(ApiResponse<CourseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] CourseCreateDto courseDto)
        {
            try
            {
                var course = await _courseService.UpdateCourseAsync(id, courseDto);
                return Ok(new ApiResponse<CourseDto>(course, "Курс обновлен успешно"));
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
                _logger.LogError(ex, "Ошибка при обновлении курса {CourseId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Произошла ошибка при обновлении курса"
                });
            }
        }

        /// <summary>
        /// Удаление курса
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            try
            {
                await _courseService.DeleteCourseAsync(id);
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении курса {CourseId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Произошла ошибка при удалении курса"
                });
            }
        }

        /// <summary>
        /// Синхронизация курсов с порталом
        /// </summary>
        [HttpPost("sync")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SyncCourses([FromBody] IEnumerable<CourseCreateDto> externalCourses)
        {
            try
            {
                await _courseService.SyncCoursesAsync(externalCourses);
                return Ok(new ApiResponse<object>(null, "Синхронизация курсов завершена"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при синхронизации курсов");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Произошла ошибка при синхронизации курсов"
                });
            }
        }
    }
}