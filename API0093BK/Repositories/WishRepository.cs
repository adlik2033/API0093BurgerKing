using API0093BK.Data;
using API0093BK.Models;
using API0093BK.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API0093BK.Repositories
{
    /// <summary>
    /// Репозиторий для работы с пожеланиями
    /// </summary>
    public class WishRepository : Repository<Wish>, IWishRepository
    {
        public WishRepository(API0093DbContext context) : base(context)
        {
        }

        /// <summary>
        /// Получение пожеланий пользователя
        /// </summary>
        public async Task<IEnumerable<Wish>> GetWishesByUserAsync(int userId)
        {
            return await _dbSet
                .Where(w => w.UserId == userId)
                .Include(w => w.User)
                .OrderByDescending(w => w.WishDate)
                .ToListAsync();
        }

        /// <summary>
        /// Получение пожеланий за период
        /// </summary>
        public async Task<IEnumerable<Wish>> GetWishesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(w => w.WishDate.Date >= startDate.Date && w.WishDate.Date <= endDate.Date)
                .Include(w => w.User)
                .OrderBy(w => w.WishDate)
                .ToListAsync();
        }

        /// <summary>
        /// Получение всех пожеланий с данными пользователей
        /// </summary>
        public async Task<IEnumerable<Wish>> GetAllWithUsersAsync()
        {
            return await _dbSet
                .Include(w => w.User)
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync();
        }
    }
}