namespace RentaAPIs.dtos
{
    public class TenantApplicationPreviewdto
    {
        public Guid AdvertismentId { get; set; }
        public Guid PropertyId { get; set; }
        public Guid? UnitId { get; set; }

        public string PropertyTitle { get; set; }
        public string PropertyAddress { get; set; }
        public string? UnitName { get; set; }
        public decimal RentAmount { get; set; }

        public Guid TenantId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string? Cellphone { get; set; }

        public decimal MonthlyIncome { get; set; }
        public decimal MonthlyExpenses { get; set; }

        public int AppScore { get; set; }
        public decimal AcceptanceScore { get; set; }
        public string PredictionLabel { get; set; } = "Unknown";

    }
}
