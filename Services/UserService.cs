using SWP_BE.DTOs;
using SWP_BE.Models;
using SWP_BE.Repositories;
using BCrypt.Net;

namespace SWP_BE.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync();
        Task<UserResponseDTO?> CreateUserAsync(UserCreateDTO dto);
        Task<bool> UpdateUserAsync(int id, UserUpdateDTO dto);
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        public UserService(IUserRepository userRepo) { _userRepo = userRepo; }

        public async Task<IEnumerable<UserResponseDTO>> GetAllUsersAsync()
        {
            var users = await _userRepo.GetAllUsersAsync();
            return users.Select(u => new UserResponseDTO
            {
                Id = u.Id,
                UserName = u.UserName,
                FullName = u.FullName,
                Role = u.Role,
                Email = u.Email,
                Expertise = u.Expertise,
                Score = u.Score,
                IsActive = u.IsActive
            });
        }

        public async Task<UserResponseDTO?> CreateUserAsync(UserCreateDTO dto)
        {
            // 1. Kiểm tra trùng lặp
            var existingUser = await _userRepo.GetUserByEmailOrUsernameAsync(dto.Email, dto.UserName);
            if (existingUser != null) return null; // Báo lỗi trùng

            // 2. Map DTO sang Model & Mã hóa mật khẩu
            var newUser = new User
            {
                UserName = dto.UserName,
                Email = dto.Email,
                FullName = dto.FullName,
                Role = dto.Role,
                Expertise = dto.Expertise,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password), // Băm mật khẩu!
                Score = 100, // Mặc định 100 điểm như Docs yêu cầu
                IsActive = true
            };

            await _userRepo.AddUserAsync(newUser);

            return new UserResponseDTO { UserName = newUser.UserName, Email = newUser.Email, Role = newUser.Role, Score = newUser.Score };
        }

        public async Task<bool> UpdateUserAsync(int id, UserUpdateDTO dto)
        {
            var user = await _userRepo.GetUserByIdAsync(id);
            if (user == null) return false;

            user.FullName = dto.FullName;
            user.Role = dto.Role;
            user.Expertise = dto.Expertise;
            user.IsActive = dto.IsActive;

            await _userRepo.UpdateUserAsync(user);
            return true;
        }
    }
}