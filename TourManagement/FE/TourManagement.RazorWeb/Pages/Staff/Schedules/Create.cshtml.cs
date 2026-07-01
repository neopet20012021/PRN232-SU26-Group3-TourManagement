using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TourManagement.RazorWeb.Pages.Staff.Schedules
{
    [Authorize(Roles = "Staff")]
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public CreateModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [BindProperty]
        public ScheduleCreateViewModel Schedule { get; set; } = new();

        public List<SelectListItem> TourList { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadToursAsync();
            Schedule.StartDate = DateTime.Now.AddDays(7);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadToursAsync();
                return Page();
            }

            if (Schedule.StartDate.Date < DateTime.Now.Date)
            {
                ModelState.AddModelError("Schedule.StartDate", "Ngày khởi hành phải từ ngày hôm nay trở đi.");
                await LoadToursAsync();
                return Page();
            }

            var client = _clientFactory.CreateClient("API");
            var payload = new
            {
                ScheduleId = 0,
                TourId = Schedule.TourId,
                StartDate = DateTime.SpecifyKind(Schedule.StartDate, DateTimeKind.Utc),
                EndDate = DateTime.SpecifyKind(Schedule.StartDate, DateTimeKind.Utc), // Dummy value to pass OData required check
                MaxParticipants = Schedule.MaxParticipants,
                AvailableSeats = Schedule.MaxParticipants, // Dummy value to pass OData required check
                ActualAdultPrice = Schedule.ActualAdultPrice,
                ActualChildPrice = Schedule.ActualChildPrice,
                GuideName = Schedule.GuideName ?? "",
                Status = "Active",
                CreatedDate = DateTime.UtcNow
            };

            var json = System.Text.Json.JsonSerializer.Serialize(payload);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync("odata/TourSchedules", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Ngày khởi hành đã được tạo thành công!";
                // TODO: Redirect to Schedules Index when created
                return RedirectToPage("/Index"); 
            }

            var errorBody = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi tạo ngày khởi hành. " + errorBody);
            await LoadToursAsync();
            return Page();
        }

        private async Task LoadToursAsync()
        {
            var client = _clientFactory.CreateClient("API");
            var response = await client.GetAsync("odata/Tours?$filter=IsActive eq true");
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ODataResponse<TourViewModel>>();
                if (result != null && result.Value != null)
                {
                    foreach (var tour in result.Value)
                    {
                        TourList.Add(new SelectListItem 
                        { 
                            Value = tour.TourId.ToString(), 
                            Text = $"{tour.TourCode} - {tour.TourName} ({tour.Days}N{tour.Nights}Đ)" 
                        });
                    }
                }
            }
        }

        public class ODataResponse<T>
        {
            public List<T> Value { get; set; } = new List<T>();
        }

        public class ScheduleCreateViewModel
        {
            [Required(ErrorMessage = "Vui lòng chọn Tour gốc")]
            public int TourId { get; set; }

            [Required(ErrorMessage = "Ngày khởi hành là bắt buộc")]
            public DateTime StartDate { get; set; }

            [Required(ErrorMessage = "Số người tối đa là bắt buộc")]
            [Range(1, 500, ErrorMessage = "Số người tối đa phải từ 1 đến 500")]
            public int MaxParticipants { get; set; }

            [Required(ErrorMessage = "Giá thực tế (Người lớn) là bắt buộc")]
            [Range(0, double.MaxValue, ErrorMessage = "Giá không hợp lệ")]
            public decimal ActualAdultPrice { get; set; }

            [Required(ErrorMessage = "Giá thực tế (Trẻ em) là bắt buộc")]
            [Range(0, double.MaxValue, ErrorMessage = "Giá không hợp lệ")]
            public decimal ActualChildPrice { get; set; }

            public string GuideName { get; set; } = string.Empty;
        }

        public class TourViewModel
        {
            public int TourId { get; set; }
            public string TourCode { get; set; } = string.Empty;
            public string TourName { get; set; } = string.Empty;
            public int Days { get; set; }
            public int Nights { get; set; }
        }
    }
}
