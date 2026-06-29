using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TourManagement.RazorWeb.Pages.Staff.Schedules
{
    [Authorize(Roles = "Admin, Staff")]
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public IndexModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public List<ScheduleViewModel> Schedules { get; set; } = new List<ScheduleViewModel>();

        public async Task OnGetAsync()
        {
            var client = _clientFactory.CreateClient("API");
            var response = await client.GetAsync("odata/TourSchedules?$expand=Tour&$orderby=StartDate desc");
            
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ODataResponse<ScheduleViewModel>>();
                if (result != null && result.Value != null)
                {
                    Schedules = result.Value;
                }
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var client = _clientFactory.CreateClient("API");
            var response = await client.DeleteAsync($"odata/TourSchedules/{id}");
            return RedirectToPage();
        }

        public class ODataResponse<T>
        {
            public List<T> Value { get; set; } = new List<T>();
        }

        public class ScheduleViewModel
        {
            public int ScheduleId { get; set; }
            public int TourId { get; set; }
            public System.DateTime StartDate { get; set; }
            public System.DateTime EndDate { get; set; }
            public int MaxParticipants { get; set; }
            public int AvailableSeats { get; set; }
            public decimal ActualAdultPrice { get; set; }
            public string Status { get; set; } = string.Empty;
            public TourViewModel Tour { get; set; } = new();
        }

        public class TourViewModel
        {
            public string TourName { get; set; } = string.Empty;
            public string TourCode { get; set; } = string.Empty;
        }
    }
}
