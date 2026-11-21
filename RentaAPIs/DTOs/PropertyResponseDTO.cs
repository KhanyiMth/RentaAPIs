namespace RentaAPIs.DTOs
{
    public class PropertyResponseDTO
    {
        public Guid PropertyId { get; set; }
        public Guid landlordID { get; set; }
        public string Nickname { get; set; }
        public string Description { get; set; }
        public string StreetName { get; set; }
        public string StreetNumber { get; set; }
        public string Suburb { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string PropertyType { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public int Parking { get; set; }
        public decimal RentAmount { get; set; }
        public decimal Deposit { get; set; }
        public string Status { get; set; }
        public bool isMultiUnit { get; set; }

        public bool IsAdvertised { get; set; }

        public List<AdvertisementSummaryDTO> Advertisements { get; set; }

        public bool HasSignedLease { get; set; }
        public List<SignedLeaseSummaryDTO> SignedLeases { get; set; }
        public List<UnitResponseDTO> Units { get; set; } = new();
        public List<AmenityDTO> Amenities { get; set; } = new();

    }

    public class AdvertisementSummaryDTO
    {
        public Guid AdvertisementId { get; set; }
        public Guid PropertyId { get; set; }
        public Guid? UnitId { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
    }

    public class SignedLeaseSummaryDTO
    {
        public Guid LeaseId { get; set; }
        public Guid TenantId { get; set; }
        public Guid PropertyId { get; set; }
        public Guid? UnitId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal MonthlyRent { get; set; }
        public decimal DepositAmount { get; set; }
        public string PaymentFrequency { get; set; }
        public string LeaseTerm { get; set; }
        public string Status { get; set; }
        public DateTime? TenantSignedAt { get; set; }
        public DateTime? ReturnedToLandlordAt { get; set; }
        public TenantSummaryDTO Tenant { get; set; }
    }

    public class TenantSummaryDTO
    {
        public Guid TenantId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
