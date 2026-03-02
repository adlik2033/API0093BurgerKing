using API0093BK.DTOs.Schedule;
using API0093BK.Models;
using API0093BK.Repositories.Interfaces;
using API0093BK.Services.Interfaces;

namespace API0093BK.Services
{
    /// <summary>
    /// Реализация сервиса расписания
    /// </summary>
    public class ScheduleService : IScheduleService
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IUserRepository _userRepository;

        public ScheduleService(IScheduleRepository scheduleRepository, IUserRepository userRepository)
        {
            _scheduleRepository = scheduleRepository;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Получение расписания пользователя на неделю
        /// </summary>
        public async Task<IEnumerable<ScheduleDto>> GetUserScheduleAsync(int userId, DateTime weekStart)
        {
            var weekEnd = weekStart.AddDays(7);
            var schedules = await _scheduleRepository.GetUserSchedulesByDateRangeAsync(userId, weekStart, weekEnd);

            if (!schedules.Any())
            {
                return Enumerable.Empty<ScheduleDto>();
            }

            return schedules.Select(MapToDto);
        }

        /// <summary>
        /// Получение расписания на неделю для всех сотрудников
        /// </summary>
        public async Task<IEnumerable<ScheduleDto>> GetWeekScheduleAsync(DateTime weekStart)
        {
            var schedules = await _scheduleRepository.GetSchedulesByWeekAsync(weekStart);

            if (!schedules.Any())
            {
                return Enumerable.Empty<ScheduleDto>();
            }

            return schedules.Select(MapToDto);
        }

        /// <summary>
        /// Создание или обновление записи в расписании
        /// </summary>
        public async Task<ScheduleDto> CreateOrUpdateScheduleAsync(ScheduleCreateDto scheduleDto, int managerId)
        {
            // Поиск существующей записи
            var existingSchedules = await _scheduleRepository.FindAsync(s =>
                s.UserId == scheduleDto.UserId && s.WorkDate.Date == scheduleDto.WorkDate.Date);

            var schedule = existingSchedules.FirstOrDefault();

            if (schedule == null)
            {
                // Создание новой записи
                schedule = new Schedule
                {
                    UserId = scheduleDto.UserId,
                    WeekStartDate = scheduleDto.WeekStartDate,
                    WeekEndDate = scheduleDto.WeekStartDate.AddDays(7),
                    WorkDate = scheduleDto.WorkDate,
                    StartTime = scheduleDto.StartTime,
                    EndTime = scheduleDto.EndTime,
                    Status = ScheduleStatus.Draft,
                    CreatedAt = DateTime.UtcNow
                };

                schedule = await _scheduleRepository.AddAsync(schedule);
            }
            else
            {
                // Обновление существующей записи
                schedule.StartTime = scheduleDto.StartTime;
                schedule.EndTime = scheduleDto.EndTime;
                schedule.UpdatedAt = DateTime.UtcNow;

                await _scheduleRepository.UpdateAsync(schedule);
            }

            return MapToDto(schedule);
        }

        /// <summary>
        /// Утверждение расписания на неделю
        /// </summary>
        public async Task<bool> ApproveWeekScheduleAsync(DateTime weekStart, int managerId)
        {
            var schedules = await _scheduleRepository.GetSchedulesByWeekAsync(weekStart);

            if (!schedules.Any())
            {
                throw new InvalidOperationException("Нет расписания для утверждения на указанную неделю");
            }

            foreach (var schedule in schedules)
            {
                schedule.Status = ScheduleStatus.Approved;
                schedule.ApprovedBy = managerId;
                schedule.UpdatedAt = DateTime.UtcNow;
                await _scheduleRepository.UpdateAsync(schedule);
            }

            return true;
        }

        /// <summary>
        /// Преобразование модели Schedule в DTO
        /// </summary>
        private ScheduleDto MapToDto(Schedule schedule)
        {
            return new ScheduleDto
            {
                Id = schedule.Id,
                UserId = schedule.UserId,
                UserName = $"{schedule.User?.FirstName} {schedule.User?.LastName}",
                WorkDate = schedule.WorkDate,
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime,
                Status = schedule.Status.ToString()
            };
        }
    }
}