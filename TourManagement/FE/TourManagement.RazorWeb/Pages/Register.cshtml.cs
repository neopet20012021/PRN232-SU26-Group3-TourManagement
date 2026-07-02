using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TourManagement.RazorWeb.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public RegisterModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [BindProperty]
        public RegisterInputModel Input { get; set; } = new RegisterInputModel();

        public string ErrorMessage { get; set; } = string.Empty;
        public string SuccessMessage { get; set; } = string.Empty;

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var client = _clientFactory.CreateClient("API");
            
            // Note: The API automatically sets Role to "Customer" in the /register endpoint
            var createDto = new
            {
                Username = Input.Username,
                Password = Input.Password,
                Role = "Customer", // Doesn't matter, backend forces it
                FullName = Input.FullName,
                Email = Input.Email
            };

            var content = new StringContent(JsonSerializer.Serialize(createDto), Encoding.UTF8, "application/json");

            var response = await client.PostAsync("api/Users/register", content);

            if (response.IsSuccessStatusCode)
            {
                SuccessMessage = "Registration successful! You can now log in.";
                return Page();
            }
            else
            {
                ErrorMessage = "Registration failed. Username may already exist.";
                return Page();
            }
        }

        public class RegisterInputModel
        {
            [Required(ErrorMessage = "Username is required")]
            [StringLength(50, MinimumLength = 3)]
            public string Username { get; set; } = string.Empty;

            [Required(ErrorMessage = "Password is required")]
            [StringLength(50, MinimumLength = 6)]
            public string Password { get; set; } = string.Empty;

            [Required(ErrorMessage = "Full Name is required")]
            [StringLength(100)]
            public string FullName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Email is required")]
            [EmailAddress]
            public string Email { get; set; } = string.Empty;
        }
    }
}
