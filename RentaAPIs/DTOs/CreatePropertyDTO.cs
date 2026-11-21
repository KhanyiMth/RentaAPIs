namespace RentaAPIs.DTOs
{
    public class CreatePropertyDTO
    {
        public string Nickname { get; set; }
        public string Description { get; set; }
        public string StreetName { get; set; }
        public string StreetNumber { get; set; }
        public string Suburb { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public int Parking { get; set; }
        public string PropertyType { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public string Status { get; set; }
        public decimal RentAmount { get; set; }
        public decimal Deposit { get; set; }

        public List<string> Amenities { get; set; } = new();
    }
}
