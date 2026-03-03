namespace API0093BK.Models
{
    /// <summary>
    /// Константы для ролей пользователей
    /// </summary>
    public static class UserRoles
    {
        public const string Administrator = "Administrator";
        public const string Manager = "Manager";
        public const string Employee = "Employee";

        /// <summary>
        /// Все доступные роли
        /// </summary>
        public static readonly string[] All = { Administrator, Manager, Employee };

        /// <summary>
        /// Проверка, является ли роль допустимой
        /// </summary>
        public static bool IsValid(string role)
        {
            return All.Contains(role);
        }

        /// <summary>
        /// Получение русского названия роли
        /// </summary>
        public static string GetRussianName(string role)
        {
            return role switch
            {
                Administrator => "Администратор",
                Manager => "Менеджер",
                Employee => "Сотрудник",
                _ => role
            };
        }

        /// <summary>
        /// Получение ролей, которые может создавать администратор
        /// </summary>
        public static string[] GetCreatableRoles()
        {
            return new[] { Manager, Employee };
        }

        /// <summary>
        /// Проверка, может ли пользователь с ролью currentRole создать пользователя с ролью targetRole
        /// </summary>
        public static bool CanCreateRole(string currentRole, string targetRole)
        {
            if (currentRole == Administrator)
            {
                return targetRole == Manager || targetRole == Employee;
            }

            if (currentRole == Manager)
            {
                return targetRole == Employee;
            }

            return false;
        }
    }
}