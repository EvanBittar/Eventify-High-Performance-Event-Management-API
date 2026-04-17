using System.Security.Claims;
using Eventify_High_Performance_Event_Management_API.Dtos;
using Eventify_High_Performance_Event_Management_API.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eventify_High_Performance_Event_Management_API.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BookingController(IBookingRepository bookingRepository) : ControllerBase
    {
        private readonly IBookingRepository _bookingRepository = bookingRepository;

        [HttpDelete("CancelBooking/{bookingId}")]
        public async Task<IActionResult> CancelBooking(int bookingId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized("User not authenticated.");

            var success = await _bookingRepository.CancelBookingAsync(bookingId);

            if (success)
                return Ok(new { Message = "Booking cancelled successfully." });

            return BadRequest(new { Message = "Could not cancel booking or booking not found." });
        }
        [HttpGet("GetMyTickets")]
        public async Task<IActionResult> GetMyTickets()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized("User not authenticated.");

            var bookings = await _bookingRepository.GetUserBookingsAsync(int.Parse(userIdClaim));
            return Ok(bookings);
        }
        [HttpPost("CreateBooking")]
        public async Task<IActionResult> CreateBooking(int eventId)
        {
            var UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (UserId == null) return Unauthorized("User not authenticated.");

            var booking = new BookingDto
            {
                EventId = eventId,
                UserId = int.Parse(UserId)
            };

            var result = await _bookingRepository.CreateBookingAsync(booking);

            if (result == "Success")
                return Ok(new { Message = "Booking created successfully." });

            return BadRequest(new { Message = result });
        }
    }
}