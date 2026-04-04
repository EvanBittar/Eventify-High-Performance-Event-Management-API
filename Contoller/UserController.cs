using Eventify_High_Performance_Event_Management_API.Data;
using Eventify_High_Performance_Event_Management_API.Dtos;
using Eventify_High_Performance_Event_Management_API.Models;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;
using Microsoft.AspNetCore.Identity;

namespace Eventify_High_Performance_Event_Management_API.Contoller
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly DataContext _dapper;
        public UserController(IConfiguration configuration)
        {
            _dapper = new DataContext(configuration);
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
            return Ok(new {
            Message = "Login Successful!",
            User = new { user.FirstName, user.LastName, user.IsAdmin }
        });
        }
    }
}