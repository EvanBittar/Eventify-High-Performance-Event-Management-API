using Eventify_High_Performance_Event_Management_API.Data;
using Eventify_High_Performance_Event_Management_API.Dtos;
using Eventify_High_Performance_Event_Management_API.Models;
using Microsoft.AspNetCore.Mvc;

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
    }
}