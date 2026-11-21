namespace RentaAPIs.Models
{
    public class ViewingBooking
    {
        public Guid ViewingBookingId { get; set; }
        public Guid ViewingAvailabilityId { get; set; }
        public Guid PropertyId { get; set; }
        public Guid TenantId { get; set; }
        public DateTime ViewingDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }

        public ViewingAvailability ViewingAvailability { get; set; } = null!;
        public Property Property { get; set; } = null;
    }
}
