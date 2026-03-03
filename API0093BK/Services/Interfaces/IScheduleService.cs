using API0093BK.DTOs.Schedule;

namespace API0093BK.Services.Interfaces
{
    public interface IScheduleService
    {
        Task<IEnumerable<ScheduleDto>> GetUserScheduleAsync(int userId, DateTime weekStart);
        Task<IEnumerable<ScheduleDto>> GetWeekScheduleAsync(DateTime weekStart);
        Task<IEnumerable<ScheduleDto>> GetFinalScheduleAsync(DateTime weekStart);
        Task<ScheduleDto> CreateOrUpdateScheduleAsync(ScheduleCreateDto scheduleDto, int managerId);
        Task<bool> ApproveWeekScheduleAsync(DateTime weekStart, int managerId);
        Task<bool> GenerateScheduleFromWishesAsync(DateTime weekStart, int managerId);
        Task<bool> ValidateScheduleConflictsAsync(DateTime weekStart);
    }
}