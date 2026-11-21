namespace RentaAPIs.cdDTOs
{
    public class AdvertisementResponseDTO
    {
        public Guid AdvertisementId{ get; set; }
        public Guid PropertyId { get; set; }
        public Guid? UnitId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public decimal RentAmount { get; set; }
        public decimal Deposit { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? PropertyName { get; set; }
        public string? UnitName { get; set; }
        public string? Address { get; set; }
        public string? StreetName { get; set; }
        public string? StreetNumber { get; set; }
        public string? Province { get; set; }
        public string? Suburb { get; set; }
        public string? City { get; set; }
        public string? PropertyType { get; set; }
        public int? Bedrooms { get; set; } 
        public int? Bathrooms { get; set; } 

        public Guid? LandlordId { get; set; }
        public string? LandlordName { get; set; }
        public string? LandlordEmail { get; set; }
        public string? LandlordPhone { get; set; }

        public List<string> Amenities { get; set; } = new();

    }
}
