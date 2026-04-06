using System.Security.Claims;
using Eventify_High_Performance_Event_Management_API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eventify_High_Performance_Event_Management_API.Contoller
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BookingController(IConfiguration configuration) : ControllerBase
    {
        private readonly DataContext _dapper = new(configuration);
        [HttpPost("BookEvent")]
        public async Task<IActionResult> BookEvent(int eventId)
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (UserId == null) return Unauthorized("User not authenticated.");

            int userId = int.Parse(UserId);    
            string sql = @"SELECT (SELECT MaxAttendees FROM Event.Events WHERE EventId = @EventId) AS MaxCapacity,
                (SELECT COUNT(*) FROM Event.Bookings WHERE EventId = @EventId AND Status = 1) AS CurrentBookings";
            var result = await _dapper.LoadDataSingle<dynamic>(sql, new { EventId = eventId });

            if (result == null) return NotFound("Event not found.");

            if (result.CurrentBookings >= result.MaxCapacity) return BadRequest("Event is fully booked.");

            string insertSql = @"INSERT INTO Event.Bookings 
                    (EventId, UserId, BookingDate, Status)
                    VALUES (@EventId, @UserId, GETDATE(), 1)";

            if (eventId <= 0) return BadRequest("Invalid event ID.");
            try
            {
                bool insertResult = await _dapper.ExecuteSql(insertSql, new { EventId = eventId, UserId = userId });
                if (insertResult) return Ok("Event booked successfully.");
            }
            catch (Exception)
            {
                return BadRequest("You have already booked this event.");
            }
           
            return BadRequest("Failed to complete booking.");
        }
    }
}