using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.RazorWeb.Pages.Tours;

namespace TourManagement.RazorWeb.Pages.Admin.Tours
{
    [Authorize(Roles = "Admin, Staff")]
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public IndexModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public List<TourAdminViewModel> Tours { get; set; } = new List<TourAdminViewModel>();

        public async Task OnGetAsync()
        {
            var client = _clientFactory.CreateClient("API");
            var response = await client.GetAsync("odata/Tours?$orderby=CreatedDate desc");
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ODataResponse<TourAdminViewModel>>();
                if (result != null && result.Value != null)
                {
                    Tours = result.Value;
                }
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            if (!User.IsInRole("Admin"))
            {
                return Forbid();
            }
            
            var client = _clientFactory.CreateClient("API");
            var response = await client.DeleteAsync($"odata/Tours/{id}");
            
            return RedirectToPage();
        }

        public class ODataResponse<T>
        {
            public List<T> Value { get; set; } = new List<T>();
        }

        public class TourAdminViewModel
        {
            public int TourId { get; set; }
            public string TourName { get; set; } = string.Empty;
            public string TourCode { get; set; } = string.Empty;
            public string Destination { get; set; } = string.Empty;
            public int Days { get; set; }
            public int Nights { get; set; }
            public decimal PricePerAdult { get; set; }
            public int AvailableSeats { get; set; }
            public int MaxCapacity { get; set; }
            public bool IsActive { get; set; }
        }
    }
}
