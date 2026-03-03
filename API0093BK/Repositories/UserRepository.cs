using API0093BK.Data;
using API0093BK.Models;
using API0093BK.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API0093BK.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(API0093DbContext context) : base(context)
        {
        }

        public async Task<User?> GetUserByEmployeeNumberAsync(string employeeNumber)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.EmployeeNumber == employeeNumber);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(string role)
        {
            return await _dbSet
                .Where(u => u.Role == role && u.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersForSyncAsync(DateTime? lastSync)
        {
            if (lastSync.HasValue)
            {
                return await _dbSet
                    .Where(u => u.LastSyncDate == null || u.LastSyncDate < lastSync)
                    .ToListAsync();
            }
            return await _dbSet
                .Where(u => u.IsActive)
                .ToListAsync();
        }

        public async Task<User?> GetUserWithCoursesAsync(int userId)
        {
            return await _dbSet
                .Include(u => u.EmployeeCourses)
                    .ThenInclude(ec => ec.Course)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}