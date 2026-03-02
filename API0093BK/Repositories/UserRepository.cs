using API0093BK.Data;
using API0093BK.Models;
using API0093BK.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API0093BK.Repositories
{
    /// <summary>
    /// Репозиторий для работы с пользователями
    /// </summary>
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(API0093DbContext context) : base(context)
        {
        }

        /// <summary>
        /// Получение пользователя по имени
        /// </summary>
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        /// <summary>
        /// Получение пользователя по email
        /// </summary>
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        /// <summary>
        /// Получение пользователей по роли
        /// </summary>
        public async Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role)
        {
            return await _dbSet
                .Where(u => u.Role == role && u.IsActive)
                .ToListAsync();
        }

        /// <summary>
        /// Получение пользователя по ID из портала
        /// </summary>
        public async Task<User?> GetUserByPortalIdAsync(int portalId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.PortalEmployeeId == portalId);
        }
    }
}