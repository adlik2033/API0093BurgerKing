namespace API0093BK.DTOs.Wish
{
    /// <summary>
    /// DTO для отображения пожелания
    /// </summary>
    public class WishDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public DateTime WishDate { get; set; }
        public string Type { get; set; } = string.Empty;
        public string? Comment { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}