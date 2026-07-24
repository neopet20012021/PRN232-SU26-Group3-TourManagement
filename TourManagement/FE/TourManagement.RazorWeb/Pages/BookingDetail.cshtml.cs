using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TourManagement.RazorWeb.Pages
{
    [Authorize(Roles = "Customer")]
    public class BookingDetailModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public BookingDetailModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public BookingDetailViewModel? Booking { get; set; }

        public async Task<IActionResult> OnGetAsync(string bookingCode)
        {
            if (string.IsNullOrEmpty(bookingCode))
                return RedirectToPage("/MyBookings");

            var client = _clientFactory.CreateClient("API");

            // Tìm booking theo BookingCode với $filter
            var userId = User.FindFirst("UserId")?.Value;
            var userName = User.Identity?.Name ?? "";
            var filterQuery = string.IsNullOrEmpty(userId)
                ? $"BookingCode eq '{bookingCode}'"
                : $"BookingCode eq '{bookingCode}' and UserId eq {userId}";

            var response = await client.GetAsync(
                $"odata/Bookings?$expand=Schedule($expand=Tour),Payments&$filter={filterQuery}&$top=1");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ODataResponse<BookingDetailViewModel>>();
                Booking = result?.Value?.Count > 0 ? result.Value[0] : null;
            }

            if (Booking == null)
                return NotFound();

            // Handle VNPay return status
            if (Request.Query.TryGetValue("status", out var statusValue))
            {
                if (statusValue == "success")
                {
                    TempData["SuccessMessage"] = "Thanh toán qua VNPay thành công!";
                }
                else if (statusValue == "failed")
                {
                    TempData["ErrorMessage"] = "Thanh toán VNPay thất bại hoặc bị hủy.";
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostMarkTransferredAsync(string bookingCode)
        {
            if (string.IsNullOrEmpty(bookingCode))
                return BadRequest();

            var client = _clientFactory.CreateClient("API");
            var response = await client.PostAsync($"odata/Bookings/{bookingCode}/mark-transferred", null);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Đã gửi thông báo xác nhận chuyển khoản. Vui lòng chờ nhân viên kiểm tra.";
            }
            else
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi gửi thông báo.";
            }

            return RedirectToPage(new { bookingCode });
        }

        public async Task<IActionResult> OnPostCreateVnPayUrlAsync(string bookingCode)
        {
            if (string.IsNullOrEmpty(bookingCode))
                return BadRequest();

            var returnUrl = Url.PageLink("/BookingDetail", values: new { bookingCode = bookingCode });
            // Cần URL API đầy đủ thay vì relative, ở RazorWeb API base URL lấy từ _clientFactory
            var client = _clientFactory.CreateClient("API");
            
            // Gọi endpoint tạo VNPay URL
            var response = await client.PostAsync($"api/Payments/create-url?bookingCode={bookingCode}&returnUrl={Uri.EscapeDataString(returnUrl ?? "")}", null);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<VnPayUrlResponse>();
                if (result != null && !string.IsNullOrEmpty(result.Url))
                {
                    return Redirect(result.Url);
                }
            }

            TempData["ErrorMessage"] = "Có lỗi xảy ra khi khởi tạo thanh toán VNPay.";
            return RedirectToPage(new { bookingCode });
        }

        public class VnPayUrlResponse
        {
            public string Url { get; set; } = string.Empty;
        }

        public class ODataResponse<T>
        {
            public System.Collections.Generic.List<T> Value { get; set; } = new();
        }

        public class BookingDetailViewModel
        {
            public int BookingId { get; set; }
            public string BookingCode { get; set; } = string.Empty;
            public int ScheduleId { get; set; }
            public ScheduleViewModel? Schedule { get; set; }
            public string CustomerName { get; set; } = string.Empty;
            public string PhoneNumber { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;

            public int AdultCount { get; set; }
            public int ChildCount { get; set; }
            public int InfantCount { get; set; }

            public string? SpecialRequest { get; set; }
            public string? PromoCode { get; set; }
            public decimal TotalPrice { get; set; }
            public decimal DiscountAmount { get; set; }
            public decimal FinalPrice { get; set; }
            public string PaymentMethod { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public DateTime BookingDate { get; set; }
            public DateTime CreatedDate { get; set; }
            public string? Notes { get; set; }
            public System.Collections.Generic.List<PaymentViewModel> Payments { get; set; } = new();
        }

        public class PaymentViewModel
        {
            public decimal Amount { get; set; }
            public string PaymentMethod { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public DateTime PaymentDate { get; set; }
        }

        public class ScheduleViewModel
        {
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public TourViewModel? Tour { get; set; }
        }

        public class TourViewModel
        {
            public string TourName { get; set; } = string.Empty;
            public string Destination { get; set; } = string.Empty;
            public int Days { get; set; }
            public int Nights { get; set; }
        }
    }
}
