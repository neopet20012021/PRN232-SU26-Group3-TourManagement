using AutoMapper;
using TourManagement.Business.DTOs;
using TourManagement.Data.Models;
using TourManagement.Data.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TourManagement.Business.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        Task<UserDTO?> GetUserByIdAsync(int id);
        Task<UserDTO?> AuthenticateAsync(string username, string password);
        Task<bool> CreateUserAsync(CreateUserDTO userDto);
        Task<bool> UpdateUserAsync(int id, UpdateUserDTO userDto);
        Task<bool> DeleteUserAsync(int id);
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserDTO>>(users);
        }

        public async Task<UserDTO?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user == null ? null : _mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO?> AuthenticateAsync(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null || user.PasswordHash != password) // In real app, hash password and verify
            {
                return null;
            }
            return _mapper.Map<UserDTO>(user);
        }

        public async Task<bool> CreateUserAsync(CreateUserDTO userDto)
        {
            var existingUser = await _userRepository.GetByUsernameAsync(userDto.Username);
            if (existingUser != null)
                return false;

            var user = new User
            {
                Username = userDto.Username,
                PasswordHash = userDto.Password, // No hashing for simplicity as per current DB seed
                Role = userDto.Role,
                FullName = userDto.FullName,
                Email = userDto.Email
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateUserAsync(int id, UpdateUserDTO userDto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return false;

            user.Role = userDto.Role;
            user.FullName = userDto.FullName;
            user.Email = userDto.Email;

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return false;

            _userRepository.Remove(user);
            await _userRepository.SaveChangesAsync();
            return true;
        }
    }
}
