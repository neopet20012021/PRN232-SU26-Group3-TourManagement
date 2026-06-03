using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TourManagement.Business;
namespace TourManagement.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public AccountController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var client = _clientFactory.CreateClient();
            var response = await client.PostAsJsonAsync("https://localhost:7055", model);

            if (response.IsSuccessStatusCode)
            {
                // Đọc dữ liệu trả về (bao gồm cả Token và Role)
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(result.Token) as JwtSecurityToken;
                var claimsIdentity = new ClaimsIdentity(jsonToken.Claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                // XỬ LÝ ĐIỀU HƯỚNG DỰA VÀO ROLE
                if (result.Role == "Admin")
                {
                    return RedirectToAction("Dashboard", "Admin"); // Về trang quản trị Admin
                }
                else
                {
                    return RedirectToAction("Index", "Home"); // User bình thường về trang chủ
                }
            }

            ModelState.AddModelError("", "Đăng nhập thất bại.");
            return View(model);
        }

        // Class hứng dữ liệu trả về từ API
        public class LoginResponse
        {
            public string Token { get; set; }
            public string Role { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            // Xóa sạch Cookie đăng nhập
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }
    }

    public class TokenResponse { public string Token { get; set; } }
}
