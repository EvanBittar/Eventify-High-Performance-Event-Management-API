using Eventify_High_Performance_Event_Management_API.Data;
using Eventify_High_Performance_Event_Management_API.Dtos;
using Eventify_High_Performance_Event_Management_API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Eventify_High_Performance_Event_Management_API.Contorller
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : ControllerBase
    {
        private readonly DataContext _dapper;
        public EventController(IConfiguration configuration)
        {
            _dapper = new DataContext(configuration);
        }
        [HttpGet("SearchEvents")]
        public async Task<IEnumerable<dynamic>> SearchEvents(string? title = null , int? CategoryId = null)
        {
            string sql = @"SELECT e.*,c.NameCategory 
                FROM Event.Events AS e 
                JOIN Event.Categories AS c ON e.CategoryId = c.CategoryId
                WHERE (@Title IS NULL OR e.Title LIKE '%' + @Title + '%')
                AND (@CategoryId IS NULL OR e.CategoryId = @CategoryId)";

            return await _dapper.LoadData<dynamic>(sql, new { Title = title  , CategoryId = CategoryId });
        }
        [HttpPost("AddEvent")]
        public async Task<IActionResult> AddEvent(EventToAddDto eventToAddDto)
        {
            string sql = @"INSERT INTO Event.Events (Title, Description, Location, StartDate, EndDate, MaxAttendees, CategoryId, CreatedBy)
                VALUES (@Title, @Description, @Location, @StartDate, @EndDate, @MaxAttendees, @CategoryId, @CreatedBy)";

            bool result = await _dapper.ExecuteSql(sql, eventToAddDto);
            if(result) return Ok("Event added successfully.");
            return BadRequest("Failed to add event.");
        
        }
    }
}