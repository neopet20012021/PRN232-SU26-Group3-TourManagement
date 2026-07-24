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

        public ScheduleDetailViewModel? Schedule { get; set; }

        [BindProperty]
        public BookingInputModel BookingInput { get; set; } = new BookingInputModel();

        public string ErrorMessage { get; set; } = string.Empty;
        public string SuccessMessage { get; set; } = string.Empty;

        public bool ShowWelcomeAlert { get; set; }

        public async Task<IActionResult> OnGetAsync(int scheduleId)
        {
            var client = _clientFactory.CreateClient("API");
            var response = await client.GetAsync($"odata/TourSchedules/{scheduleId}?$expand=Tour");
            if (response.IsSuccessStatusCode)
            {
                Schedule = await response.Content.ReadFromJsonAsync<ScheduleDetailViewModel>();
                BookingInput.ScheduleId = scheduleId;

                if (User.Identity != null && User.Identity.IsAuthenticated)
                {
                    BookingInput.CustomerName = User.FindFirst("FullName")?.Value ?? User.Identity.Name ?? "";
                    var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value 
                                ?? User.FindFirst("Email")?.Value 
                                ?? User.Identity.Name ?? "";
                    BookingInput.Email = email;

                    // Kiểm tra xem user này đã từng đặt tour nào chưa
                    var userId = User.FindFirst("UserId")?.Value;
                    var filterQuery = string.IsNullOrEmpty(userId) 
                        ? $"Email eq '{email}'" 
                        : $"(UserId eq {userId} or Email eq '{email}')";

                    var bookingCheck = await client.GetAsync($"odata/Bookings?$filter={filterQuery}&$top=1");
                    if (bookingCheck.IsSuccessStatusCode)
                    {
                        var checkContent = await bookingCheck.Content.ReadFromJsonAsync<ODataCheckResponse>();
                        if (checkContent == null || checkContent.Value == null || checkContent.Value.Count == 0)
                        {
                            // Chưa từng đặt tour -> Tự động áp mã WELCOME và hiện thông báo
                            BookingInput.PromoCode = "WELCOME";
                            ShowWelcomeAlert = true;
                        }
                    }
                }
                
                return Page();
            }
            return NotFound();
        }

        public class ODataCheckResponse
        {
            public System.Collections.Generic.List<object> Value { get; set; } = new();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var client = _clientFactory.CreateClient("API");
            var tourResponse = await client.GetAsync($"odata/TourSchedules/{BookingInput.ScheduleId}?$expand=Tour");
            if (tourResponse.IsSuccessStatusCode)
            {
                Schedule = await tourResponse.Content.ReadFromJsonAsync<ScheduleDetailViewModel>();
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
                BookingId = 0,
                BookingCode = "TEMP", 
                CreatedDate = DateTime.UtcNow,
                BookingDate = DateTime.UtcNow,
                ScheduleId = BookingInput.ScheduleId,
                CustomerName = BookingInput.CustomerName,
                PhoneNumber = BookingInput.PhoneNumber,
                Email = BookingInput.Email,
                AdultCount = BookingInput.AdultCount,
                ChildCount = BookingInput.ChildCount,
                InfantCount = BookingInput.InfantCount,

                SpecialRequest = BookingInput.SpecialRequest ?? "",
                PaymentMethod = BookingInput.PaymentMethod ?? "Cash",
                PromoCode = BookingInput.PromoCode ?? "",
                UserId = userId,

                Status = "Pending",
                TotalPrice = 0m,
                DiscountAmount = 0m,
                FinalPrice = 0m
            };

            var content = new StringContent(JsonSerializer.Serialize(bookingPayload), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("odata/Bookings", content);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var createdBooking = JsonSerializer.Deserialize<JsonElement>(responseContent);
                string bookingCode = createdBooking.GetProperty("BookingCode").GetString() ?? "";

                return RedirectToPage("/BookingDetail", new { bookingCode = bookingCode });
            }
            else
            {
                var err = await response.Content.ReadAsStringAsync();
                ErrorMessage = "Failed to book tour. " + err;
                return Page();
            }
        }

        public async Task<IActionResult> OnPostCalculatePriceAsync([FromBody] PriceCalculationRequest request)
        {
            var client = _clientFactory.CreateClient("API");
            
            // Lấy UserId từ session đăng nhập hiện tại - không phụ thuộc vào email người dùng điền vào form
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                var userIdClaim = User.FindFirst("UserId")?.Value;
                if (int.TryParse(userIdClaim, out int uid))
                {
                    request.UserId = uid;
                }
            }

            var response = await client.PostAsJsonAsync("odata/Bookings/calculate-price", request);
            var result = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                return Content(result, "application/json");
            }
            return Content(result, "application/json"); // Trả về nội dung JSON chứa message lỗi để JS hiển thị
        }
    }

    public class PriceCalculationRequest
    {
        public int ScheduleId { get; set; }
        public int AdultCount { get; set; }
        public int ChildCount { get; set; }
        public string? PromoCode { get; set; }
        public string? UserEmail { get; set; }
        public int? UserId { get; set; }
    }

    public class BookingInputModel
    {
        [Required]
        public int ScheduleId { get; set; }

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


        public string? SpecialRequest { get; set; }

        [Required(ErrorMessage = "Please select a payment method")]
        public string PaymentMethod { get; set; } = string.Empty;

        public string? PromoCode { get; set; }
    }

    public class ScheduleDetailViewModel
    {
        public int ScheduleId { get; set; }
        public DateTime StartDate { get; set; }
        public decimal ActualAdultPrice { get; set; }
        public decimal ActualChildPrice { get; set; }
        public int AvailableSeats { get; set; }
        public TourDetailViewModel? Tour { get; set; }
    }

    public class TourDetailViewModel
    {
        public int TourId { get; set; }
        public string TourName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Itinerary { get; set; } = string.Empty;
        public string IncludedServices { get; set; } = string.Empty;
        public string ExcludedServices { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public int Days { get; set; }
        public int Nights { get; set; }
    }
}
