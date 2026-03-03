using API0093BK.DTOs.Wish;
using API0093BK.Models;
using API0093BK.Repositories.Interfaces;
using API0093BK.Services.Interfaces;

namespace API0093BK.Services
{
    public class WishService : IWishService
    {
        private readonly IWishRepository _wishRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<WishService> _logger;

        public WishService(
            IWishRepository wishRepository,
            IUserRepository userRepository,
            ILogger<WishService> logger)
        {
            _wishRepository = wishRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<WishDto>> GetUserWishesAsync(int userId)
        {
            var wishes = await _wishRepository.GetWishesByUserAsync(userId);
            return wishes.Select(MapToDto);
        }

        public async Task<IEnumerable<WishDto>> GetAllWishesAsync()
        {
            var wishes = await _wishRepository.GetAllWithUsersAsync();
            return wishes.Select(MapToDto);
        }

        public async Task<IEnumerable<WishDto>> GetPendingWishesAsync()
        {
            var wishes = await _wishRepository.GetPendingWishesAsync();
            return wishes.Select(MapToDto);
        }

        public async Task<IEnumerable<WishDto>> GetWishesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var wishes = await _wishRepository.GetWishesByDateRangeAsync(startDate, endDate);
            return wishes.Select(MapToDto);
        }

        public async Task<WishDto> CreateWishAsync(int userId, WishCreateDto wishDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("Пользователь не найден");

            // Проверка существующего пожелания на эту дату
            if (await HasWishForDateAsync(userId, wishDto.RequestedDate))
                throw new InvalidOperationException("У вас уже есть пожелание на эту дату");

            // Проверка корректности времени
            if (wishDto.StartTime.HasValue && wishDto.EndTime.HasValue)
            {
                if (wishDto.StartTime >= wishDto.EndTime)
                    throw new InvalidOperationException("Время начала должно быть меньше времени окончания");
            }

            var wish = new Wish
            {
                UserId = userId,
                RequestedDate = wishDto.RequestedDate.Date,
                StartTime = wishDto.StartTime,
                EndTime = wishDto.EndTime,
                Comment = wishDto.Comment,
                Status = WishStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            var createdWish = await _wishRepository.AddAsync(wish);
            _logger.LogInformation("Создано новое пожелание {WishId} для пользователя {UserId}", createdWish.Id, userId);

            return MapToDto(createdWish);
        }

        public async Task<bool> DeleteWishAsync(int wishId, int userId)
        {
            var wish = await _wishRepository.GetByIdAsync(wishId);

            if (wish == null)
                throw new KeyNotFoundException("Пожелание не найдено");

            if (wish.UserId != userId)
                throw new UnauthorizedAccessException("Вы можете удалять только свои пожелания");

            if (wish.Status != WishStatus.Pending)
                throw new InvalidOperationException("Нельзя удалить пожелание, которое уже обработано");

            await _wishRepository.DeleteAsync(wish);
            _logger.LogInformation("Пожелание {WishId} удалено пользователем {UserId}", wishId, userId);

            return true;
        }

        public async Task<WishDto> UpdateWishStatusAsync(int wishId, string status, int managerId)
        {
            if (!WishStatus.All.Contains(status))
                throw new ArgumentException($"Недопустимый статус: {status}");

            var wish = await _wishRepository.GetByIdAsync(wishId);

            if (wish == null)
                throw new KeyNotFoundException("Пожелание не найдено");

            var oldStatus = wish.Status;
            wish.Status = status;
            wish.UpdatedAt = DateTime.UtcNow;

            await _wishRepository.UpdateAsync(wish);
            _logger.LogInformation("Статус пожелания {WishId} изменен с {OldStatus} на {NewStatus} менеджером {ManagerId}",
                wishId, oldStatus, status, managerId);

            return MapToDto(wish);
        }

        public async Task<bool> HasWishForDateAsync(int userId, DateTime date)
        {
            var wishes = await _wishRepository.FindAsync(w =>
                w.UserId == userId && w.RequestedDate == date.Date);
            return wishes.Any();
        }

        private WishDto MapToDto(Wish wish)
        {
            return new WishDto
            {
                Id = wish.Id,
                UserId = wish.UserId,
                UserName = wish.User?.FullName ?? "Неизвестно",
                RequestedDate = wish.RequestedDate,
                StartTime = wish.StartTime,
                EndTime = wish.EndTime,
                Status = wish.Status,
                Comment = wish.Comment,
                CreatedAt = wish.CreatedAt
            };
        }
    }
}