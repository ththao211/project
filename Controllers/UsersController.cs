using Microsoft.AspNetCore.Mvc;
using SWP_BE.DTOs;
using SWP_BE.Services;

namespace SWP_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize(Roles = "Admin")] // Tạm thời  để test trên Swagger trước khi có Đăng nhập
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDTO dto)
        {
            var result = await _userService.CreateUserAsync(dto);
            if (result == null) return BadRequest("Username hoặc Email đã tồn tại!");
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateDTO dto)
        {
            var success = await _userService.UpdateUserAsync(id, dto);
            if (!success) return NotFound("Không tìm thấy User!");
            return Ok("Cập nhật thành công!");
        }
    }
}