namespace RentaAPIs.DTOs
{
    public class AffordabilityPredictionDTO
    {
        public float MonthlyIncome { get; set; }
        public float MonthlyExpenses { get; set; }
        public float DisposableIncome { get; set; }
        public float PersonCount { get; set; }
        public float AdultCount { get; set; }
        public float EmployedCount { get; set; }
        public float AverageAge { get; set; }
        public float EmploymentRate { get; set; }
        public string AffordabilityClass { get; set; } = string.Empty;
        public float AffordableScore { get; set; }
        public float ModerateScore { get; set; }
        public float HighRiskScore { get; set; }
    }
}
