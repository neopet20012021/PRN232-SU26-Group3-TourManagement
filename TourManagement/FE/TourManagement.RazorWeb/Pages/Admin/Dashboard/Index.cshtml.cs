using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace TourManagement.RazorWeb.Pages.Admin.Dashboard
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public IndexModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public DashboardStatsViewModel Stats { get; set; } = new DashboardStatsViewModel();

        public async Task OnGetAsync()
        {
            var client = _clientFactory.CreateClient("API");
            var response = await client.GetAsync("api/Analytics/DashboardStats");
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<DashboardStatsViewModel>();
                if (result != null)
                {
                    Stats = result;
                }
            }
        }

        public class DashboardStatsViewModel
        {
            public int TotalTours { get; set; }
            public int TotalSchedules { get; set; }
            public int TotalUsers { get; set; }
            public int TotalBookings { get; set; }
            public int PendingBookings { get; set; }
            public decimal TotalRevenue { get; set; }
            public System.Collections.Generic.List<MonthlyRevenueItem> MonthlyRevenue { get; set; } = new();
            public System.Collections.Generic.List<StatusCountItem> StatusCounts { get; set; } = new();
        }

        public class MonthlyRevenueItem
        {
            public string MonthName { get; set; } = string.Empty;
            public decimal Revenue { get; set; }
        }

        public class StatusCountItem
        {
            public string Status { get; set; } = string.Empty;
            public int Count { get; set; }
        }
    }
}
