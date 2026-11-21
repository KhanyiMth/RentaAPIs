namespace RentaAPIs.dtos
{
    public class CreateViewingBookingDTO
    {
        public Guid ViewingAvailabilityId { get; set; }
        public TimeSpan StartTime { get; set; }

    }
}
