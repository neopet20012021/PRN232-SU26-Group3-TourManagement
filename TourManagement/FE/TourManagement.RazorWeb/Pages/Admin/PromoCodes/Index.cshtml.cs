using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TourManagement.RazorWeb.Pages.Admin.PromoCodes
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public IndexModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public List<PromoCodeViewModel> PromoCodes { get; set; } = new List<PromoCodeViewModel>();

        [BindProperty(SupportsGet = true)]
        public string? SearchQuery { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var client = _clientFactory.CreateClient("API");
            var response = await client.GetAsync("odata/PromoCodes");

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ODataResponse<PromoCodeViewModel>>();
                if (result?.Value != null)
                {
                    PromoCodes = result.Value;
                }

                if (!string.IsNullOrWhiteSpace(SearchQuery))
                {
                    var lowerQuery = SearchQuery.ToLower();
                    PromoCodes = PromoCodes.Where(p => p.Code.ToLower().Contains(lowerQuery)).ToList();
                }
            }
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var client = _clientFactory.CreateClient("API");
            var response = await client.DeleteAsync($"odata/PromoCodes({id})");

            return RedirectToPage();
        }

        public class ODataResponse<T>
        {
            public List<T> Value { get; set; } = new List<T>();
        }

        public class PromoCodeViewModel
        {
            public int PromoCodeId { get; set; }
            public string Code { get; set; } = string.Empty;
            public decimal DiscountPercent { get; set; }
            public decimal MinBookingValue { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public int MaxUsage { get; set; }
            public int UsageCount { get; set; }
            public bool IsActive { get; set; }
        }
    }
}
