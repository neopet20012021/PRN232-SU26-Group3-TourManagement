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

        [Required(ErrorMessage = "Lịch khởi hành là bắt buộc")]
        public int ScheduleId { get; set; }

        [ForeignKey(nameof(ScheduleId))]
        public virtual TourSchedule? Schedule { get; set; }

        [Required(ErrorMessage = "Tên khách hàng là bắt buộc")]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [StringLength(20)]
        [RegularExpression(@"^\+?[0-9\s\-\.]{7,15}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;


        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng người lớn không được nhỏ hơn 0")]
        public int AdultCount { get; set; } = 1;

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng trẻ em không được nhỏ hơn 0")]
        public int ChildCount { get; set; } = 0;

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng trẻ sơ sinh không được nhỏ hơn 0")]
        public int InfantCount { get; set; } = 0;


        [StringLength(500)]
        public string? SpecialRequest { get; set; }

        public int? PromoCodeId { get; set; }

        [ForeignKey(nameof(PromoCodeId))]
        public virtual PromoCode? AppliedPromoCode { get; set; }

        [NotMapped]
        public string? PromoCode { get; set; }

        [NotMapped]
        public string PaymentMethod { get; set; } = string.Empty;

        [Column(TypeName = "DECIMAL(18, 2)")]
        public decimal TotalPrice { get; set; }

        [Column(TypeName = "DECIMAL(18, 2)")]
        public decimal DiscountAmount { get; set; }

        [Column(TypeName = "DECIMAL(18, 2)")]
        public decimal FinalPrice { get; set; }

        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

        [StringLength(20)]
        [Column(TypeName = "VARCHAR(20)")]
        public string Status { get; set; } = "pending";

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Required]
        public DateTime BookingDate { get; set; }


        public int? UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual User? User { get; set; }
    }
}