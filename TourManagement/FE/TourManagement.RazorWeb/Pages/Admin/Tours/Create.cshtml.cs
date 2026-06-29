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
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public CreateModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [BindProperty]
        public TourCreateViewModel Tour { get; set; } = new TourCreateViewModel();

        public void OnGet()
        {
            // Default values
            Tour.IsActive = true;
            Tour.MaxCapacity = 20;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var client = _clientFactory.CreateClient("API");
            var response = await client.PostAsJsonAsync("odata/Tours", Tour);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("./Index");
            }

            ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi tạo tour. Vui lòng thử lại.");
            return Page();
        }

        public class TourCreateViewModel
        {
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
