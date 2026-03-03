using API0093BK.DTOs.Course;

namespace API0093BK.Services.Interfaces
{
    public interface ICourseService
    {
        Task<IEnumerable<CourseDto>> GetAllCoursesAsync();
        Task<CourseDto> GetCourseByIdAsync(int id);
        Task<CourseDto> GetCourseByExternalIdAsync(string externalId);
        Task<CourseDto> CreateCourseAsync(CourseCreateDto courseDto);
        Task<CourseDto> UpdateCourseAsync(int id, CourseCreateDto courseDto);
        Task<bool> DeleteCourseAsync(int id);
        Task<IEnumerable<CourseDto>> GetMandatoryCoursesAsync();
        Task SyncCoursesAsync(IEnumerable<CourseCreateDto> externalCourses);
    }
}