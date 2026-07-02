using System;
using System.ComponentModel.DataAnnotations;

namespace TourManagement.Web.Models
{
    public class BookingWizardSessionModel
    {
        public int SelectedTourId { get; set; }
        public int SelectedScheduleId { get; set; }
        public string? CustomerName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public int AdultCount { get; set; } = 1;
        public int ChildCount { get; set; } = 0;
        public string? RoomType { get; set; }
        public string? PromoCode { get; set; }
        public DateTime BookingDate { get; set; }
        public string? SpecialRequest { get; set; }
        public string? PaymentMethod { get; set; }
    }

    public class BookingDetailsViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn tour.")]
        public int TourId { get; set; }

        public int ScheduleId { get; set; }

        public string TourName { get; set; } = string.Empty;
        public DateTime DepartureDate { get; set; }
        public decimal PricePerAdult { get; set; }
        public decimal PricePerChild { get; set; }

        [Required(ErrorMessage = "Tên khách hàng là bắt buộc.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Họ tên phải từ 3 đến 100 ký tự.")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [RegularExpression(@"^\+?[0-9\s\-\.]{7,15}$", ErrorMessage = "Số điện thoại không hợp lệ.")]
        [Display(Name = "Số điện thoại")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        public string Email { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Số CCCD không được quá 50 ký tự.")]
        public string? CCCD { get; set; }

        [StringLength(200, ErrorMessage = "Địa chỉ không được quá 200 ký tự.")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Số người lớn là bắt buộc.")]
        [Range(1, 100, ErrorMessage = "Số người lớn phải từ 1 đến 100.")]
        public int AdultCount { get; set; } = 1;

        [Required(ErrorMessage = "Số trẻ em là bắt buộc.")]
        [Range(0, 100, ErrorMessage = "Số trẻ em phải từ 0 đến 100.")]
        public int ChildCount { get; set; } = 0;

        [Required(ErrorMessage = "Số trẻ nhỏ là bắt buộc.")]
        [Range(0, 100, ErrorMessage = "Số trẻ nhỏ phải từ 0 đến 100.")]
        public int InfantCount { get; set; } = 0;

        [StringLength(50)]
        public string RoomType { get; set; } = "Phòng đôi";

        [StringLength(500, ErrorMessage = "Yêu cầu đặc biệt không được quá 500 ký tự.")]
        public string? SpecialRequest { get; set; }

        [StringLength(20)]
        public string? PromoCode { get; set; }

        [Required(ErrorMessage = "Phương thức thanh toán là bắt buộc.")]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = "cash";

        [Required(ErrorMessage = "Ngày đi là bắt buộc.")]
        public DateTime BookingDate { get; set; } = DateTime.Now;
    }

    public class BookingReviewViewModel
    {
        public string? TourName { get; set; }
        public DateTime DepartureDate { get; set; }
        public string? CustomerName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public int AdultCount { get; set; }
        public int ChildCount { get; set; }
        public string? PromoCode { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalPrice { get; set; }
    }
}
