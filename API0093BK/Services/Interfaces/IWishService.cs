using API0093BK.DTOs.Wish;

namespace API0093BK.Services.Interfaces
{
    public interface IWishService
    {
        Task<IEnumerable<WishDto>> GetUserWishesAsync(int userId);
        Task<IEnumerable<WishDto>> GetAllWishesAsync();
        Task<IEnumerable<WishDto>> GetPendingWishesAsync();
        Task<IEnumerable<WishDto>> GetWishesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<WishDto> CreateWishAsync(int userId, WishCreateDto wishDto);
        Task<bool> DeleteWishAsync(int wishId, int userId);
        Task<WishDto> UpdateWishStatusAsync(int wishId, string status, int managerId);
        Task<bool> HasWishForDateAsync(int userId, DateTime date);
    }
}