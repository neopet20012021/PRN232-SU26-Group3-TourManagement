using System;
using System.ComponentModel.DataAnnotations;

namespace TourManagement.Business.DTOs
{
    public class CreateBookingDTO
    {
        [Required(ErrorMessage = "Tour là bắt buộc")]
        public int TourId { get; set; }

        [Required(ErrorMessage = "Tên khách hàng là bắt buộc")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Tên phải từ 3-100 ký tự")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [RegularExpression(@"^0\d{9}$|^\+84\d{9}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        [StringLength(50)]
        public string? CCCD { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "Số người lớn phải từ 1 đến 100")]
        public int AdultCount { get; set; } = 1;

        [Required]
        [Range(0, 100, ErrorMessage = "Số trẻ em phải từ 0 đến 100")]
        public int ChildCount { get; set; } = 0;

        [Required]
        [Range(0, 100, ErrorMessage = "Số trẻ sơ sinh phải từ 0 đến 100")]
        public int InfantCount { get; set; } = 0;

        public string? RoomType { get; set; }
        public string? SpecialRequest { get; set; }
        public string? PromoCode { get; set; }
        public string PaymentMethod { get; set; } = "cash";
        public DateTime BookingDate { get; set; } = DateTime.Now;
    }

    public class BookingDTO
    {
        public int BookingId { get; set; }
        public string BookingCode { get; set; } = string.Empty;
        public int TourId { get; set; }
        public string TourName { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? CCCD { get; set; }
        public string? Address { get; set; }
        public int AdultCount { get; set; }
        public int ChildCount { get; set; }
        public int InfantCount { get; set; }
        public string? RoomType { get; set; }
        public string? SpecialRequest { get; set; }
        public string? PromoCode { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalPrice { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime BookingDate { get; set; }
    }

    public class BookingResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public BookingDTO? Data { get; set; }
    }

    public class TourSelectDTO
    {
        public int Id { get; set; }
        public string TourName { get; set; } = string.Empty;
        public string TourCode { get; set; } = string.Empty;
        public DateTime DepartureDate { get; set; }
        public int AvailableSeats { get; set; }
        public decimal PricePerAdult { get; set; }
        public decimal PricePerChild { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public string? Image { get; set; }
    }

    public class PriceCalculationDTO
    {
        public decimal OriginalPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalPrice { get; set; }
        public string? AppliedPromoCode { get; set; }
    }
}