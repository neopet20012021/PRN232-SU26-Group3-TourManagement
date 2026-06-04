using System;
using System.ComponentModel.DataAnnotations;

namespace TourManagement.Web.Models
{
    // ─── BookingWizard session ─────────────────────────────────────────────────
    public class BookingWizardSessionModel
    {
        public int SelectedTourId { get; set; }
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

    // ─── Booking wizard step view models ──────────────────────────────────────
    public class BookingDetailsViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn tour.")]
        public int TourId { get; set; }

        public string TourName { get; set; } = string.Empty;
        public DateTime DepartureDate { get; set; }
        public decimal PricePerAdult { get; set; }
        public decimal PricePerChild { get; set; }

        [Required(ErrorMessage = "Tên khách hàng là bắt buộc.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Họ tên phải từ 3 đến 100 ký tự.")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc.")]
        [RegularExpression(@"^0\d{9}$|^\+84\d{9}$", ErrorMessage = "Số điện thoại không hợp lệ.")]
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

    // ─── Account view models ───────────────────────────────────────────────────
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Tên đăng nhập là bắt buộc.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên đăng nhập phải từ 3 đến 50 ký tự.")]
        [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Tên đăng nhập chỉ gồm chữ, số và dấu gạch dưới.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Họ và tên là bắt buộc.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Họ tên phải từ 3 đến 100 ký tự.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        [RegularExpression(@"^0\d{9}$|^\+84\d{9}$|^$", ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 ký tự trở lên.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    // ─── Admin Booking management view models ─────────────────────────────────
    public class AdminBookingListViewModel
    {
        public IEnumerable<TourManagement.Business.DTOs.BookingDTO> Bookings { get; set; } = new List<TourManagement.Business.DTOs.BookingDTO>();
        public string? SearchKeyword { get; set; }
        public string? StatusFilter { get; set; }
        public int TotalCount { get; set; }
    }
}
