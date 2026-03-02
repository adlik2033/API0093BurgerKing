using System.ComponentModel.DataAnnotations;

namespace API0093BK.DTOs.Wish
{
    /// <summary>
    /// DTO для создания пожелания
    /// </summary>
    public class WishCreateDto
    {
        [Required(ErrorMessage = "Дата пожелания обязательна")]
        public DateTime WishDate { get; set; }

        [Required(ErrorMessage = "Тип пожелания обязателен")]
        public string Type { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Комментарий не может превышать 500 символов")]
        public string? Comment { get; set; }
    }
}