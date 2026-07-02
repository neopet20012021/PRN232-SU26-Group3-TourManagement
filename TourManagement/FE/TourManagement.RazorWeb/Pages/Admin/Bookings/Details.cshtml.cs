using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.RazorWeb.Pages.Tours;

namespace TourManagement.RazorWeb.Pages.Admin.Bookings
{
    [Authorize(Roles = "Admin, Staff")]
    public class DetailsModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public DetailsModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public BookingAdminDetailViewModel? Booking { get; set; }

        [BindProperty]
        public int BookingId { get; set; }

        [BindProperty]
        [Required]
        public string NewStatus { get; set; } = string.Empty;

        public string SuccessMessage { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var client = _clientFactory.CreateClient("API");
            var response = await client.GetAsync($"odata/Bookings/{id}?$expand=Schedule($expand=Tour),Payments");
            
            if (response.IsSuccessStatusCode)
            {
                Booking = await response.Content.ReadFromJsonAsync<BookingAdminDetailViewModel>();
                if (Booking != null)
                {
                    NewStatus = Booking.Status; // Preselect current status
                    return Page();
                }
            }
            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (User.IsInRole("Admin"))
            {
                ErrorMessage = "Admin chỉ có quyền xem tổng quan đơn đặt chỗ. Việc xử lý sẽ do Staff phụ trách.";
                return Page();
            }
            
            var client = _clientFactory.CreateClient("API");
            
            // Re-fetch booking first
            var response = await client.GetAsync($"odata/Bookings/{BookingId}?$expand=Schedule($expand=Tour),Payments");
            if (response.IsSuccessStatusCode)
            {
                Booking = await response.Content.ReadFromJsonAsync<BookingAdminDetailViewModel>();
            }

            if (Booking == null) return NotFound();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Update status
            Booking.Status = NewStatus;
            
            // Fix DateTime formatting for OData (must be UTC for ISO 8601 'Z' suffix)
            Booking.BookingDate = DateTime.SpecifyKind(Booking.BookingDate, DateTimeKind.Utc);
            Booking.CreatedDate = DateTime.SpecifyKind(Booking.CreatedDate, DateTimeKind.Utc);
            
            // Avoid sending mismatched navigation properties back
            Booking.Schedule = null;

            // We use PUT to update the booking. OData requires the full object usually.
            var content = new StringContent(JsonSerializer.Serialize(Booking), Encoding.UTF8, "application/json");
            
            // Wait, we need to pass the JWT/Cookie if backend requires Authorize. 
            // In our setup, the API doesn't share cookie so PUT /odata/Bookings might fail if it has [Authorize].
            // Let's call the API.
            var updateResponse = await client.PutAsync($"odata/Bookings/{BookingId}", content);

            if (updateResponse.IsSuccessStatusCode)
            {
                SuccessMessage = "Booking status updated successfully.";
            }
            else
            {
                var errorBody = await updateResponse.Content.ReadAsStringAsync();
                ErrorMessage = $"Failed to update booking status. Error: {errorBody}";
            }

            return Page();
        }

        public class BookingAdminDetailViewModel
        {
            public int BookingId { get; set; }
            public string BookingCode { get; set; } = string.Empty;
            public int ScheduleId { get; set; }
            public TourScheduleViewModel? Schedule { get; set; }
            public string CustomerName { get; set; } = string.Empty;
            public string PhoneNumber { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public int AdultCount { get; set; }
            public int ChildCount { get; set; }
            public int InfantCount { get; set; }
            public string PaymentMethod { get; set; } = string.Empty; // NotMapped
            public decimal TotalPrice { get; set; }
            public string Status { get; set; } = string.Empty;
            public System.DateTime BookingDate { get; set; }
            public System.DateTime CreatedDate { get; set; }
            public int? UserId { get; set; }
            public string? CreatedBy { get; set; }
            public System.Collections.Generic.List<PaymentViewModel> Payments { get; set; } = new();
        }

        public class PaymentViewModel
        {
            public decimal Amount { get; set; }
            public string PaymentMethod { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public System.DateTime PaymentDate { get; set; }
        }

        public class TourScheduleViewModel
        {
            public int ScheduleId { get; set; }
            public System.DateTime StartDate { get; set; }
            public TourDetailViewModel? Tour { get; set; }
        }
    }
}
