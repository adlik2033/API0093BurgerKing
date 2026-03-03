using API0093BK.Models;

namespace API0093BK.Repositories.Interfaces
{
    public interface IScheduleRepository : IRepository<Schedule>
    {
        Task<IEnumerable<Schedule>> GetSchedulesByUserAsync(int userId);
        Task<IEnumerable<Schedule>> GetSchedulesByWeekAsync(DateTime weekStart);
        Task<IEnumerable<Schedule>> GetUserSchedulesByDateRangeAsync(int userId, DateTime startDate, DateTime endDate);
        Task<IEnumerable<Schedule>> GetFinalSchedulesByWeekAsync(DateTime weekStart);
    }
}