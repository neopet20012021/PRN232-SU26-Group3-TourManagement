using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Json;

namespace TourManagement.RazorWeb.Pages;

public class IndexModel : PageModel
{
    private readonly IHttpClientFactory _clientFactory;

    public IndexModel(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public List<TourScheduleViewModel> Schedules { get; set; } = new List<TourScheduleViewModel>();
    public List<ReviewViewModel> RecentReviews { get; set; } = new List<ReviewViewModel>();

    [BindProperty(SupportsGet = true)]
    public string? Keyword { get; set; }

    [BindProperty(SupportsGet = true)]
    public decimal? MinPrice { get; set; }

    [BindProperty(SupportsGet = true)]
    public decimal? MaxPrice { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Duration { get; set; }

    public async Task OnGetAsync()
    {
        var client = _clientFactory.CreateClient("API");
        
        var filter = "Status eq 'Active'";

        if (!string.IsNullOrWhiteSpace(Keyword))
        {
            var kw = Keyword.ToLower().Replace("'", "''");
            filter += $" and (contains(tolower(Tour/TourName), '{kw}') or contains(tolower(Tour/Destination), '{kw}'))";
        }

        if (MinPrice.HasValue)
        {
            filter += $" and ActualAdultPrice ge {MinPrice.Value}";
        }

        if (MaxPrice.HasValue)
        {
            filter += $" and ActualAdultPrice le {MaxPrice.Value}";
        }

        if (!string.IsNullOrWhiteSpace(Duration))
        {
            if (Duration == "1-3") filter += " and Tour/Days le 3";
            else if (Duration == "4-5") filter += " and Tour/Days ge 4 and Tour/Days le 5";
            else if (Duration == "6+") filter += " and Tour/Days ge 6";
        }

        var requestUri = $"odata/TourSchedules?$expand=Tour&$filter={filter}&$orderby=StartDate asc";
        var response = await client.GetAsync(requestUri);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<ODataResponse<TourScheduleViewModel>>();
            if (result != null && result.Value != null)
            {
                Schedules = result.Value;
            }
        }

        // Fetch recent reviews
        try
        {
            var reviewsResponse = await client.GetAsync("odata/Reviews?$expand=Tour&$orderby=CreatedDate desc&$top=10");
            if (reviewsResponse.IsSuccessStatusCode)
            {
                var reviewsResult = await reviewsResponse.Content.ReadFromJsonAsync<ODataResponse<ReviewViewModel>>();
                if (reviewsResult != null && reviewsResult.Value != null)
                {
                    RecentReviews = reviewsResult.Value;
                }
            }
        }
        catch
        {
            // Fail silently
        }
    }

    public class ODataResponse<T>
    {
        public List<T> Value { get; set; } = new List<T>();
    }

    public class TourScheduleViewModel
    {
        public int ScheduleId { get; set; }
        public DateTime StartDate { get; set; }
        public int AvailableSeats { get; set; }
        public decimal ActualAdultPrice { get; set; }
        public TourViewModel Tour { get; set; }
    }

    public class TourViewModel
    {
        public int TourId { get; set; }
        public string TourName { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public int Days { get; set; }
        public int Nights { get; set; }
    }

    public class ReviewViewModel
    {
        public int ReviewId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public int TourId { get; set; }
        public TourViewModel? Tour { get; set; }
    }
}
