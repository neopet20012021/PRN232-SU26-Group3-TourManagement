using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TourManagement.RazorWeb.Pages
{
    [Authorize]
    public class ProfileModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public ProfileModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public decimal TotalSpent { get; set; }
        public int TotalToursBooked { get; set; }

        public async Task OnGetAsync()
        {
            FullName = User.FindFirst("FullName")?.Value ?? User.Identity?.Name ?? "Unknown";
            Email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? "N/A";
            Username = User.Identity?.Name ?? "N/A";
            Role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? "Customer";

            var client = _clientFactory.CreateClient("API");
            var userId = User.FindFirst("UserId")?.Value;
            
            var filterQuery = string.IsNullOrEmpty(userId) 
                ? $"CreatedBy eq '{User.Identity?.Name}'" 
                : $"(UserId eq {userId} or CreatedBy eq '{User.Identity?.Name}')";

            var response = await client.GetAsync($"odata/Bookings?$filter={filterQuery}");
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ODataResponse<BookingBasic>>();
                if (result != null && result.Value != null)
                {
                    // Only sum Paid or Pending tours, avoid counting Cancelled ones
                    var validBookings = result.Value.Where(b => b.Status.ToLower() != "cancelled").ToList();
                    TotalToursBooked = validBookings.Count;
                    TotalSpent = validBookings.Sum(b => b.TotalPrice);
                }
            }
        }

        public class ODataResponse<T>
        {
            public List<T> Value { get; set; } = new List<T>();
        }

        public class BookingBasic
        {
            public decimal TotalPrice { get; set; }
            public string Status { get; set; } = string.Empty;
        }
    }
}
