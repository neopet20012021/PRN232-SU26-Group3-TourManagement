using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text;

namespace TourManagement.RazorWeb.Pages.Users
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public IndexModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public List<UserViewModel> Users { get; set; } = new List<UserViewModel>();

        [BindProperty(SupportsGet = true)]
        public string? SearchName { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? RoleFilter { get; set; }

        [BindProperty]
        public UserInputModel UserInput { get; set; } = new UserInputModel();

        public async Task OnGetAsync()
        {
            var client = _clientFactory.CreateClient("API");
            
            // Build OData query
            var query = "?$orderby=UserId desc";
            var filters = new List<string>();
            
            if (!string.IsNullOrEmpty(SearchName))
            {
                filters.Add($"contains(tolower(Username), '{SearchName.ToLower()}')");
            }
            if (!string.IsNullOrEmpty(RoleFilter))
            {
                filters.Add($"Role eq '{RoleFilter}'");
            }

            if (filters.Count > 0)
            {
                query += "&$filter=" + string.Join(" and ", filters);
            }

            var response = await client.GetAsync("api/Users" + query);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<List<UserViewModel>>();
                if (result != null)
                {
                    Users = result;
                }
            }
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            var client = _clientFactory.CreateClient("API");
            var content = new StringContent(JsonSerializer.Serialize(UserInput), Encoding.UTF8, "application/json");
            
            var response = await client.PostAsync("api/Users", content);
            
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            var client = _clientFactory.CreateClient("API");
            
            // In Edit, password might be empty if not changing
            var updateDto = new 
            {
                Role = UserInput.Role,
                FullName = UserInput.FullName,
                Email = UserInput.Email
            };

            var content = new StringContent(JsonSerializer.Serialize(updateDto), Encoding.UTF8, "application/json");
            var response = await client.PutAsync($"api/Users/{UserInput.UserId}", content);
            
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var client = _clientFactory.CreateClient("API");
            await client.DeleteAsync($"api/Users/{id}");
            return RedirectToPage();
        }
    }

    public class UserViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class UserInputModel
    {
        [BindProperty(Name = "User.UserId")]
        public int UserId { get; set; }
        
        [BindProperty(Name = "User.Username")]
        public string Username { get; set; } = string.Empty;
        
        [BindProperty(Name = "User.Password")]
        public string? Password { get; set; }
        
        [BindProperty(Name = "User.Role")]
        public string Role { get; set; } = string.Empty;
        
        [BindProperty(Name = "User.FullName")]
        public string FullName { get; set; } = string.Empty;
        
        [BindProperty(Name = "User.Email")]
        public string Email { get; set; } = string.Empty;
    }
}
