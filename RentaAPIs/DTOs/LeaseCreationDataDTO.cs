namespace RentaAPIs.DTOs
{
    public class LeaseCreationDataDTO
    {
        public Guid ApplicationId { get; set; }
        public Guid TenantId { get; set; }
        public string TenantName { get; set; } = string.Empty;
        public string TenantIDNumber { get; set; } = string.Empty;
        public Guid LandlordId { get; set; }
        public string LandlordName { get; set; } = string.Empty;
        public string LandlordIDNumber { get; set; } = string.Empty;
        public string LandlordCellphone { get; set; } = string.Empty;
        public Guid PropertyId { get; set; }
        public string PropertyName { get; set; } = string.Empty;
        public string PropertyAddress { get; set; } = string.Empty;
        public Guid? UnitId { get; set; }
        public string UnitName { get; set; } = string.Empty;
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public int Parking { get; set; }
        public decimal MonthlyRent { get; set; }
        public decimal DepositAmount { get; set; }
        public string PaymentFrequency { get; set; } = "Monthly";
        public int RentDueDay { get; set; } = 1;
    }
}
