using API0093BK.DTOs.User;

namespace API0093BK.Services.Interfaces
{
    public interface IUserManagementService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto> GetUserByIdAsync(int id);
        Task<UserDto> GetUserByEmployeeNumberAsync(string employeeNumber);
        Task<UserDto> CreateUserAsync(UserCreateDto userCreateDto, int adminId);
        Task<UserDto> UpdateUserAsync(int userId, UserUpdateDto userUpdateDto, int adminId);
        Task<bool> DeleteUserAsync(int userId, int adminId);
        Task<bool> ResetPasswordAsync(int userId, string newPassword, int adminId);
        Task<IEnumerable<UserDto>> GetUsersByRoleAsync(string role);
        Task UpdateLastSyncDateAsync(int userId);

        Task SendTrainingAsync(int userId);
    }
}