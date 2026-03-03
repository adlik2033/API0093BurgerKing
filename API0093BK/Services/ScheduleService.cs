using API0093BK.DTOs.Schedule;
using API0093BK.Models;
using API0093BK.Repositories.Interfaces;
using API0093BK.Services.Interfaces;

namespace API0093BK.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IUserRepository _userRepository;
        private readonly IWishRepository _wishRepository;
        private readonly ILogger<ScheduleService> _logger;

        public ScheduleService(
            IScheduleRepository scheduleRepository,
            IUserRepository userRepository,
            IWishRepository wishRepository,
            ILogger<ScheduleService> logger)
        {
            _scheduleRepository = scheduleRepository;
            _userRepository = userRepository;
            _wishRepository = wishRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<ScheduleDto>> GetUserScheduleAsync(int userId, DateTime weekStart)
        {
            var weekEnd = weekStart.AddDays(7);
            var schedules = await _scheduleRepository.GetUserSchedulesByDateRangeAsync(userId, weekStart, weekEnd);
            return schedules.Select(MapToDto);
        }

        public async Task<IEnumerable<ScheduleDto>> GetWeekScheduleAsync(DateTime weekStart)
        {
            var schedules = await _scheduleRepository.GetSchedulesByWeekAsync(weekStart);
            return schedules.Select(MapToDto);
        }

        public async Task<IEnumerable<ScheduleDto>> GetFinalScheduleAsync(DateTime weekStart)
        {
            var schedules = await _scheduleRepository.GetFinalSchedulesByWeekAsync(weekStart);
            return schedules.Select(MapToDto);
        }

        public async Task<ScheduleDto> CreateOrUpdateScheduleAsync(ScheduleCreateDto scheduleDto, int managerId)
        {
            var user = await _userRepository.GetByIdAsync(scheduleDto.UserId);
            if (user == null)
                throw new KeyNotFoundException($"Пользователь с ID {scheduleDto.UserId} не найден");

            // Проверка корректности времени
            if (scheduleDto.StartTime >= scheduleDto.EndTime)
                throw new InvalidOperationException("Время начала должно быть меньше времени окончания");

            // Поиск существующей записи
            var existingSchedules = await _scheduleRepository.FindAsync(s =>
                s.UserId == scheduleDto.UserId && s.WorkDate.Date == scheduleDto.WorkDate.Date);

            var schedule = existingSchedules.FirstOrDefault();

            if (schedule == null)
            {
                schedule = new Schedule
                {
                    UserId = scheduleDto.UserId,
                    WeekStartDate = scheduleDto.WeekStartDate.Date,
                    WorkDate = scheduleDto.WorkDate.Date,
                    StartTime = scheduleDto.StartTime,
                    EndTime = scheduleDto.EndTime,
                    IsFinal = false,
                    CreatedAt = DateTime.UtcNow
                };

                schedule = await _scheduleRepository.AddAsync(schedule);
                _logger.LogInformation("Создана запись в расписании для пользователя {UserId} на {WorkDate}",
                    schedule.UserId, schedule.WorkDate);
            }
            else
            {
                if (schedule.IsFinal)
                    throw new InvalidOperationException("Нельзя изменить утвержденное расписание");

                schedule.StartTime = scheduleDto.StartTime;
                schedule.EndTime = scheduleDto.EndTime;
                schedule.UpdatedAt = DateTime.UtcNow;

                await _scheduleRepository.UpdateAsync(schedule);
                _logger.LogInformation("Обновлена запись в расписании для пользователя {UserId} на {WorkDate}",
                    schedule.UserId, schedule.WorkDate);
            }

            return MapToDto(schedule);
        }

        public async Task<bool> ApproveWeekScheduleAsync(DateTime weekStart, int managerId)
        {
            var schedules = await _scheduleRepository.GetSchedulesByWeekAsync(weekStart);

            if (!schedules.Any())
                throw new InvalidOperationException("Нет расписания для утверждения на указанную неделю");

            foreach (var schedule in schedules)
            {
                schedule.IsFinal = true;
                schedule.ApprovedBy = managerId;
                schedule.UpdatedAt = DateTime.UtcNow;
                await _scheduleRepository.UpdateAsync(schedule);
            }

            _logger.LogInformation("Утверждено расписание на неделю {WeekStart} менеджером {ManagerId}",
                weekStart, managerId);

            return true;
        }

        public async Task<bool> GenerateScheduleFromWishesAsync(DateTime weekStart, int managerId)
        {
            var weekEnd = weekStart.AddDays(7);

            // Получаем одобренные пожелания на неделю
            var approvedWishes = await _wishRepository.GetWishesByDateRangeAsync(weekStart, weekEnd);
            approvedWishes = approvedWishes.Where(w => w.Status == WishStatus.Approved);

            // Получаем всех активных сотрудников
            var users = await _userRepository.GetUsersByRoleAsync(UserRoles.Employee);

            // Удаляем существующие черновики
            var existingDrafts = await _scheduleRepository.GetSchedulesByWeekAsync(weekStart);
            foreach (var draft in existingDrafts.Where(s => !s.IsFinal))
            {
                await _scheduleRepository.DeleteAsync(draft);
            }

            // Создаем новое расписание
            foreach (var user in users)
            {
                var userWishes = approvedWishes.Where(w => w.UserId == user.Id);

                for (int day = 0; day < 7; day++)
                {
                    var currentDate = weekStart.AddDays(day);
                    var wishForDay = userWishes.FirstOrDefault(w => w.RequestedDate == currentDate.Date);

                    var schedule = new Schedule
                    {
                        UserId = user.Id,
                        WeekStartDate = weekStart.Date,
                        WorkDate = currentDate.Date,
                        IsFinal = false,
                        CreatedAt = DateTime.UtcNow
                    };

                    if (wishForDay != null)
                    {
                        // Если есть пожелание, используем указанное время или стандартное
                        schedule.StartTime = wishForDay.StartTime ?? new TimeSpan(9, 0, 0);
                        schedule.EndTime = wishForDay.EndTime ?? new TimeSpan(18, 0, 0);
                    }
                    else
                    {
                        // Стандартное расписание
                        schedule.StartTime = new TimeSpan(9, 0, 0);
                        schedule.EndTime = new TimeSpan(18, 0, 0);
                    }

                    await _scheduleRepository.AddAsync(schedule);
                }
            }

            _logger.LogInformation("Сгенерировано расписание на неделю {WeekStart}", weekStart);

            return true;
        }

        public async Task<bool> ValidateScheduleConflictsAsync(DateTime weekStart)
        {
            var schedules = await _scheduleRepository.GetSchedulesByWeekAsync(weekStart);
            var conflicts = new List<string>();

            // Группируем по дням
            var schedulesByDay = schedules.GroupBy(s => s.WorkDate);


            return !conflicts.Any();
        }

        private ScheduleDto MapToDto(Schedule schedule)
        {
            return new ScheduleDto
            {
                Id = schedule.Id,
                UserId = schedule.UserId,
                UserName = schedule.User?.FullName ?? "Неизвестно",
                WeekStartDate = schedule.WeekStartDate,
                WorkDate = schedule.WorkDate,
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime,
                IsFinal = schedule.IsFinal
            };
        }
    }
}