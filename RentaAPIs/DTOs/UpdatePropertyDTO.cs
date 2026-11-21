namespace RentaAPIs.cdDTOs
{
    public class UpdatePropertyDTO
    {
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
        public int Deposit { get; set; }
        public decimal RentAmount { get; set; }
        public string Status { get; set; }

    }
}
