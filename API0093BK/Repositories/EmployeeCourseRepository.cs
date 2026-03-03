using API0093BK.Data;
using API0093BK.Models;
using API0093BK.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API0093BK.Repositories
{
    public class EmployeeCourseRepository : Repository<EmployeeCourse>, IEmployeeCourseRepository
    {
        public EmployeeCourseRepository(API0093DbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<EmployeeCourse>> GetUserCoursesAsync(int userId)
        {
            return await _dbSet
                .Where(ec => ec.UserId == userId)
                .Include(ec => ec.Course)
                .Include(ec => ec.User)
                .OrderBy(ec => ec.ExpiryDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<EmployeeCourse>> GetIncompleteMandatoryCoursesAsync()
        {
            return await _dbSet
                .Where(ec => ec.Course != null && ec.Course.IsMandatory &&
                       (ec.Status == CourseStatus.NotStarted || ec.Status == CourseStatus.InProgress))
                .Include(ec => ec.Course)
                .Include(ec => ec.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<EmployeeCourse>> GetEmployeesWithExpiringCoursesAsync(int daysThreshold)
        {
            var thresholdDate = DateTime.UtcNow.AddDays(daysThreshold);

            return await _dbSet
                .Where(ec => ec.ExpiryDate != null &&
                       ec.ExpiryDate <= thresholdDate &&
                       ec.ExpiryDate > DateTime.UtcNow)
                .Include(ec => ec.Course)
                .Include(ec => ec.User)
                .ToListAsync();
        }

        public async Task<EmployeeCourse?> GetUserCourseAsync(int userId, int courseId)
        {
            return await _dbSet
                .Include(ec => ec.Course)
                .FirstOrDefaultAsync(ec => ec.UserId == userId && ec.CourseId == courseId);
        }

        public async Task<IEnumerable<EmployeeCourse>> GetCoursesByStatusAsync(string status)
        {
            return await _dbSet
                .Where(ec => ec.Status == status)
                .Include(ec => ec.Course)
                .Include(ec => ec.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<EmployeeCourse>> GetExpiredCoursesAsync()
        {
            return await _dbSet
                .Where(ec => ec.ExpiryDate != null && ec.ExpiryDate < DateTime.UtcNow)
                .Include(ec => ec.Course)
                .Include(ec => ec.User)
                .ToListAsync();
        }
    }
}