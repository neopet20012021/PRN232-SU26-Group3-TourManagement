using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TourManagement.Data.Models;
using TourManagement.Data.Repositories;

namespace TourManagement.Business.Services
{
    public interface IAccountService
    {
        Task<User?> AuthenticateAsync(string username, string password);
        Task<(bool Success, string Message)> RegisterAsync(string username, string password, string fullName, string email, string? phoneNumber);
        Task<User?> GetUserByIdAsync(int userId);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }

    public class AccountService : IAccountService
    {
        private readonly IUserRepository _userRepository;
        private const string Salt = "TourMgmt_Salt_2026";

        public AccountService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null) return null;

            if (!VerifyPassword(password, user.PasswordHash)) return null;

            return user;
        }

        public async Task<(bool Success, string Message)> RegisterAsync(string username, string password, string fullName, string email, string? phoneNumber)
        {
            if (await _userRepository.UsernameExistsAsync(username))
                return (false, "Tên đăng nhập đã tồn tại.");

            if (await _userRepository.EmailExistsAsync(email))
                return (false, "Email đã được sử dụng.");

            var user = new User
            {
                Username = username,
                PasswordHash = HashPassword(password),
                FullName = fullName,
                Email = email,
                PhoneNumber = phoneNumber,
                Role = "Customer",
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return (true, "Đăng ký thành công!");
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _userRepository.GetByIdAsync(userId);
        }

        public string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password + Salt);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password) == hash;
        }
    }
}
