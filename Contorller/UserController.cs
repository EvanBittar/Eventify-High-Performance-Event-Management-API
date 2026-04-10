using Eventify_High_Performance_Event_Management_API.Data;
using Eventify_High_Performance_Event_Management_API.Dtos;
using Eventify_High_Performance_Event_Management_API.Models;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Eventify_High_Performance_Event_Management_API.Contorller
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly DataContext _dapper;
        private readonly IConfiguration _config;
        public UserController(IConfiguration configuration)
        {
            _config = configuration;
            _dapper = new DataContext(_config);
        }
        [HttpGet("test")]
        public async Task<IActionResult> Test()
        {
            return Ok(await _dapper.LoadData<DateTime>("SELECT GETDATE()"));
        }
        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers()
        {
            var sql = "SELECT * FROM Event.Users";
            var users = await _dapper.LoadData<User>(sql);
            return Ok(users);
        }
        [HttpPost("AddUsers")]
        public async Task<IActionResult> AddUsers(UserToAddDto user)
        {
            var sql = @"INSERT INTO Event.Users(
                    FirstName ,
                    LastName ,
                    Email ,
                    PasswordHash ,
                    IsAdmin) VALUES(
                    @FirstName, @LastName, @Email, @PasswordHash, @IsAdmin)";
            bool result = await _dapper.ExecuteSql(sql, user);
            if (result)
            {
                return Ok("User added successfully");
            }
            else
            {
                return BadRequest("Failed to add user");
            }
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserToAddDto userToAddDto)
        {
            string checkEmailSql = "SELECT Email FROM Event.Users WHERE Email = @Email";
            var existingEmail = await _dapper.LoadDataSingle<string>(checkEmailSql , new { Email = userToAddDto.Email });
            if (existingEmail != null)
            {
                return BadRequest("Email already exists");
            }
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(userToAddDto.PasswordHash);

            string sql = @"INSERT INTO Event.Users(FirstName ,LastName ,Email ,PasswordHash ,IsAdmin) VALUES(
                    @FirstName, @LastName, @Email, @PasswordHash, @IsAdmin)";
            var parameters = new
            {
                FirstName = userToAddDto.FirstName,
                LastName = userToAddDto.LastName,
                Email = userToAddDto.Email,
                PasswordHash = passwordHash,
                IsAdmin = false
            };
            bool result = await _dapper.ExecuteSql(sql, parameters);

            if (result) return Ok("User registered successfully");
            return BadRequest("Failed to register user");
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserToLoginDto userLogin)
        {
            string sql = "SELECT * FROM Event.Users WHERE Email = @Email";
            var user = await _dapper.LoadDataSingle<User>(sql, new { Email = userLogin.Email});
            if (user == null)
            {
                return BadRequest("Invalid email or password");
            }
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(userLogin.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                return BadRequest("Invalid email or password");
            }
            var claim = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("IsAdmin", user.IsAdmin.ToString())
            };
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:PasswordKey").Value!));
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