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
    public class DetailsModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public DetailsModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public TourDetailViewModel? Tour { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var client = _clientFactory.CreateClient("API");
            var response = await client.GetAsync($"odata/Tours/{id}");
            if (response.IsSuccessStatusCode)
            {
                Tour = await response.Content.ReadFromJsonAsync<TourDetailViewModel>();
                return Page();
            }
            return NotFound();
        }
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
        public decimal PricePerAdult { get; set; }
        public decimal ChildPrice { get; set; }
        public DateTime DepartureDate { get; set; }
        public int AvailableSeats { get; set; }
    }
}
