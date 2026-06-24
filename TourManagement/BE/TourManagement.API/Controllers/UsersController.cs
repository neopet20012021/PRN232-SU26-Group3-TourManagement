using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using TourManagement.Business.DTOs;
using TourManagement.Business.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace TourManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ODataController
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [EnableQuery]
        public async Task<IActionResult> Get()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        [EnableQuery]
        public async Task<IActionResult> Get(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateUserDTO userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.CreateUserAsync(userDto);
            if (!result)
            {
                return Conflict("Username already exists.");
            }

            return Ok(new { message = "User created successfully." });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateUserDTO userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.UpdateUserAsync(id, userDto);
            if (!result)
            {
                return NotFound();
            }

            return Ok(new { message = "User updated successfully." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result)
            {
                return NotFound();
            }

            return Ok(new { message = "User deleted successfully." });
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userService.AuthenticateAsync(loginDto.Username, loginDto.Password);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            return Ok(user);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] CreateUserDTO userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Force role to Customer
            userDto.Role = "Customer";

            var result = await _userService.CreateUserAsync(userDto);
            if (!result)
            {
                return Conflict(new { message = "Username already exists." });
            }

            return Ok(new { message = "Registration successful." });
        }
    }
}
