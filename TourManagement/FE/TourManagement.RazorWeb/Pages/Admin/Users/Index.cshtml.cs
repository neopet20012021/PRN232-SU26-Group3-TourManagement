using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TourManagement.RazorWeb.Pages.Admin.Users
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public IndexModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public List<UserViewModel> UsersList { get; set; } = new List<UserViewModel>();

        public async Task<IActionResult> OnGetAsync()
        {
            var client = _clientFactory.CreateClient("API");
            var response = await client.GetAsync("api/Users"); // It's an OData controller but route is api/Users
            
            if (response.IsSuccessStatusCode)
            {
                // Note: If OData is correctly mapped, it might return { "@odata.context": "...", "value": [...] }
                // or just [...] depending on exact setup.
                try
                {
                    var result = await response.Content.ReadFromJsonAsync<List<UserViewModel>>();
                    if (result != null)
                    {
                        UsersList = result;
                    }
                }
                catch
                {
                    var resultOdata = await response.Content.ReadFromJsonAsync<ODataResponse<UserViewModel>>();
                    if (resultOdata?.Value != null)
                    {
                        UsersList = resultOdata.Value;
                    }
                }
            }
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var client = _clientFactory.CreateClient("API");
            var response = await client.DeleteAsync($"api/Users/{id}");

            return RedirectToPage();
        }

        public class ODataResponse<T>
        {
            public List<T> Value { get; set; } = new List<T>();
        }

        public class UserViewModel
        {
            public int UserId { get; set; }
            public string Username { get; set; } = string.Empty;
            public string Role { get; set; } = string.Empty;
            public string FullName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
        }
    }
}
