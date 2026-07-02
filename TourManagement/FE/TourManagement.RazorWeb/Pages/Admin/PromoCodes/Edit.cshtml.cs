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
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public EditModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var client = _clientFactory.CreateClient("API");
            var response = await client.GetAsync($"odata/PromoCodes({id})");

            if (!response.IsSuccessStatusCode)
            {
                return RedirectToPage("./Index");
            }

            var promoCode = await response.Content.ReadFromJsonAsync<PromoCodeDto>();
            if (promoCode == null)
            {
                return RedirectToPage("./Index");
            }

            Input = new InputModel
            {
                PromoCodeId = promoCode.PromoCodeId,
                Code = promoCode.Code,
                DiscountPercent = promoCode.DiscountPercent * 100m, // Convert back to percentage
                MinBookingValue = promoCode.MinBookingValue,
                StartDate = promoCode.StartDate,
                EndDate = promoCode.EndDate,
                MaxUsage = promoCode.MaxUsage,
                UsageCount = promoCode.UsageCount,
                IsActive = promoCode.IsActive
            };

            return Page();
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
                PromoCodeId = Input.PromoCodeId,
                Code = Input.Code.ToUpper().Trim(),
                DiscountPercent = Input.DiscountPercent / 100m, // Convert percentage to decimal
                MinBookingValue = Input.MinBookingValue,
                StartDate = Input.StartDate.ToUniversalTime(),
                EndDate = Input.EndDate.ToUniversalTime(),
                MaxUsage = Input.MaxUsage,
                UsageCount = Input.UsageCount,
                IsActive = Input.IsActive
            };

            var response = await client.PutAsJsonAsync($"odata/PromoCodes({Input.PromoCodeId})", promoCode);

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
            public int PromoCodeId { get; set; }

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
            public int MaxUsage { get; set; }

            public int UsageCount { get; set; }

            public bool IsActive { get; set; }
        }

        public class PromoCodeDto
        {
            public int PromoCodeId { get; set; }
            public string Code { get; set; } = string.Empty;
            public decimal DiscountPercent { get; set; }
            public decimal MinBookingValue { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public int MaxUsage { get; set; }
            public int UsageCount { get; set; }
            public bool IsActive { get; set; }
        }
    }
}
