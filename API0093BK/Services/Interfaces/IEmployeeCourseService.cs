using API0093BK.DTOs.EmployeeCourse;

namespace API0093BK.Services.Interfaces
{
    public interface IEmployeeCourseService
    {
        Task<IEnumerable<EmployeeCourseDto>> GetUserCoursesAsync(int userId);
        Task<IEnumerable<EmployeeCourseDto>> GetAllUserCoursesAsync();
        Task<EmployeeCourseDto> UpdateCourseStatusAsync(int userId, UpdateCourseStatusDto statusDto);
        Task<IEnumerable<EmployeeCourseDto>> GetIncompleteMandatoryCoursesAsync();
        Task<IEnumerable<EmployeeCourseDto>> GetExpiringCoursesAsync(int daysThreshold);
        Task<bool> AssignCourseToUserAsync(int userId, int courseId);
        Task SyncEmployeeCoursesAsync(int userId, IEnumerable<UpdateCourseStatusDto> externalCourses);
    }
}