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
                ? $"BookingCode eq '{bookingCode}' and CreatedBy eq '{userName}'"
                : $"BookingCode eq '{bookingCode}' and (UserId eq {userId} or CreatedBy eq '{userName}')";

            var response = await client.GetAsync(
                $"odata/Bookings?$expand=Schedule($expand=Tour)&$filter={filterQuery}&$top=1");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ODataResponse<BookingDetailViewModel>>();
                Booking = result?.Value?.Count > 0 ? result.Value[0] : null;
            }

            if (Booking == null)
                return NotFound();

            return Page();
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
            public string? CCCD { get; set; }
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
            public DateTime BookingDate { get; set; }
            public DateTime CreatedDate { get; set; }
            public string? Notes { get; set; }
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
