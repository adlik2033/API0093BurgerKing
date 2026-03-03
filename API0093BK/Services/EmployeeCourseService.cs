using API0093BK.DTOs.EmployeeCourse;
using API0093BK.Models;
using API0093BK.Repositories.Interfaces;
using API0093BK.Services.Interfaces;

namespace API0093BK.Services
{
    public class EmployeeCourseService : IEmployeeCourseService
    {
        private readonly IEmployeeCourseRepository _employeeCourseRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly ILogger<EmployeeCourseService> _logger;

        public EmployeeCourseService(
            IEmployeeCourseRepository employeeCourseRepository,
            IUserRepository userRepository,
            ICourseRepository courseRepository,
            ILogger<EmployeeCourseService> logger)
        {
            _employeeCourseRepository = employeeCourseRepository;
            _userRepository = userRepository;
            _courseRepository = courseRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<EmployeeCourseDto>> GetUserCoursesAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"Пользователь с ID {userId} не найден");

            var courses = await _employeeCourseRepository.GetUserCoursesAsync(userId);
            return courses.Select(MapToDto);
        }

        public async Task<IEnumerable<EmployeeCourseDto>> GetAllUserCoursesAsync()
        {
            var courses = await _employeeCourseRepository.GetAllAsync();
            var result = new List<EmployeeCourseDto>();

            foreach (var course in courses)
            {
                result.Add(await MapToDtoWithDetails(course));
            }

            return result;
        }

        public async Task<EmployeeCourseDto> UpdateCourseStatusAsync(int userId, UpdateCourseStatusDto statusDto)
        {
            if (!CourseStatus.All.Contains(statusDto.Status))
                throw new ArgumentException($"Недопустимый статус: {statusDto.Status}");

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"Пользователь с ID {userId} не найден");

            var course = await _courseRepository.GetByIdAsync(statusDto.CourseId);
            if (course == null)
                throw new KeyNotFoundException($"Курс с ID {statusDto.CourseId} не найден");

            var employeeCourse = await _employeeCourseRepository.GetUserCourseAsync(userId, statusDto.CourseId);

            if (employeeCourse == null)
            {
                employeeCourse = new EmployeeCourse
                {
                    UserId = userId,
                    CourseId = statusDto.CourseId,
                    Status = statusDto.Status,
                    CompletionDate = statusDto.CompletionDate,
                    ExpiryDate = statusDto.ExpiryDate,
                    LastSyncDate = DateTime.UtcNow
                };
                employeeCourse = await _employeeCourseRepository.AddAsync(employeeCourse);
                _logger.LogInformation("Курс {CourseId} назначен пользователю {UserId}", statusDto.CourseId, userId);
            }
            else
            {
                var oldStatus = employeeCourse.Status;
                employeeCourse.Status = statusDto.Status;
                employeeCourse.CompletionDate = statusDto.CompletionDate;
                employeeCourse.ExpiryDate = statusDto.ExpiryDate;
                employeeCourse.LastSyncDate = DateTime.UtcNow;

                await _employeeCourseRepository.UpdateAsync(employeeCourse);
                _logger.LogInformation("Статус курса {CourseId} для пользователя {UserId} изменен с {OldStatus} на {NewStatus}",
                    statusDto.CourseId, userId, oldStatus, statusDto.Status);
            }

            return await MapToDtoWithDetails(employeeCourse);
        }

        public async Task<IEnumerable<EmployeeCourseDto>> GetIncompleteMandatoryCoursesAsync()
        {
            var courses = await _employeeCourseRepository.GetIncompleteMandatoryCoursesAsync();
            var result = new List<EmployeeCourseDto>();

            foreach (var course in courses)
            {
                result.Add(await MapToDtoWithDetails(course));
            }

            return result;
        }

        public async Task<IEnumerable<EmployeeCourseDto>> GetExpiringCoursesAsync(int daysThreshold)
        {
            var courses = await _employeeCourseRepository.GetEmployeesWithExpiringCoursesAsync(daysThreshold);
            var result = new List<EmployeeCourseDto>();

            foreach (var course in courses)
            {
                result.Add(await MapToDtoWithDetails(course));
            }

            return result;
        }

        public async Task<bool> AssignCourseToUserAsync(int userId, int courseId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"Пользователь с ID {userId} не найден");

            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null)
                throw new KeyNotFoundException($"Курс с ID {courseId} не найден");

            var existing = await _employeeCourseRepository.GetUserCourseAsync(userId, courseId);
            if (existing != null)
                throw new InvalidOperationException($"Курс уже назначен пользователю");

            var employeeCourse = new EmployeeCourse
            {
                UserId = userId,
                CourseId = courseId,
                Status = CourseStatus.NotStarted,
                LastSyncDate = DateTime.UtcNow
            };

            await _employeeCourseRepository.AddAsync(employeeCourse);
            _logger.LogInformation("Курс {CourseId} назначен пользователю {UserId}", courseId, userId);

            return true;
        }

        public async Task SyncEmployeeCoursesAsync(int userId, IEnumerable<UpdateCourseStatusDto> externalCourses)
        {
            foreach (var externalCourse in externalCourses)
            {
                try
                {
                    await UpdateCourseStatusAsync(userId, externalCourse);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при синхронизации курса пользователя {UserId}", userId);
                }
            }

            // Обновляем дату последней синхронизации пользователя
            var user = await _userRepository.GetByIdAsync(userId);
            if (user != null)
            {
                user.LastSyncDate = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user);
            }

            _logger.LogInformation("Синхронизация курсов для пользователя {UserId} завершена", userId);
        }

        private EmployeeCourseDto MapToDto(EmployeeCourse employeeCourse)
        {
            return new EmployeeCourseDto
            {
                Id = employeeCourse.Id,
                UserId = employeeCourse.UserId,
                UserName = employeeCourse.User?.FullName ?? "Неизвестно",
                EmployeeNumber = employeeCourse.User?.EmployeeNumber ?? "Неизвестно",
                CourseId = employeeCourse.CourseId,
                CourseTitle = employeeCourse.Course?.Title ?? "Неизвестно",
                CourseExternalId = employeeCourse.Course?.ExternalId ?? "Неизвестно",
                IsMandatory = employeeCourse.Course?.IsMandatory ?? false,
                Status = employeeCourse.Status,
                CompletionDate = employeeCourse.CompletionDate,
                ExpiryDate = employeeCourse.ExpiryDate,
                LastSyncDate = employeeCourse.LastSyncDate
            };
        }

        private async Task<EmployeeCourseDto> MapToDtoWithDetails(EmployeeCourse employeeCourse)
        {
            // Загружаем связанные данные, если они еще не загружены
            if (employeeCourse.User == null)
                employeeCourse.User = await _userRepository.GetByIdAsync(employeeCourse.UserId);

            if (employeeCourse.Course == null)
                employeeCourse.Course = await _courseRepository.GetByIdAsync(employeeCourse.CourseId);

            return MapToDto(employeeCourse);
        }
    }
}