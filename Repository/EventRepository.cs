using Eventify_High_Performance_Event_Management_API.Data;
using Eventify_High_Performance_Event_Management_API.Models;
using Eventify_High_Performance_Event_Management_API.Dtos;
using Eventify_High_Performance_Event_Management_API.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Eventify_High_Performance_Event_Management_API.Repository
{
    public class EventRepository : IEventRepository
    {
        private readonly DataContext _dapper;

        public EventRepository(DataContext dapper)
        {
            _dapper = dapper;
        }

        public async Task<IEnumerable<Event>> GetAllEventsAsync()
        {
            string sql = "SELECT * FROM Event.Events";
            return await _dapper.LoadData<Event>(sql, new { });
        }

        public async Task<Event?> GetEventByIdAsync(int id)
        {
            string sql = "SELECT * FROM Event.Events WHERE EventId = @EventId";
            return await _dapper.LoadDataSingle<Event>(sql, new { EventId = id });
        }

        public async Task<bool> CreateEventAsync(EventToAddDto eventToAddDto)
        {
            string sql = @"INSERT INTO Event.Events (Title, Description, Location, StartDate, EndDate, MaxAttendees, CategoryId, CreatedBy)
                VALUES (@Title, @Description, @Location, @StartDate, @EndDate, @MaxAttendees, @CategoryId, @CreatedBy)";

            return await _dapper.ExecuteSql(sql, eventToAddDto);
        }
        public async Task<IEnumerable<dynamic>> SearchEvents(string? title = null, int? CategoryId = null)
        {
            string sql = @"SELECT e.*,c.NameCategory 
                FROM Event.Events AS e 
                JOIN Event.Categories AS c ON e.CategoryId = c.CategoryId
                WHERE (@Title IS NULL OR e.Title LIKE '%' + @Title + '%')
                AND (@CategoryId IS NULL OR e.CategoryId = @CategoryId)";

            return await _dapper.LoadData<dynamic>(sql, new { Title = title, CategoryId = CategoryId });
        }
        public async Task<bool> UpdateEventAsync(int id, EventToAddDto eventToAddDto)
        {
            string checkSql = "SELECT COUNT(*) FROM Event.Events WHERE EventId = @EventId";
            int count = await _dapper.LoadDataSingle<int>(checkSql, new { EventId = id });

            if (count == 0)
            {
                return false;
            }

            string sql = @"UPDATE Event.Events 
        SET Title = @Title, 
            Description = @Description, 
            Location = @Location, 
            StartDate = @StartDate, 
            EndDate = @EndDate, 
            MaxAttendees = @MaxAttendees, 
            CategoryId = @CategoryId, 
            CreatedBy = @CreatedBy 
        WHERE EventId = @EventId";

            return await _dapper.ExecuteSql(sql, new
            {
                EventId = id,
                eventToAddDto.Title,
                eventToAddDto.Description,
                eventToAddDto.Location,
                eventToAddDto.StartDate,
                eventToAddDto.EndDate,
                eventToAddDto.MaxAttendees,
                eventToAddDto.CategoryId,
                eventToAddDto.CreatedBy
            });
        }
    }
}