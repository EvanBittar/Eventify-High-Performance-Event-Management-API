using Eventify_High_Performance_Event_Management_API.Data;
using Eventify_High_Performance_Event_Management_API.Dtos;
using Eventify_High_Performance_Event_Management_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Eventify_High_Performance_Event_Management_API.Repository.Interfaces;
using Eventify_High_Performance_Event_Management_API.Repository;

namespace Eventify_High_Performance_Event_Management_API.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IUserRepository _userRepository;
        public UserController(IUserRepository userRepository, IConfiguration configuration)
        {
            _config = configuration;
            _userRepository = userRepository;
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
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(userToAddDto.PasswordHash);

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
            return BadRequest("Failed to register user");
            // return await _userRepository.GetUserByEmailAsync(userToAddDto.Email) != null ? Ok("User registered successfully") : BadRequest("Failed to register user");
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserToLoginDto userLogin)
        {
            var user = await _userRepository.GetUserByEmailAsync(userLogin.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(userLogin.Password, user.PasswordHash))
            {
                return BadRequest("Invalid email or password");
            }

            var claim = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("IsVerified", user.IsVerified.ToString()),
                new Claim("IsAdmin", user.IsAdmin.ToString())
            };

            var keyString = _config.GetSection("AppSettings:PasswordKey").Value;
            if (string.IsNullOrEmpty(keyString)) return StatusCode(500, "Server configuration error");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claim),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new
            {
                token = tokenHandler.WriteToken(token)
            });
        }
    }
}