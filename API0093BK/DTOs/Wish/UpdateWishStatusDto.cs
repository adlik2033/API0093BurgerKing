using System.ComponentModel.DataAnnotations;

namespace API0093BK.DTOs.Wish
{
    public class UpdateWishStatusDto
    {
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = string.Empty;
    }
}