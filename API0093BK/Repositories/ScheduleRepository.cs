using API0093BK.Data;
using API0093BK.Models;
using API0093BK.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API0093BK.Repositories
{
    /// <summary>
    /// Репозиторий для работы с расписанием
    /// </summary>
    public class ScheduleRepository : Repository<Schedule>, IScheduleRepository
    {
        public ScheduleRepository(API0093DbContext context) : base(context)
        {
        }

        /// <summary>
        /// Получение расписания пользователя
        /// </summary>
        public async Task<IEnumerable<Schedule>> GetSchedulesByUserAsync(int userId)
        {
            return await _dbSet
                .Where(s => s.UserId == userId)
                .Include(s => s.User)
                .Include(s => s.Approver)
                .OrderBy(s => s.WorkDate)
                .ToListAsync();
        }

        /// <summary>
        /// Получение расписания на неделю
        /// </summary>
        public async Task<IEnumerable<Schedule>> GetSchedulesByWeekAsync(DateTime weekStart)
        {
            var weekEnd = weekStart.AddDays(7);
            return await _dbSet
                .Where(s => s.WorkDate >= weekStart && s.WorkDate < weekEnd)
                .Include(s => s.User)
                .Include(s => s.Approver)
                .OrderBy(s => s.WorkDate)
                .ThenBy(s => s.User!.LastName)
                .ToListAsync();
        }

        /// <summary>
        /// Получение расписания пользователя за период
        /// </summary>
        public async Task<IEnumerable<Schedule>> GetUserSchedulesByDateRangeAsync(int userId, DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(s => s.UserId == userId && s.WorkDate >= startDate && s.WorkDate <= endDate)
                .Include(s => s.User)
                .Include(s => s.Approver)
                .OrderBy(s => s.WorkDate)
                .ToListAsync();
        }
    }
}