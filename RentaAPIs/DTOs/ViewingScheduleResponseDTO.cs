namespace RentaAPIs.cdDTOs
{
    public class ViewingScheduleResponseDTO
    {

        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public Guid PropertyId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int SlotsCreated { get; set; }

        public int SlotsSkipped { get; set; }

        public List<Guid> CreatedAvailabilityIds { get; set; } = new();
    }
}
