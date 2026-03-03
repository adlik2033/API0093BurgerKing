using API0093BK.DTOs.Course;
using API0093BK.Models;
using API0093BK.Repositories.Interfaces;
using API0093BK.Services.Interfaces;

namespace API0093BK.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ILogger<CourseService> _logger;

        public CourseService(ICourseRepository courseRepository, ILogger<CourseService> logger)
        {
            _courseRepository = courseRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<CourseDto>> GetAllCoursesAsync()
        {
            var courses = await _courseRepository.GetAllAsync();
            return courses.Select(MapToDto);
        }

        public async Task<CourseDto> GetCourseByIdAsync(int id)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null)
                throw new KeyNotFoundException($"Курс с ID {id} не найден");

            return MapToDto(course);
        }

        public async Task<CourseDto> GetCourseByExternalIdAsync(string externalId)
        {
            var course = await _courseRepository.GetByExternalIdAsync(externalId);
            if (course == null)
                throw new KeyNotFoundException($"Курс с ExternalId {externalId} не найден");

            return MapToDto(course);
        }

        public async Task<CourseDto> CreateCourseAsync(CourseCreateDto courseDto)
        {
            var existing = await _courseRepository.GetByExternalIdAsync(courseDto.ExternalId);
            if (existing != null)
                throw new InvalidOperationException($"Курс с ExternalId {courseDto.ExternalId} уже существует");

            var course = new Course
            {
                ExternalId = courseDto.ExternalId,
                Title = courseDto.Title,
                IsMandatory = courseDto.IsMandatory,
                LastSyncDate = DateTime.UtcNow
            };

            var created = await _courseRepository.AddAsync(course);
            _logger.LogInformation("Создан новый курс: {CourseTitle} (ID: {CourseId})", created.Title, created.Id);

            return MapToDto(created);
        }

        public async Task<CourseDto> UpdateCourseAsync(int id, CourseCreateDto courseDto)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null)
                throw new KeyNotFoundException($"Курс с ID {id} не найден");

            course.Title = courseDto.Title;
            course.IsMandatory = courseDto.IsMandatory;
            course.LastSyncDate = DateTime.UtcNow;

            await _courseRepository.UpdateAsync(course);
            _logger.LogInformation("Обновлен курс: {CourseTitle} (ID: {CourseId})", course.Title, course.Id);

            return MapToDto(course);
        }

        public async Task<bool> DeleteCourseAsync(int id)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            if (course == null)
                throw new KeyNotFoundException($"Курс с ID {id} не найден");

            await _courseRepository.DeleteAsync(course);
            _logger.LogInformation("Удален курс: {CourseTitle} (ID: {CourseId})", course.Title, course.Id);

            return true;
        }

        public async Task<IEnumerable<CourseDto>> GetMandatoryCoursesAsync()
        {
            var courses = await _courseRepository.GetMandatoryCoursesAsync();
            return courses.Select(MapToDto);
        }

        public async Task SyncCoursesAsync(IEnumerable<CourseCreateDto> externalCourses)
        {
            foreach (var externalCourse in externalCourses)
            {
                try
                {
                    var existing = await _courseRepository.GetByExternalIdAsync(externalCourse.ExternalId);

                    if (existing == null)
                    {
                        await CreateCourseAsync(externalCourse);
                    }
                    else
                    {
                        await UpdateCourseAsync(existing.Id, externalCourse);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при синхронизации курса {ExternalId}", externalCourse.ExternalId);
                }
            }

            _logger.LogInformation("Синхронизация курсов завершена");
        }

        private CourseDto MapToDto(Course course)
        {
            return new CourseDto
            {
                Id = course.Id,
                ExternalId = course.ExternalId,
                Title = course.Title,
                IsMandatory = course.IsMandatory,
                LastSyncDate = course.LastSyncDate
            };
        }
    }
}