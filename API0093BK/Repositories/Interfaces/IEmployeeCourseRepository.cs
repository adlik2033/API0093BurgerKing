using API0093BK.Models;

namespace API0093BK.Repositories.Interfaces
{
    public interface IEmployeeCourseRepository : IRepository<EmployeeCourse>
    {
        Task<IEnumerable<EmployeeCourse>> GetUserCoursesAsync(int userId);
        Task<IEnumerable<EmployeeCourse>> GetIncompleteMandatoryCoursesAsync();
        Task<IEnumerable<EmployeeCourse>> GetEmployeesWithExpiringCoursesAsync(int daysThreshold);
        Task<EmployeeCourse?> GetUserCourseAsync(int userId, int courseId);
        Task<IEnumerable<EmployeeCourse>> GetCoursesByStatusAsync(string status);
        Task<IEnumerable<EmployeeCourse>> GetExpiredCoursesAsync();
    }
}