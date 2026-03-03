using API0093BK.Data;
using API0093BK.Models;
using API0093BK.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API0093BK.Repositories
{
    public class CourseRepository : Repository<Course>, ICourseRepository
    {
        public CourseRepository(API0093DbContext context) : base(context)
        {
        }

        public async Task<Course?> GetByExternalIdAsync(string externalId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.ExternalId == externalId);
        }

        public async Task<IEnumerable<Course>> GetMandatoryCoursesAsync()
        {
            return await _dbSet
                .Where(c => c.IsMandatory)
                .ToListAsync();
        }

        public async Task<IEnumerable<Course>> GetCoursesByLastSyncAsync(DateTime since)
        {
            return await _dbSet
                .Where(c => c.LastSyncDate == null || c.LastSyncDate < since)
                .ToListAsync();
        }
    }
}