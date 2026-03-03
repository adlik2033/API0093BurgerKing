using API0093BK.Models;

namespace API0093BK.Repositories.Interfaces
{
    public interface ICourseRepository : IRepository<Course>
    {
        Task<Course?> GetByExternalIdAsync(string externalId);
        Task<IEnumerable<Course>> GetMandatoryCoursesAsync();
        Task<IEnumerable<Course>> GetCoursesByLastSyncAsync(DateTime since);
    }
}