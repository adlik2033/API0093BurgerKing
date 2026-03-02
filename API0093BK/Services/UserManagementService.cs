using API0093BK.DTOs.User;
using API0093BK.Helpers;
using API0093BK.Models;
using API0093BK.Repositories.Interfaces;
using API0093BK.Services.Interfaces;

namespace API0093BK.Services
{
    /// <summary>
    /// Реализация сервиса управления пользователями
    /// </summary>
    public class UserManagementService : IUserManagementService
    {
        private readonly IUserRepository _userRepository;

        public UserManagementService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Получение всех активных пользователей
        /// </summary>
        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            var result = new List<UserDto>();

            foreach (var user in users.Where(u => u.IsActive))
            {
                result.Add(await MapToDto(user));
            }

            return result;
        }

        /// <summary>
        /// Получение пользователя по ID
        /// </summary>
        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException($"Пользователь с ID {id} не найден");

            return await MapToDto(user);
        }

        /// <summary>
        /// Создание нового пользователя
        /// </summary>
        /// <param name="userCreateDto">Данные для создания</param>
        /// <param name="adminId">ID администратора, создающего пользователя</param>
        /// <returns>Созданный пользователь</returns>
        public async Task<UserDto> CreateUserAsync(UserCreateDto userCreateDto, int adminId)
        {
            // Проверка уникальности имени пользователя
            if (await _userRepository.ExistsAsync(u => u.Username == userCreateDto.Username))
                throw new InvalidOperationException($"Имя пользователя '{userCreateDto.Username}' уже занято");

            // Проверка уникальности email
            if (await _userRepository.ExistsAsync(u => u.Email == userCreateDto.Email))
                throw new InvalidOperationException($"Email '{userCreateDto.Email}' уже используется");

            // Проверка уникальности ID из портала
            if (userCreateDto.PortalEmployeeId.HasValue)
            {
                var existingByPortal = await _userRepository.GetUserByPortalIdAsync(userCreateDto.PortalEmployeeId.Value);
                if (existingByPortal != null)
                    throw new InvalidOperationException($"ID сотрудника из портала {userCreateDto.PortalEmployeeId} уже назначен");
            }

            var user = new User
            {
                Username = userCreateDto.Username,
                PasswordHash = PasswordHelper.HashPassword(userCreateDto.Password),
                Email = userCreateDto.Email,
                FirstName = userCreateDto.FirstName,
                LastName = userCreateDto.LastName,
                Role = Enum.Parse<UserRole>(userCreateDto.Role),
                PortalEmployeeId = userCreateDto.PortalEmployeeId,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = adminId,
                IsActive = true
            };

            var createdUser = await _userRepository.AddAsync(user);
            return await MapToDto(createdUser);
        }

        /// <summary>
        /// Обновление данных пользователя
        /// </summary>
        public async Task<UserDto> UpdateUserAsync(int userId, UserUpdateDto userUpdateDto, int adminId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"Пользователь с ID {userId} не найден");

            // Обновление только переданных полей
            if (!string.IsNullOrEmpty(userUpdateDto.Email))
                user.Email = userUpdateDto.Email;

            if (!string.IsNullOrEmpty(userUpdateDto.FirstName))
                user.FirstName = userUpdateDto.FirstName;

            if (!string.IsNullOrEmpty(userUpdateDto.LastName))
                user.LastName = userUpdateDto.LastName;

            if (!string.IsNullOrEmpty(userUpdateDto.Role))
                user.Role = Enum.Parse<UserRole>(userUpdateDto.Role);

            if (userUpdateDto.PortalEmployeeId.HasValue)
                user.PortalEmployeeId = userUpdateDto.PortalEmployeeId;

            if (userUpdateDto.IsActive.HasValue)
                user.IsActive = userUpdateDto.IsActive.Value;

            user.UpdatedAt = DateTime.UtcNow;
            user.UpdatedBy = adminId;

            await _userRepository.UpdateAsync(user);
            return await MapToDto(user);
        }

        /// <summary>
        /// Удаление (деактивация) пользователя
        /// </summary>
        public async Task<bool> DeleteUserAsync(int userId, int adminId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"Пользователь с ID {userId} не найден");

            // Запрет на удаление самого себя
            if (userId == adminId)
                throw new InvalidOperationException("Нельзя удалить свой собственный аккаунт");

            // Мягкое удаление - просто деактивируем
            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;
            user.UpdatedBy = adminId;

            await _userRepository.UpdateAsync(user);
            return true;
        }

        /// <summary>
        /// Сброс пароля пользователя
        /// </summary>
        public async Task<bool> ResetPasswordAsync(int userId, string newPassword, int adminId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"Пользователь с ID {userId} не найден");

            user.PasswordHash = PasswordHelper.HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;
            user.UpdatedBy = adminId;

            await _userRepository.UpdateAsync(user);
            return true;
        }

        /// <summary>
        /// Отправка пользователя на обучение
        /// </summary>
        public async Task SendTrainingAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"Пользователь с ID {userId} не найден");

            if (!user.PortalEmployeeId.HasValue)
                throw new InvalidOperationException("Пользователь не связан с ID сотрудника из портала");

            // Здесь будет вызов к API портала обучения
            await Task.CompletedTask;
        }

        /// <summary>
        /// Преобразование модели User в DTO с получением имени создателя
        /// </summary>
        private async Task<UserDto> MapToDto(User user)
        {
            string createdByName = "System";
            if (user.CreatedBy > 0)
            {
                var creator = await _userRepository.GetByIdAsync(user.CreatedBy);
                if (creator != null)
                    createdByName = $"{creator.FirstName} {creator.LastName}";
            }

            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.ToString(),
                PortalEmployeeId = user.PortalEmployeeId,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                CreatedBy = createdByName,
                LastLoginAt = user.LastLoginAt
            };
        }
    }
}