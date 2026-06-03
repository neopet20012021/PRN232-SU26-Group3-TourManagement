using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TourManagement.Data.Models
{
    [Table("Bookings")]
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        [Required(ErrorMessage = "Mã booking là bắt buộc")]
        [StringLength(20)]
        [Column(TypeName = "VARCHAR(20)")]
        public string BookingCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tour là bắt buộc")]
        public int TourId { get; set; }

        [ForeignKey(nameof(TourId))]
        public virtual Tour? Tour { get; set; }

        [Required(ErrorMessage = "Tên khách hàng là bắt buộc")]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [StringLength(20)]
        [RegularExpression(@"^0\d{9}$|^\+84\d{9}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        [StringLength(50)]
        public string? CCCD { get; set; }

        [Required]
        public int AdultCount { get; set; } = 1;

        [Required]
        public int ChildCount { get; set; } = 0;

        [Required]
        public int InfantCount { get; set; } = 0;

        [StringLength(50)]
        public string? RoomType { get; set; }

        [StringLength(500)]
        public string? SpecialRequest { get; set; }

        [StringLength(20)]
        public string? PromoCode { get; set; }

        [Column(TypeName = "DECIMAL(18, 2)")]
        public decimal TotalPrice { get; set; }

        [Column(TypeName = "DECIMAL(18, 2)")]
        public decimal DiscountAmount { get; set; }

        [Column(TypeName = "DECIMAL(18, 2)")]
        public decimal FinalPrice { get; set; }

        [StringLength(50)]
        public string PaymentMethod { get; set; } = string.Empty;

        [StringLength(20)]
        [Column(TypeName = "VARCHAR(20)")]
        public string Status { get; set; } = "pending";

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Required]
        public DateTime BookingDate { get; set; }

        [StringLength(200)]
        public string? Notes { get; set; }

        [StringLength(50)]
        public string? CreatedBy { get; set; }
    }
}