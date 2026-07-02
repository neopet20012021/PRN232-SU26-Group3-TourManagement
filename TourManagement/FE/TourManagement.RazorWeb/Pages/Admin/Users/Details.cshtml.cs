using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static TourManagement.RazorWeb.Pages.Admin.Users.IndexModel;

namespace TourManagement.RazorWeb.Pages.Admin.Users
{
    [Authorize(Roles = "Admin")]
    public class DetailsModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public DetailsModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public UserViewModel? UserDetail { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var client = _clientFactory.CreateClient("API");
            var response = await client.GetAsync($"api/Users/{id}");

            if (response.IsSuccessStatusCode)
            {
                UserDetail = await response.Content.ReadFromJsonAsync<UserViewModel>();
                if (UserDetail != null)
                {
                    return Page();
                }
            }

            return NotFound();
        }
    }
}
