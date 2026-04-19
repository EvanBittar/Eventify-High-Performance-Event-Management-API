using Eventify_High_Performance_Event_Management_API.Dtos;
using Eventify_High_Performance_Event_Management_API.Models;
using Microsoft.AspNetCore.Mvc;
using Eventify_High_Performance_Event_Management_API.Repository.Interfaces;
using Eventify_High_Performance_Event_Management_API.Services.Interfaces;

namespace Eventify_High_Performance_Event_Management_API.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthService _authService;
        public UserController(IUserRepository userRepository, IAuthService authService)
        {
            _userRepository = userRepository;
            _authService = authService;
        }
        // [HttpGet("test")]
        // public async Task<IActionResult> Test()
        // {
        //     return Ok(await _dapper.LoadData<DateTime>("SELECT GETDATE()"));
        // }

        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return Ok(users);
        }
        [HttpGet("GetUserById/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null) return NotFound("User not found");
            return Ok(user);
        }
        [HttpPost("GetUserByEmailAsync")]
        public async Task<IActionResult> GetUserByEmailAsync(string email)
        {
            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user == null) return NotFound("User not found");
            return Ok(user);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserToAddDto userToAddDto)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(userToAddDto.Email);
            if (existingUser != null)
            {
                return BadRequest("Email already exists");
            }
            string passwordHash = _authService.HashPassword(userToAddDto.PasswordHash);

            var parameters = new User
            {
                FirstName = userToAddDto.FirstName,
                LastName = userToAddDto.LastName,
                Email = userToAddDto.Email,
                PasswordHash = passwordHash,
                IsAdmin = false,
                IsVerified = false
            };
            bool result = await _userRepository.AddUserAsync(parameters);

            if (result) return Ok("User registered successfully");
            return StatusCode(500, "Something went wrong while saving the user.");
            // return await _userRepository.GetUserByEmailAsync(userToAddDto.Email) != null ? Ok("User registered successfully") : BadRequest("Failed to register user");
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserToLoginDto userLogin)
        {
            var user = await _userRepository.GetUserByEmailAsync(userLogin.Email);

            if (user == null || !_authService.VerifyPassword(userLogin.Password, user.PasswordHash))
            {
                return BadRequest("Invalid email or password");
            }

            return Ok(new { token = _authService.CreateToken(user) });
        }
    }
}