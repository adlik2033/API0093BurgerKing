using API0093BK.Models;

namespace API0093BK.Repositories.Interfaces
{
    /// <summary>
    /// Интерфейс репозитория пожеланий
    /// </summary>
    public interface IWishRepository : IRepository<Wish>
    {
        Task<IEnumerable<Wish>> GetWishesByUserAsync(int userId);
        Task<IEnumerable<Wish>> GetWishesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Wish>> GetAllWithUsersAsync();
    }
}