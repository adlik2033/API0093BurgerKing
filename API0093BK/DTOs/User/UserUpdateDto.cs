namespace API0093BK.DTOs.User
{
    /// <summary>
    /// DTO для обновления пользователя
    /// </summary>
    public class UserUpdateDto
    {
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Role { get; set; }
        public int? PortalEmployeeId { get; set; }
        public bool? IsActive { get; set; }
    }
}