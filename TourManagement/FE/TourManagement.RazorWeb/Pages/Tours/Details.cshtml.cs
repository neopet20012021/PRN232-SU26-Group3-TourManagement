using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TourManagement.RazorWeb.Pages.Tours
{
    public class DetailsModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public DetailsModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public ScheduleDetailViewModel? Schedule { get; set; }
        public List<ScheduleDetailViewModel> RelatedSchedules { get; set; } = new List<ScheduleDetailViewModel>();
        public ReviewSummaryViewModel? ReviewSummary { get; set; }

        [BindProperty]
        public CreateReviewInputModel NewReview { get; set; } = new CreateReviewInputModel();

        public string? ReviewErrorMessage { get; set; }
        public string? ReviewSuccessMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(int scheduleId)
        {
            var client = _clientFactory.CreateClient("API");
            var response = await client.GetAsync($"odata/TourSchedules/{scheduleId}?$expand=Tour");
            if (response.IsSuccessStatusCode)
            {
                Schedule = await response.Content.ReadFromJsonAsync<ScheduleDetailViewModel>();

                // Fetch other active tour schedules
                var allResponse = await client.GetAsync("odata/TourSchedules?$expand=Tour&$filter=Status eq 'Active'");
                if (allResponse.IsSuccessStatusCode)
                {
                    var result = await allResponse.Content.ReadFromJsonAsync<ODataResponse<ScheduleDetailViewModel>>();
                    if (result != null && result.Value != null)
                    {
                        // Exclude the current schedule and take up to 3 tours
                        RelatedSchedules = result.Value
                            .Where(x => x.ScheduleId != scheduleId && x.Tour != null)
                            .Take(3)
                            .ToList();
                    }
                }

                if (Schedule?.Tour != null)
                {
                    await LoadReviewsAsync(client, Schedule.Tour.TourId);
                }

                if (User.Identity != null && User.Identity.IsAuthenticated)
                {
                    NewReview.CustomerName = User.FindFirst("FullName")?.Value ?? User.Identity.Name ?? "";
                }

                return Page();
            }
            return NotFound();
        }

        public async Task<IActionResult> OnPostReviewAsync(int scheduleId)
        {
            var client = _clientFactory.CreateClient("API");
            var response = await client.GetAsync($"odata/TourSchedules/{scheduleId}?$expand=Tour");
            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            Schedule = await response.Content.ReadFromJsonAsync<ScheduleDetailViewModel>();

            if (Schedule?.Tour == null)
            {
                return NotFound();
            }

            NewReview.TourId = Schedule.Tour.TourId;

            if (!ModelState.IsValid)
            {
                ReviewErrorMessage = "Vui lòng kiểm tra lại thông tin đánh giá (Chọn số sao cho các tiêu chí và nhập nội dung nhận xét).";
                await LoadReviewsAsync(client, Schedule.Tour.TourId);
                return Page();
            }

            var postResponse = await client.PostAsJsonAsync("api/reviews", NewReview);
            if (postResponse.IsSuccessStatusCode)
            {
                ReviewSuccessMessage = "Cảm ơn bạn đã gửi đánh giá chi tiết cho chuyến đi này!";
                NewReview.Comment = string.Empty;
                NewReview.Rating = 5;
                NewReview.CleanlinessRating = 5;
                NewReview.ComfortRating = 5;
                NewReview.AmenitiesRating = 5;
                NewReview.ValueRating = 5;
            }
            else
            {
                var errorStr = await postResponse.Content.ReadAsStringAsync();
                ReviewErrorMessage = $"Gửi đánh giá không thành công: {errorStr}";
            }

            await LoadReviewsAsync(client, Schedule.Tour.TourId);
            return Page();
        }

        private async Task LoadReviewsAsync(HttpClient client, int tourId)
        {
            try
            {
                var summaryResponse = await client.GetAsync($"api/reviews/tour/{tourId}/summary");
                if (summaryResponse.IsSuccessStatusCode)
                {
                    ReviewSummary = await summaryResponse.Content.ReadFromJsonAsync<ReviewSummaryViewModel>();
                }
            }
            catch
            {
                ReviewSummary = new ReviewSummaryViewModel { TourId = tourId };
            }
        }
    }

    public class CreateReviewInputModel
    {
        public int TourId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên của bạn")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn số sao đánh giá chung")]
        [Range(1, 5)]
        public int Rating { get; set; } = 5;

        [Range(1, 5)]
        public int CleanlinessRating { get; set; } = 5;

        [Range(1, 5)]
        public int ComfortRating { get; set; } = 5;

        [Range(1, 5)]
        public int AmenitiesRating { get; set; } = 5;

        [Range(1, 5)]
        public int ValueRating { get; set; } = 5;

        [Required(ErrorMessage = "Vui lòng nhập nhận xét")]
        [StringLength(1000, ErrorMessage = "Nhận xét không dài quá 1000 ký tự")]
        public string Comment { get; set; } = string.Empty;
    }

    public class ReviewSummaryViewModel
    {
        public int TourId { get; set; }
        public double AverageRating { get; set; }
        public double AvgCleanliness { get; set; }
        public double AvgComfort { get; set; }
        public double AvgAmenities { get; set; }
        public double AvgValue { get; set; }
        public int TotalReviews { get; set; }
        public Dictionary<int, int> StarCounts { get; set; } = new Dictionary<int, int>();
        public List<ReviewItemViewModel> Reviews { get; set; } = new List<ReviewItemViewModel>();
    }

    public class ReviewItemViewModel
    {
        public int ReviewId { get; set; }
        public int TourId { get; set; }
        public int? UserId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public int CleanlinessRating { get; set; }
        public int ComfortRating { get; set; }
        public int AmenitiesRating { get; set; }
        public int ValueRating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }

    public class ODataResponse<T>
    {
        public List<T> Value { get; set; } = new List<T>();
    }
}
