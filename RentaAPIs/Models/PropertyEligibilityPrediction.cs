using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace RentaAPIs.Models
{
    public class PropertyEligibilityPrediction
    {
        [Key]
        public Guid PredicitionID { get; set; }
        public Guid TenantID { get; set; }
        public Guid PropertyID { get; set; }
        public decimal Probability { get; set; }
        public string EligibilityLevel { get; set; }
        public DateTime CalculatedAt { get; set; }
        public string ModelVersion { get; set; }


        public Tenant Tenant { get; set; }
        public Property Property { get; set; }
    }
}
