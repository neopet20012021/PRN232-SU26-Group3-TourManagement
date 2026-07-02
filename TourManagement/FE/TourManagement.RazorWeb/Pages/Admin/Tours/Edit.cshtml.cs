using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TourManagement.RazorWeb.Pages.Admin.Tours
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
        public TourEditViewModel Tour { get; set; } = new TourEditViewModel();

        [TempData]
        public string? SuccessMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var client = _clientFactory.CreateClient("API");
            var response = await client.GetAsync($"odata/Tours({id})");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<TourEditViewModel>();
                if (data != null)
                {
                    Tour = data;
                    return Page();
                }
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (Tour.Nights > Tour.Days || Tour.Nights < Tour.Days - 1)
            {
                ModelState.AddModelError("Tour.Nights", "Số đêm không hợp lệ so với số ngày (phải bằng số ngày hoặc nhỏ hơn 1 ngày).");
                return Page();
            }

            var client = _clientFactory.CreateClient("API");
            var response = await client.PutAsJsonAsync($"odata/Tours({id})", Tour);

            if (response.IsSuccessStatusCode)
            {
                SuccessMessage = "Cập nhật thông tin tour thành công!";
                return RedirectToPage("./Index");
            }

            ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi cập nhật tour. Vui lòng thử lại.");
            return Page();
        }

        public class TourEditViewModel
        {
            public int TourId { get; set; }

            [Required(ErrorMessage = "Tên tour là bắt buộc")]
            [StringLength(150, ErrorMessage = "Tên tour không được vượt quá 150 ký tự")]
            public string TourName { get; set; } = string.Empty;

            [StringLength(20)]
            [Required(ErrorMessage = "Mã tour là bắt buộc")]
            public string TourCode { get; set; } = string.Empty;

            [Required(ErrorMessage = "Mô tả là bắt buộc")]
            public string Description { get; set; } = string.Empty;

            [Required]
            [Range(1, 30, ErrorMessage = "Số ngày phải từ 1 đến 30")]
            public int Days { get; set; }

            [Required]
            [Range(0, 30, ErrorMessage = "Số đêm phải từ 0 đến 30")]
            public int Nights { get; set; }

            [Required(ErrorMessage = "Giá người lớn là bắt buộc")]
            [Range(0, 1000000000)]
            public decimal PricePerAdult { get; set; }

            [Required]
            [Range(0, 1000000000)]
            public decimal ChildPrice { get; set; }

            [StringLength(100)]
            [Required(ErrorMessage = "Thể loại là bắt buộc")]
            public string Category { get; set; } = string.Empty;

            [StringLength(100)]
            [Required(ErrorMessage = "Điểm đến là bắt buộc")]
            public string Destination { get; set; } = string.Empty;

            [Required]
            [Range(1, 200)]
            public int MaxCapacity { get; set; }

            [Required]
            public bool IsActive { get; set; } = true;

            [StringLength(500)]
            public string? Itinerary { get; set; }

            [StringLength(500)]
            public string? IncludedServices { get; set; }

            [StringLength(500)]
            public string? ExcludedServices { get; set; }

            [StringLength(200)]
            public string? Image { get; set; }
        }
    }
}
