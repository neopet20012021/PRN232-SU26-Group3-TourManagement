using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourManagement.Data.Models
{
    [Table("Reviews")]
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        [Required]
        public int TourId { get; set; }

        public int? UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [Range(1, 5, ErrorMessage = "Điểm đánh giá chung từ 1 đến 5 sao")]
        public int Rating { get; set; } = 5;

        [Required]
        [Range(1, 5, ErrorMessage = "Điểm sạch sẽ từ 1 đến 5 sao")]
        public int CleanlinessRating { get; set; } = 5;

        [Required]
        [Range(1, 5, ErrorMessage = "Điểm thoải mái từ 1 đến 5 sao")]
        public int ComfortRating { get; set; } = 5;

        [Required]
        [Range(1, 5, ErrorMessage = "Điểm tiện ích & dịch vụ từ 1 đến 5 sao")]
        public int AmenitiesRating { get; set; } = 5;

        [Required]
        [Range(1, 5, ErrorMessage = "Điểm phù hợp từ 1 đến 5 sao")]
        public int ValueRating { get; set; } = 5;

        [Required]
        [StringLength(1000)]
        public string Comment { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [ForeignKey("TourId")]
        public virtual Tour? Tour { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}
