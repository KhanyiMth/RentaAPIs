namespace RentaAPIs.Models
{
    public class ViewingAvailability
    {
        public Guid ViewingAvailabilityId { get; set; }
        public Guid PropertyId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int SlotDurationMinutes { get; set; }
        public bool isActive { get; set; }
        public DateTime CreatedDate { get; set; }


        public Property Property { get; set; } = null!;

        public ICollection<ViewingBooking> Bookings { get; set; } = new List<ViewingBooking>();

    }
}
