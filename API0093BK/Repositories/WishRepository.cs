using API0093BK.Data;
using API0093BK.Models;
using API0093BK.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API0093BK.Repositories
{
    public class WishRepository : Repository<Wish>, IWishRepository
    {
        public WishRepository(API0093DbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Wish>> GetWishesByUserAsync(int userId)
        {
            return await _dbSet
                .Where(w => w.UserId == userId)
                .Include(w => w.User)
                .OrderByDescending(w => w.RequestedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Wish>> GetWishesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(w => w.RequestedDate >= startDate.Date && w.RequestedDate <= endDate.Date)
                .Include(w => w.User)
                .OrderBy(w => w.RequestedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Wish>> GetWishesByStatusAsync(string status)
        {
            return await _dbSet
                .Where(w => w.Status == status)
                .Include(w => w.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<Wish>> GetAllWithUsersAsync()
        {
            return await _dbSet
                .Include(w => w.User)
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Wish>> GetPendingWishesAsync()
        {
            return await _dbSet
                .Where(w => w.Status == WishStatus.Pending)
                .Include(w => w.User)
                .OrderBy(w => w.RequestedDate)
                .ToListAsync();
        }
    }
}