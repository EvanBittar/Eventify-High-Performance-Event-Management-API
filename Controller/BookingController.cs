using System.Security.Claims;
using Eventify_High_Performance_Event_Management_API.Data;
using Eventify_High_Performance_Event_Management_API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eventify_High_Performance_Event_Management_API.Controller
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
        [HttpDelete("CancelBooking")]
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (UserId == null) return Unauthorized("User not authenticated.");

            if (bookingId <= 0) return BadRequest("Invalid booking ID.");

            int userId = int.Parse(UserId);

            string updateSql = @"UPDATE
             Event.Bookings 
             SET Status = 2 
             WHERE BookingId = @BookingId 
             AND UserId = @UserId
             AND Status = 1";

            bool result = await _dapper.ExecuteSql(updateSql, new { BookingId = bookingId, UserId = userId });

            if (result)
            {
                return Ok("Booking cancelled successfully.");

            }
            return BadRequest("Could not cancel booking. It may not exist or is already cancelled.");
        }
        [HttpGet("GetMyTickets")]
        public async Task<IActionResult> GetMyTickets()
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (UserId == null) return Unauthorized("User not authenticated.");

            int userId = int.Parse(UserId);

            string sql = @"SELECT 
                    b.BookingId , 
                    e.Title , 
                    e.StartDate , 
                    e.Location ,
                    c.NameCategory ,
                    b.Status
                    FROM Event.Bookings AS b
                    JOIN Event.Events AS e ON b.EventId = e.EventId
                    JOIN Event.Categories AS c ON e.CategoryId = c.CategoryId
                    WHERE b.UserId = @UserId AND b.Status <> 2";
            
            var myTickets = await _dapper.LoadData<BookingViewDto>(sql, new {UserId = userId});
            return Ok(myTickets);
        }
    }
}