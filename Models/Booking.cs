namespace Eventify_High_Performance_Event_Management_API.Models
{
    public class Booking
    {
        public int BookingId { get; set; }
        public int EventId { get; set; }
        public int UserId { get; set; }
        public DateTime BookingDate { get; set; } = DateTime.Now;
        public byte Status { get; set; } // 1: Confirmed, 2: Cancelled
    }
}