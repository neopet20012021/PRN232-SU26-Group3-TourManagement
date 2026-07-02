using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TourManagement.RazorWeb.Pages
{
    [Authorize(Roles = "Customer")]
    public class MyBookingsModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public MyBookingsModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public List<BookingViewModel> Bookings { get; set; } = new List<BookingViewModel>();

        public async Task OnGetAsync()
        {
            var client = _clientFactory.CreateClient("API");
            
            var userId = User.FindFirst("UserId")?.Value;
            var filterQuery = string.IsNullOrEmpty(userId) 
                ? $"CreatedBy eq '{User.Identity?.Name}'" 
                : $"(UserId eq {userId} or CreatedBy eq '{User.Identity?.Name}')";

            var response = await client.GetAsync($"odata/Bookings?$expand=Schedule($expand=Tour)&$filter={filterQuery}&$orderby=BookingDate desc");
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ODataResponse<BookingViewModel>>();
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

        public class BookingViewModel
        {
            public int BookingId { get; set; }
            public string BookingCode { get; set; } = string.Empty;
            public int ScheduleId { get; set; }
            public ScheduleViewModel? Schedule { get; set; }
            public int AdultCount { get; set; }
            public int ChildCount { get; set; }
            public int InfantCount { get; set; }
            public decimal TotalPrice { get; set; }
            public decimal FinalPrice { get; set; }
            public string Status { get; set; } = string.Empty;
            public DateTime BookingDate { get; set; }
        }

        public class ScheduleViewModel
        {
            public DateTime StartDate { get; set; }
            public TourViewModel? Tour { get; set; }
        }

        public class TourViewModel
        {
            public string TourName { get; set; } = string.Empty;
        }
    }
}
