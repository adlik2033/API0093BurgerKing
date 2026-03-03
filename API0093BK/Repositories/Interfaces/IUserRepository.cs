using API0093BK.Models;

namespace API0093BK.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetUserByEmployeeNumberAsync(string employeeNumber);
        Task<User?> GetUserByEmailAsync(string email);
        Task<IEnumerable<User>> GetUsersByRoleAsync(string role);
        Task<IEnumerable<User>> GetUsersForSyncAsync(DateTime? lastSync);
        Task<User?> GetUserWithCoursesAsync(int userId);
    }
}