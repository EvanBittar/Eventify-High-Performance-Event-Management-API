using Eventify_High_Performance_Event_Management_API.Dtos;
using Eventify_High_Performance_Event_Management_API.Models;
using Eventify_High_Performance_Event_Management_API.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eventify_High_Performance_Event_Management_API.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : ControllerBase
    {
        private readonly IEventRepository _eventRepository;
        public EventController(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }
        [HttpGet("GetAllEvents")]
        public async Task<IActionResult> GetAllEvents()
        {
            var events = await _eventRepository.GetAllEventsAsync();
            return Ok(events);
        }
        [HttpGet("GetAllEventsById/{id}")]
        public async Task<Event?> GetAllEventsById(int id)
        {
            return await _eventRepository.GetEventByIdAsync(id);
        }
        [HttpGet("SearchEvents")]
        public async Task<IActionResult> SearchEvents(string? title = null, int? CategoryId = null)
        {
            var events = await _eventRepository.SearchEvents(title, CategoryId);
            return Ok(events);
        }
        [HttpPost("AddEvent")]
        public async Task<IActionResult> AddEvent(EventToAddDto eventToAddDto)
        {
            if (await _eventRepository.CreateEventAsync(eventToAddDto)) return Ok("Event added successfully.");
            return BadRequest("Failed to add event.");
        }
        [HttpPut("UpdateEvent/{id}")]
        public async Task<IActionResult> UpdateEvent(int id, EventToAddDto eventToAddDto)
        {
            if (await _eventRepository.UpdateEventAsync(id, eventToAddDto)) return Ok("Event Updated successfully.");
            return BadRequest("Failed to Updated event.");
        }
        [Authorize]
        [HttpGet("DashboardStats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var stats = await _eventRepository.GetDashboardStatsAsync();
            return Ok(stats);
        }
    }
}