using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TourManagement.RazorWeb.Pages.Admin.PromoCodes
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public CreateModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
            // Default dates
            Input.StartDate = DateTime.Now;
            Input.EndDate = DateTime.Now.AddDays(30);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Input.EndDate <= Input.StartDate)
            {
                ModelState.AddModelError("Input.EndDate", "Ngày kết thúc phải sau ngày bắt đầu.");
                return Page();
            }

            var client = _clientFactory.CreateClient("API");
            var promoCode = new
            {
                Code = Input.Code.ToUpper().Trim(),
                DiscountPercent = Input.DiscountPercent / 100m, // Convert e.g. 10% to 0.10
                MinBookingValue = Input.MinBookingValue,
                StartDate = DateTime.SpecifyKind(Input.StartDate, DateTimeKind.Utc),
                EndDate = DateTime.SpecifyKind(Input.EndDate, DateTimeKind.Utc),
                MaxUsage = Input.MaxUsage,
                UsageCount = 0,
                IsActive = Input.IsActive
            };

            var response = await client.PostAsJsonAsync("odata/PromoCodes", promoCode);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("./Index");
            }

            ErrorMessage = await response.Content.ReadAsStringAsync();
            if (ErrorMessage != null && (ErrorMessage.Contains("already exists") || ErrorMessage.Contains("tồn tại")))
            {
                ModelState.AddModelError("Input.Code", "Mã khuyến mãi này đã tồn tại.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi gọi API: " + ErrorMessage);
            }

            return Page();
        }

        public class InputModel
        {
            [Required(ErrorMessage = "Mã khuyến mãi là bắt buộc")]
            [StringLength(50, MinimumLength = 3, ErrorMessage = "Độ dài mã từ 3 đến 50 ký tự")]
            [RegularExpression(@"^[a-zA-Z0-9_\-]+$", ErrorMessage = "Mã chỉ chứa chữ, số, gạch dưới và gạch ngang")]
            public string Code { get; set; } = string.Empty;

            [Required(ErrorMessage = "Phần trăm giảm giá là bắt buộc")]
            [Range(1, 100, ErrorMessage = "Tỷ lệ giảm giá phải từ 1% đến 100%")]
            public decimal DiscountPercent { get; set; }

            [Required(ErrorMessage = "Giá trị đơn hàng tối thiểu là bắt buộc")]
            [Range(0, 1000000000, ErrorMessage = "Giá trị đơn hàng tối thiểu không hợp lệ")]
            public decimal MinBookingValue { get; set; }

            [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc")]
            public DateTime StartDate { get; set; }

            [Required(ErrorMessage = "Ngày kết thúc là bắt buộc")]
            public DateTime EndDate { get; set; }

            [Required(ErrorMessage = "Lượt sử dụng tối đa là bắt buộc")]
            [Range(1, int.MaxValue, ErrorMessage = "Số lượt sử dụng phải từ 1 trở lên")]
            public int MaxUsage { get; set; } = 100;

            public bool IsActive { get; set; } = true;
        }
    }
}
