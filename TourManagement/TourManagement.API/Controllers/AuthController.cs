using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TourManagement.Business;
using TourManagement.Data;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly TourManagementDbContext _context; // Inject DbContext của bạn vào đây

    public AuthController(TourManagementDbContext context)
    {
        _context = context;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginModel model)
    {
        // Tìm user trong DB
        var user = _context.Users.FirstOrDefault(u => u.Username == model.Username && u.Password == model.Password);

        if (user != null)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role) // Lấy Role thực tế: "Admin" hoặc "User"
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Key_Bi_Mat_Sieu_Cap_Vua_Dai_Vua_An_Toan_12345"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = creds,
                Issuer = "TourManagementAPI",
                Audience = "TourManagementWeb"
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Trả về cả Token và Role để phía Web xử lý điều hướng
            return Ok(new { Token = tokenHandler.WriteToken(token), Role = user.Role });
        }

        return Unauthorized("Tài khoản hoặc mật khẩu không chính xác.");
    }
}