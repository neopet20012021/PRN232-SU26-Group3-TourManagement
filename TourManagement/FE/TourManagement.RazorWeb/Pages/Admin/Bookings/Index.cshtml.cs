using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TourManagement.RazorWeb.Pages.Tours; // To reuse ViewModels

namespace TourManagement.RazorWeb.Pages.Admin.Bookings
{
    [Authorize(Roles = "Admin, Staff")]
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public IndexModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public List<BookingAdminViewModel> Bookings { get; set; } = new List<BookingAdminViewModel>();

        public async Task OnGetAsync()
        {
            var client = _clientFactory.CreateClient("API");
            var response = await client.GetAsync("odata/Bookings?$expand=Tour&$orderby=BookingDate desc");
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ODataResponse<BookingAdminViewModel>>();
                if (result != null && result.Value != null)
                {
                    Bookings = result.Value;
                }
            }
        }

        public class ODataResponse<T>
        {
            public List<T> Value { get; set; } = new List<T>();
        }

        public class BookingAdminViewModel
        {
            public int BookingId { get; set; }
            public string BookingCode { get; set; } = string.Empty;
            public int TourId { get; set; }
            public TourDetailViewModel? Tour { get; set; }
            public string CustomerName { get; set; } = string.Empty;
            public string PhoneNumber { get; set; } = string.Empty;
            public int AdultCount { get; set; }
            public int ChildCount { get; set; }
            public int InfantCount { get; set; }
            public decimal TotalPrice { get; set; }
            public string Status { get; set; } = string.Empty;
            public System.DateTime BookingDate { get; set; }
        }
    }
}
