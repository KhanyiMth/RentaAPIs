namespace RentaAPIs.DTOs
{
    public class RentaMatchDTOs
    {
        public class RentaMatchRequestDTO
        {
            public float MonthlyIncome { get; set; }
            public float MonthlyExpenses { get; set; }
            public float PersonCount { get; set; }
            public float AdultCount { get; set; }
            public float EmployedCount { get; set; }
            public float AverageAge { get; set; }
            public float EmploymentRate { get; set; }

            public float AppScore { get; set; }

            public Guid PropertyId { get; set; }
            public float PropertyRent { get; set; }
            public float PropertyDeposit { get; set; }
            public int PropertyBedrooms { get; set; }
        }

        public class RentaMatchResponseDTO
        {
            public Guid PropertyId { get; set; }
            public float PredictedMonthlyRent { get; set; }
            public float PropertyRent { get; set; }
            public float FinancialFitScore { get; set; }
            public float BehaviourScore { get; set; }
            public float PropertyFitScore { get; set; }
            public float RentaMatchScore { get; set; }
            public string MatchLevel { get; set; } = string.Empty;
            public string FinancialInsight { get; set; } = string.Empty;
            public string MatchInsight { get; set; } = string.Empty;
        }

    }
}
