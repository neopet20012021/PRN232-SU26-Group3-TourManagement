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

    public List<TourViewModel> Tours { get; set; } = new List<TourViewModel>();

    public async Task OnGetAsync()
    {
        var client = _clientFactory.CreateClient("API");
        var response = await client.GetAsync("odata/Tours?$filter=IsActive eq true&$orderby=DepartureDate asc");
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<ODataResponse<TourViewModel>>();
            if (result != null && result.Value != null)
            {
                Tours = result.Value;
            }
        }
    }

    public class ODataResponse<T>
    {
        public List<T> Value { get; set; } = new List<T>();
    }

    public class TourViewModel
    {
        public int TourId { get; set; }
        public string TourName { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public int Days { get; set; }
        public int Nights { get; set; }
        public decimal PricePerAdult { get; set; }
        public DateTime DepartureDate { get; set; }
        public int AvailableSeats { get; set; }
    }
}
