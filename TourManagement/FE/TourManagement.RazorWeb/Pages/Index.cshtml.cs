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

    public async Task OnGetAsync()
    {
        var client = _clientFactory.CreateClient("API");
        var response = await client.GetAsync("odata/TourSchedules?$expand=Tour&$filter=Status eq 'Active'&$orderby=StartDate asc");
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<ODataResponse<TourScheduleViewModel>>();
            if (result != null && result.Value != null)
            {
                Schedules = result.Value;
            }
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
}
