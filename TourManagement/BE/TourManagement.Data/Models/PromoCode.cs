using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourManagement.Data.Models
{
    [Table("PromoCodes")]
    public class PromoCode
    {
        [Key]
        public int PromoCodeId { get; set; }

        [Required(ErrorMessage = "Mã khuyến mãi là bắt buộc")]
        [StringLength(50)]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phần trăm giảm giá là bắt buộc")]
        [Range(0.0, 1.0, ErrorMessage = "Phần trăm giảm giá phải từ 0 đến 1 (ví dụ 0.1 cho 10%)")]
        [Column(TypeName = "DECIMAL(18, 2)")]
        public decimal DiscountPercent { get; set; }

        [Required]
        [Range(0, 1000000000, ErrorMessage = "Giá trị đơn hàng tối thiểu không hợp lệ")]
        [Column(TypeName = "DECIMAL(18, 2)")]
        public decimal MinBookingValue { get; set; } = 0;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Số lần sử dụng tối đa phải lớn hơn 0")]
        public int MaxUsage { get; set; }

        [Required]
        public int UsageCount { get; set; } = 0;

        [Required]
        public bool IsActive { get; set; } = true;
    }
}
