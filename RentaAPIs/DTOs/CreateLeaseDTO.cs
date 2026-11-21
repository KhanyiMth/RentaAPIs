namespace RentaAPIs.DTOs
{
    public class CreateLeaseDTO
    {
        public Guid ApplicationId { get; set; }
        public DateTime StartDate { get; set; }
        public string LeaseTerm { get; set; } = string.Empty;
        public string PaymentFrequency { get; set; } = "Monthly";
        public int RentDueDay { get; set; } = 1;
        public string Terms { get; set; } = string.Empty;
        public List<string> UtilitiesIncluded { get; set; } = new();
        public bool PetsAllowed { get; set; } = false;
        public string SpecialConditions { get; set; } = string.Empty;

    }
}
