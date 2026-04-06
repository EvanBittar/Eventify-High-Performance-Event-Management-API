using Eventify_High_Performance_Event_Management_API.Data;
using Microsoft.AspNetCore.Mvc;

namespace Eventify_High_Performance_Event_Management_API.Dtos
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController(IConfiguration configuration) : ControllerBase
    {
        private readonly DataContext _dapper = new(configuration);
        [HttpPost("BookEvent")]
        public async Task<IActionResult> BookEvent(BookingDto bookingDto)
        {
            string sql = @"SELECT (SELECT MaxAttendees FROM Event.Events WHERE EventId = @EventId) AS MaxCapacity,
                (SELECT COUNT(*) FROM Event.Bookings WHERE EventId = @EventId AND Status = 1) AS CurrentBookings";
            var result = await _dapper.LoadDataSingle<dynamic>(sql, new { EventId = bookingDto.EventId });

            if (result == null) return NotFound("Event not found.");

            if (result.CurrentBookings >= result.MaxCapacity) return BadRequest("Event is fully booked.");

            string insertSql = @"INSERT INTO Event.Bookings 
                    (EventId, UserId, BookingDate, Status)
                    VALUES (@EventId, @UserId, GETDATE(), 1)";
            try
            {
                bool insertResult = await _dapper.ExecuteSql(insertSql, bookingDto);
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