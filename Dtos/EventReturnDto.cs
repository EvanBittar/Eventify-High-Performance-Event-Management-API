namespace Eventify_High_Performance_Event_Management_API.Dtos
{
    public class EventReturnDto
    {
        public string Title { get; set; } = "";
        public decimal AverageRating { get; set; }
        public int ReviewsCount { get; set; }
        public string Location { get; set; } = "";
    }
}