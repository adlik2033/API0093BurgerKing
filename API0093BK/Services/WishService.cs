using API0093BK.DTOs.Wish;
using API0093BK.Models;
using API0093BK.Repositories.Interfaces;
using API0093BK.Services.Interfaces;

namespace API0093BK.Services
{
    /// <summary>
    /// Реализация сервиса пожеланий
    /// </summary>
    public class WishService : IWishService
    {
        private readonly IWishRepository _wishRepository;
        private readonly IUserRepository _userRepository;

        public WishService(IWishRepository wishRepository, IUserRepository userRepository)
        {
            _wishRepository = wishRepository;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Получение пожеланий конкретного пользователя
        /// </summary>
        public async Task<IEnumerable<WishDto>> GetUserWishesAsync(int userId)
        {
            var wishes = await _wishRepository.GetWishesByUserAsync(userId);

            if (!wishes.Any())
            {
                return Enumerable.Empty<WishDto>();
            }

            return wishes.Select(MapToDto);
        }

        /// <summary>
        /// Получение всех пожеланий (для менеджера)
        /// </summary>
        public async Task<IEnumerable<WishDto>> GetAllWishesAsync()
        {
            var wishes = await _wishRepository.GetAllWithUsersAsync();

            if (!wishes.Any())
            {
                return Enumerable.Empty<WishDto>();
            }

            return wishes.Select(MapToDto);
        }

        /// <summary>
        /// Создание нового пожелания
        /// </summary>
        public async Task<WishDto> CreateWishAsync(int userId, WishCreateDto wishDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("Пользователь не найден");

            // Проверка существующего пожелания на эту дату
            var existingWishes = await _wishRepository.FindAsync(w =>
                w.UserId == userId && w.WishDate.Date == wishDto.WishDate.Date);

            if (existingWishes.Any())
                throw new InvalidOperationException("У вас уже есть пожелание на эту дату");

            var wish = new Wish
            {
                UserId = userId,
                WishDate = wishDto.WishDate,
                Type = Enum.Parse<WishType>(wishDto.Type),
                Comment = wishDto.Comment,
                Status = WishStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            var createdWish = await _wishRepository.AddAsync(wish);
            return MapToDto(createdWish);
        }

        /// <summary>
        /// Удаление пожелания
        /// </summary>
        public async Task<bool> DeleteWishAsync(int wishId, int userId)
        {
            var wish = await _wishRepository.GetByIdAsync(wishId);

            if (wish == null)
                throw new KeyNotFoundException("Пожелание не найдено");

            // Проверка прав на удаление
            if (wish.UserId != userId)
                throw new UnauthorizedAccessException("Вы можете удалять только свои пожелания");

            // Проверка статуса пожелания
            if (wish.Status != WishStatus.Pending)
                throw new InvalidOperationException("Нельзя удалить пожелание, которое уже обработано");

            await _wishRepository.DeleteAsync(wish);
            return true;
        }

        /// <summary>
        /// Обновление статуса пожелания (для менеджера)
        /// </summary>
        public async Task<WishDto> UpdateWishStatusAsync(int wishId, string status, int managerId)
        {
            var wish = await _wishRepository.GetByIdAsync(wishId);

            if (wish == null)
                throw new KeyNotFoundException("Пожелание не найдено");

            wish.Status = Enum.Parse<WishStatus>(status);
            wish.UpdatedAt = DateTime.UtcNow;

            await _wishRepository.UpdateAsync(wish);
            return MapToDto(wish);
        }

        /// <summary>
        /// Преобразование модели Wish в DTO
        /// </summary>
        private WishDto MapToDto(Wish wish)
        {
            return new WishDto
            {
                Id = wish.Id,
                UserId = wish.UserId,
                UserName = $"{wish.User?.FirstName} {wish.User?.LastName}",
                WishDate = wish.WishDate,
                Type = wish.Type.ToString(),
                Comment = wish.Comment,
                Status = wish.Status.ToString(),
                CreatedAt = wish.CreatedAt
            };
        }
    }
}