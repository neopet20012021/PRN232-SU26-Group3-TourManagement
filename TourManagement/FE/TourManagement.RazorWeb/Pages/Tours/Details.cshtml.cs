using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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

        public ScheduleDetailViewModel? Schedule { get; set; }
        public List<ScheduleDetailViewModel> RelatedSchedules { get; set; } = new List<ScheduleDetailViewModel>();

        public async Task<IActionResult> OnGetAsync(int scheduleId)
        {
            var client = _clientFactory.CreateClient("API");
            var response = await client.GetAsync($"odata/TourSchedules/{scheduleId}?$expand=Tour");
            if (response.IsSuccessStatusCode)
            {
                Schedule = await response.Content.ReadFromJsonAsync<ScheduleDetailViewModel>();

                // Fetch other active tour schedules
                var allResponse = await client.GetAsync("odata/TourSchedules?$expand=Tour&$filter=Status eq 'Active'");
                if (allResponse.IsSuccessStatusCode)
                {
                    var result = await allResponse.Content.ReadFromJsonAsync<ODataResponse<ScheduleDetailViewModel>>();
                    if (result != null && result.Value != null)
                    {
                        // Exclude the current schedule and take up to 3 tours
                        RelatedSchedules = result.Value
                            .Where(x => x.ScheduleId != scheduleId && x.Tour != null)
                            .Take(3)
                            .ToList();
                    }
                }

                return Page();
            }
            return NotFound();
        }
    }

    public class ODataResponse<T>
    {
        public List<T> Value { get; set; } = new List<T>();
    }
}
