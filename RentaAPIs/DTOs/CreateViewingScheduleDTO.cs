namespace RentaAPIs.DTOs
{
    public class CreateViewingScheduleDTO
    {
        public Guid PropertyId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public List<DayOfWeek> DaysOfWeek { get; set; } = new();

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public int SlotDurationMinutes { get; set; }
    }
}
