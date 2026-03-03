using API0093BK.DTOs.User;
using API0093BK.Helpers;
using API0093BK.Models;
using API0093BK.Repositories.Interfaces;
using API0093BK.Services.Interfaces;

namespace API0093BK.Services
{
    public class UserManagementService : IUserManagementService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserManagementService> _logger;

        public UserManagementService(IUserRepository userRepository, ILogger<UserManagementService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

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

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException($"Пользователь с ID {id} не найден");

            return await MapToDto(user);
        }

        public async Task<UserDto> GetUserByEmployeeNumberAsync(string employeeNumber)
        {
            var user = await _userRepository.GetUserByEmployeeNumberAsync(employeeNumber);
            if (user == null)
                throw new KeyNotFoundException($"Пользователь с табельным номером {employeeNumber} не найден");

            return await MapToDto(user);
        }

        public async Task<UserDto> CreateUserAsync(UserCreateDto userCreateDto, int adminId)
        {
            // Проверка уникальности табельного номера
            if (await _userRepository.ExistsAsync(u => u.EmployeeNumber == userCreateDto.EmployeeNumber))
                throw new InvalidOperationException($"Табельный номер '{userCreateDto.EmployeeNumber}' уже используется");

            // Проверка уникальности email
            if (await _userRepository.ExistsAsync(u => u.Email == userCreateDto.Email))
                throw new InvalidOperationException($"Email '{userCreateDto.Email}' уже используется");

            var user = new User
            {
                EmployeeNumber = userCreateDto.EmployeeNumber,
                PasswordHash = PasswordHelper.HashPassword(userCreateDto.Password),
                Email = userCreateDto.Email,
                FullName = userCreateDto.FullName,
                Role = userCreateDto.Role,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = adminId,
                IsActive = true
            };

            var createdUser = await _userRepository.AddAsync(user);
            _logger.LogInformation("Создан новый пользователь: {EmployeeNumber} (ID: {UserId})",
                createdUser.EmployeeNumber, createdUser.Id);

            return await MapToDto(createdUser);
        }

        public async Task<UserDto> UpdateUserAsync(int userId, UserUpdateDto userUpdateDto, int adminId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"Пользователь с ID {userId} не найден");

            if (!string.IsNullOrEmpty(userUpdateDto.FullName))
                user.FullName = userUpdateDto.FullName;

            if (!string.IsNullOrEmpty(userUpdateDto.Email))
                user.Email = userUpdateDto.Email;

            if (!string.IsNullOrEmpty(userUpdateDto.Role))
                user.Role = userUpdateDto.Role;

            if (userUpdateDto.IsActive.HasValue)
                user.IsActive = userUpdateDto.IsActive.Value;

            user.UpdatedAt = DateTime.UtcNow;
            user.UpdatedBy = adminId;

            await _userRepository.UpdateAsync(user);
            _logger.LogInformation("Обновлен пользователь: {EmployeeNumber} (ID: {UserId})",
                user.EmployeeNumber, user.Id);

            return await MapToDto(user);
        }

        public async Task<bool> DeleteUserAsync(int userId, int adminId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"Пользователь с ID {userId} не найден");

            if (userId == adminId)
                throw new InvalidOperationException("Нельзя удалить свой собственный аккаунт");

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;
            user.UpdatedBy = adminId;

            await _userRepository.UpdateAsync(user);
            _logger.LogInformation("Деактивирован пользователь: {EmployeeNumber} (ID: {UserId})",
                user.EmployeeNumber, user.Id);

            return true;
        }

        public async Task<bool> ResetPasswordAsync(int userId, string newPassword, int adminId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"Пользователь с ID {userId} не найден");

            user.PasswordHash = PasswordHelper.HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;
            user.UpdatedBy = adminId;

            await _userRepository.UpdateAsync(user);
            _logger.LogInformation("Сброшен пароль пользователя: {EmployeeNumber} (ID: {UserId})",
                user.EmployeeNumber, user.Id);

            return true;
        }

        public async Task<IEnumerable<UserDto>> GetUsersByRoleAsync(string role)
        {
            var users = await _userRepository.GetUsersByRoleAsync(role);
            var result = new List<UserDto>();

            foreach (var user in users)
            {
                result.Add(await MapToDto(user));
            }

            return result;
        }

        public async Task UpdateLastSyncDateAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user != null)
            {
                user.LastSyncDate = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user);
            }
        }

        /// <summary>
        /// Отправка пользователя на обучение
        /// </summary>
        public async Task SendTrainingAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException($"Пользователь с ID {userId} не найден");

            // Здесь будет интеграция с порталом обучения
            // Пока просто логируем
            _logger.LogInformation("Пользователь {EmployeeNumber} (ID: {UserId}) отправлен на обучение",
                user.EmployeeNumber, user.Id);

            // В будущем здесь будет вызов API портала
            await Task.CompletedTask;
        }

        private async Task<UserDto> MapToDto(User user)
        {
            string createdByName = "System";
            if (user.CreatedBy.HasValue && user.CreatedBy > 0)
            {
                var creator = await _userRepository.GetByIdAsync(user.CreatedBy.Value);
                if (creator != null)
                    createdByName = creator.FullName;
            }

            return new UserDto
            {
                Id = user.Id,
                EmployeeNumber = user.EmployeeNumber,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                LastSyncDate = user.LastSyncDate,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                CreatedBy = createdByName
            };
        }
    }
}