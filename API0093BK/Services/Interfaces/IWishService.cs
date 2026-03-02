using API0093BK.DTOs.Wish;

namespace API0093BK.Services.Interfaces
{
    /// <summary>
    /// Сервис для работы с пожеланиями
    /// </summary>
    public interface IWishService
    {
        Task<IEnumerable<WishDto>> GetUserWishesAsync(int userId);
        Task<IEnumerable<WishDto>> GetAllWishesAsync();
        Task<WishDto> CreateWishAsync(int userId, WishCreateDto wishDto);
        Task<bool> DeleteWishAsync(int wishId, int userId);
        Task<WishDto> UpdateWishStatusAsync(int wishId, string status, int managerId);
    }
}