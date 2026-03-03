using System.ComponentModel.DataAnnotations;

namespace API0093BK.DTOs.Wish
{
    public class WishCreateDto
    {
        [Required(ErrorMessage = "Дата запроса обязательна")]
        public DateTime RequestedDate { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        [StringLength(500, ErrorMessage = "Комментарий не может превышать 500 символов")]
        public string? Comment { get; set; }
    }
}