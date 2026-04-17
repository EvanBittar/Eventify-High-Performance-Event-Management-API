using Eventify_High_Performance_Event_Management_API.Dtos;

namespace Eventify_High_Performance_Event_Management_API.Repository
{
    public interface IReviewRepository
    {
        Task<string> AddReviewAsync(ReviewDto reviewDto , int userId);
        Task<IEnumerable<dynamic>> GetEventReviewsAsync(int eventId);

    }
}