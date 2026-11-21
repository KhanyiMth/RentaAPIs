namespace RentaAPIs.DTOs
{
    public class LeaseResponseDTO
    {
        public Guid LeaseId { get; set; }
        public Guid ApplicationId { get; set; }

        public Guid TenantId { get; set; }
        public string TenantName { get; set; } = string.Empty;
        public string TenantIDNumber { get; set; } = string.Empty;
        public string TenantAddress { get; set; } = string.Empty;

        public Guid LandlordId { get; set; }
        public string LandlordName { get; set; } = string.Empty;
        public string LandlordIDNumber { get; set; } = string.Empty;
        public string LandlordAddress { get; set; } = string.Empty;

        public Guid PropertyId { get; set; }
        public string PropertyName { get; set; } = string.Empty;
        public string PropertyAddress { get; set; } = string.Empty;

        public Guid? UnitId { get; set; }
        public string UnitName { get; set; } = string.Empty;
        public int UnitNumber { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string LeaseTerm { get; set; } = string.Empty;

        public decimal MonthlyRent { get; set; }
        public decimal DepositAmount { get; set; }

        public string PaymentFrequency { get; set; } = "Monthly";
        public int RentDueDay { get; set; } = 1;
        public DateTime DueDate { get; set; }

        public string Terms { get; set; } = string.Empty;

        public List<string> UtilitiesIncluded { get; set; } = new();
        public bool PetsAllowed { get; set; }
        public string SpecialConditions { get; set; } = string.Empty;
        public string AgreementVersion { get; set; } = "1.0";

        public string Status { get; set; } = string.Empty;

        public bool IsSignedByTenant { get; set; }
        public bool IsSignedByLandlord { get; set; }

        public DateTime? TenantSignedAt { get; set; }
        public DateTime? LandlordSignedAt { get; set; }
        public DateTime? ReturnedToLandlordAt { get; set; }

        public DateTime CreatedDate { get; set; }

    }
}
