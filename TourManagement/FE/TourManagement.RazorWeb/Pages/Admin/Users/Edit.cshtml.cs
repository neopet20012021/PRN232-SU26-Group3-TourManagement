using System.ComponentModel.DataAnnotations;
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
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public EditModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [BindProperty]
        public EditUserViewModel UserInput { get; set; } = new();

        [BindProperty]
        public int UserId { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var client = _clientFactory.CreateClient("API");
            var response = await client.GetAsync($"api/Users/{id}");

            if (response.IsSuccessStatusCode)
            {
                var user = await response.Content.ReadFromJsonAsync<UserViewModel>();
                if (user != null)
                {
                    UserId = user.UserId;
                    UserInput = new EditUserViewModel
                    {
                        FullName = user.FullName,
                        Email = user.Email,
                        Role = user.Role
                    };
                    return Page();
                }
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var client = _clientFactory.CreateClient("API");
            var response = await client.PutAsJsonAsync($"api/Users/{UserId}", UserInput);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("./Index");
            }

            ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi cập nhật thông tin người dùng.");
            return Page();
        }

        public class EditUserViewModel
        {
            [Required(ErrorMessage = "Vai trò là bắt buộc")]
            public string Role { get; set; } = string.Empty;

            [Required(ErrorMessage = "Họ và tên là bắt buộc")]
            public string FullName { get; set; } = string.Empty;

            [Required(ErrorMessage = "Email là bắt buộc")]
            [EmailAddress(ErrorMessage = "Email không hợp lệ")]
            public string Email { get; set; } = string.Empty;
        }
    }
}
