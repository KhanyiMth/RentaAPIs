using System.ComponentModel.DataAnnotations;

namespace RentaAPIs.Models
{
    public class Unit
    {
        [Key]
        public Guid UnitId {  get; set; }
        public Guid PropertyId {  get; set; }
        public string UnitName { get; set; }
        public int UnitNumber { get; set; }
        public int Parking { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public decimal RentAmount { get; set; }
        public decimal Deposit { get; set; }
        public string Description { get; set; }
        public string Status { get; set; } = "Available";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Property property { get; set; }
        public bool isOccupied { get; set; }

        public ICollection<Lease> Leases {  get; set; } = new List<Lease>();
        public ICollection<MaintenanceRequest> MaintenanceRequests { get; set;} = new List<MaintenanceRequest>();

    }
}
