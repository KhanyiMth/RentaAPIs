namespace RentaAPIs.dtos
{
    public class TenantApplicationResponseDTO
    {
        public Guid TenantApplicationId { get; set; }
        public Guid TenantId { get; set; }
        public Guid AdvertisementId { get; set; }
        public Guid PropertyId { get; set; }
        public Guid? UnitId { get; set; }
        public string PropertyTitle { get; set; } = string.Empty;
        public string PropertyAddress { get; set; } = string.Empty;
        public string? UnitName { get; set; }
        public decimal RentAmount { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Cellphone { get; set; }
        public decimal MonthlyIncome { get; set; }
        public decimal MonthlyExpenses { get; set; }
        public int AppScore { get; set; }
        public decimal AcceptanceScore { get; set; }
        public string? PredictionLabel { get; set; }
        public string Status { get; set; } = "Pending";
        public string? TenantMessage { get; set; }
        public DateTime SubmittedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
