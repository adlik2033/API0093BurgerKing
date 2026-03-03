using API0093BK.Models;

namespace API0093BK.Repositories.Interfaces
{
    public interface IWishRepository : IRepository<Wish>
    {
        Task<IEnumerable<Wish>> GetWishesByUserAsync(int userId);
        Task<IEnumerable<Wish>> GetWishesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Wish>> GetWishesByStatusAsync(string status);
        Task<IEnumerable<Wish>> GetAllWithUsersAsync();
        Task<IEnumerable<Wish>> GetPendingWishesAsync();
    }
}