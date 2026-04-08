namespace Eventify_High_Performance_Event_Management_API.Dtos
{
    public class BookingViewDto
    {
        public int BookingId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public string NameCategory { get; set; } = string.Empty;
        public byte Status { get; set; } // 1: Confirmed, 2: Cancelled, 3: Attended
    }
}