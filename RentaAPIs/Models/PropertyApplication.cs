using System.ComponentModel.DataAnnotations;

namespace RentaAPIs.Models
{
    public class PropertyApplication
    {
        [Key]
        public Guid ApplicationID { get; set; }
        public Guid TenantID { get; set; }
        public Guid PropertyID { get; set; }
        public DateTime ApplicationDate { get; set; }
        public string Status { get; set; }


        public Tenant Tenant { get; set; }
        public Property Property { get; set; }

    }
}
