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

        [BindProperty(SupportsGet = true)]
        public string? SearchQuery { get; set; }

        public int TotalUsers { get; set; }
        public int AdminCount { get; set; }
        public int StaffCount { get; set; }

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

                var allUsers = UsersList;
                TotalUsers = allUsers.Count;
                AdminCount = allUsers.Count(u => u.Role == "Admin");
                StaffCount = allUsers.Count(u => u.Role == "Staff");

                if (!string.IsNullOrWhiteSpace(SearchQuery))
                {
                    var lowerQuery = SearchQuery.ToLower();
                    UsersList = allUsers.Where(u => 
                        (u.FullName != null && u.FullName.ToLower().Contains(lowerQuery)) ||
                        (u.Email != null && u.Email.ToLower().Contains(lowerQuery)) ||
                        (u.Username != null && u.Username.ToLower().Contains(lowerQuery))
                    ).ToList();
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
