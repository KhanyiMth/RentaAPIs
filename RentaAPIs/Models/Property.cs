using System.ComponentModel.DataAnnotations;

namespace RentaAPIs.Models
{
    public class Property
    {
        [Key]
        public Guid PropertyId { get; set; }
        public Guid LandlordId { get; set; }
        public ICollection<Unit> Units { get; set; }
        public string Nickname { get; set; }
        public string Description { get; set; }
        public string StreetName { get; set; }
        public string StreetNumber { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Suburb { get; set; }
        public string PropertyType { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public int Parking { get; set; }
        public decimal RentAmount { get; set; }
        public decimal Deposit { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }


        public ICollection<MaintenanceRequest> MaintenanceRequests { get; set; } = new List<MaintenanceRequest>();
        public Landlord Landlord { get; set; }
        public ICollection<Document> Documents { get; set; } = new List<Document>();
        public ICollection<Amenity> Amenities { get; set; } = new List<Amenity>();

        public ICollection<ViewingAvailability> ViewingAvailabilities { get; set; } = new List<ViewingAvailability>();

        public ICollection<ViewingBooking> ViewingBookings { get; set; } = new List<ViewingBooking>();
    }
}
