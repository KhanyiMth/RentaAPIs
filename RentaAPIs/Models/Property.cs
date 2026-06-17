namespace RentaAPIs.Models
{
    public class Property
    {
        public Guid PropertyID { get; set; }
        public Guid OwnerID { get; set; }
        public string Title { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string PropertyType { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public decimal RentAmount { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
