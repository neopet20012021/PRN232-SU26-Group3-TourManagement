using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TourManagement.RazorWeb.Pages.Tours
{
    public class BookModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public BookModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public TourDetailViewModel? Tour { get; set; }

        [BindProperty]
        public BookingInputModel BookingInput { get; set; } = new BookingInputModel();

        public string ErrorMessage { get; set; } = string.Empty;
        public string SuccessMessage { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var client = _clientFactory.CreateClient("API");
            var response = await client.GetAsync($"odata/Tours/{id}");
            if (response.IsSuccessStatusCode)
            {
                Tour = await response.Content.ReadFromJsonAsync<TourDetailViewModel>();
                BookingInput.TourId = id;

                if (User.Identity != null && User.Identity.IsAuthenticated)
                {
                    BookingInput.CustomerName = User.FindFirst("FullName")?.Value ?? User.Identity.Name ?? "";
                }
                
                return Page();
            }
            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var client = _clientFactory.CreateClient("API");
            var tourResponse = await client.GetAsync($"odata/Tours/{BookingInput.TourId}");
            if (tourResponse.IsSuccessStatusCode)
            {
                Tour = await tourResponse.Content.ReadFromJsonAsync<TourDetailViewModel>();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            int? userId = null;
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userIdClaim = User.FindFirst("UserId")?.Value;
                if (int.TryParse(userIdClaim, out int uid))
                {
                    userId = uid;
                }
            }

            var bookingPayload = new
            {
                TourId = BookingInput.TourId,
                CustomerName = BookingInput.CustomerName,
                PhoneNumber = BookingInput.PhoneNumber,
                Email = BookingInput.Email,
                AdultCount = BookingInput.AdultCount,
                ChildCount = BookingInput.ChildCount,
                InfantCount = BookingInput.InfantCount,
                RoomType = BookingInput.RoomType,
                SpecialRequest = BookingInput.SpecialRequest,
                PaymentMethod = BookingInput.PaymentMethod,
                PromoCode = BookingInput.PromoCode,
                UserId = userId,
                CreatedBy = User.Identity?.Name 
            };

            var content = new StringContent(JsonSerializer.Serialize(bookingPayload), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("odata/Bookings", content);

            if (response.IsSuccessStatusCode)
            {
                SuccessMessage = "Booking successful! Your order is Pending.";
                ModelState.Clear();
                // To prevent resubmission or hide the form, we keep it as is since SuccessMessage handles it.
                return Page();
            }
            else
            {
                var err = await response.Content.ReadAsStringAsync();
                ErrorMessage = "Failed to book tour. " + err;
                return Page();
            }
        }
    }

    public class BookingInputModel
    {
        [Required]
        public int TourId { get; set; }

        [Required(ErrorMessage = "Full Name is required")]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone Number is required")]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Range(1, 100)]
        public int AdultCount { get; set; } = 1;

        [Required]
        [Range(0, 100)]
        public int ChildCount { get; set; } = 0;

        [Required]
        [Range(0, 100)]
        public int InfantCount { get; set; } = 0;

        public string? RoomType { get; set; }
        public string? SpecialRequest { get; set; }

        [Required(ErrorMessage = "Please select a payment method")]
        public string PaymentMethod { get; set; } = string.Empty;

        public string? PromoCode { get; set; }
    }
}
