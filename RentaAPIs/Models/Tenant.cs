using System.ComponentModel.DataAnnotations;

namespace RentaAPIs.Models
{
    public class Tenant
    {
        [Key]
        public Guid TenantId { get; set; }
        public string UserId { get; set; }
        public Guid? LandlordID { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        //public string Email { get; set; }
        public string Cellphone { get; set; }
        //public string PasswordHash { get; set; }
        public DateTime CreatedDate { get; set; }
        public string IDNumber { get; set; }
        public string EmergencyContactName { get; set; }
        public string EmergencyContactPhone { get; set; }
        public string Occupation {  get; set; }
        public string EmployerName { get; set; }
        public decimal MonthlyIncome { get; set; }
        public decimal MonthlyExpenses { get; set; }
        public int HouseholdSize { get; set; }
        public int AppScore { get; set; }
        public string Address { get; set; } = string.Empty;

        public Landlord? Landlord { get; set; }
        public ICollection<Lease> Leases { get; set; } = new List<Lease>();
        public ICollection<MaintenanceRequest> MaintenanceRequests { get; set; } = new List<MaintenanceRequest>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public ICollection<Document> Documents { get; set; } = new List<Document>();
        public ICollection<PropertyApplication> PropertyApplications { get; set; } = new List<PropertyApplication>();
    }
}
