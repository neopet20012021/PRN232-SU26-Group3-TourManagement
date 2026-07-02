using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TourManagement.RazorWeb.Pages.Admin.Users
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public CreateModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [BindProperty]
        public CreateUserViewModel UserInput { get; set; } = new();

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
            var response = await client.PostAsJsonAsync("api/Users", UserInput);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToPage("./Index");
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                ModelState.AddModelError(string.Empty, "Tên đăng nhập đã tồn tại. Vui lòng chọn tên khác.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi khi tạo người dùng mới.");
            }

            return Page();
        }

        public class CreateUserViewModel
        {
            [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
            [MinLength(3, ErrorMessage = "Tên đăng nhập phải có ít nhất 3 ký tự")]
            public string Username { get; set; } = string.Empty;

            [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
            [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
            public string Password { get; set; } = string.Empty;

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
