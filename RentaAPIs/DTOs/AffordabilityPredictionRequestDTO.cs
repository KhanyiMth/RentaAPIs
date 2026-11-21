namespace RentaAPIs.DTOs
{
    public class AffordabilityPredictionRequestDTO
    {
        public float MonthlyIncome { get; set; }
        public float MonthlyExpenses { get; set; }
        public float DisposableIncome { get; set; }
        public float PersonCount { get; set; }
        public float AdultCount { get; set; }
        public float EmployedCount { get; set; }
        public float AverageAge { get; set; }
        public float EmploymentRate { get; set; }
    }
}
