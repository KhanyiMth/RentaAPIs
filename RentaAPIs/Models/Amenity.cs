namespace RentaAPIs.Models
{
    public class Amenity
    {
        public Guid Id { get; set; }
        public string AmenityName { get; set; }

        public ICollection<Property> Properties { get; set; } = new List<Property>();

    }
}
