using Microsoft.ML.Data;

namespace RentaAPIs.ML.Models
{
    public class ModelInput
    {
        [LoadColumn(0)]
        public float MonthlyIncome { get; set; }

        [LoadColumn(1)]
        public float MonthlyExpenses { get; set; }

        [LoadColumn(2)]
        public float HouseholdSize { get; set; }

        [LoadColumn(3)]
        public float MonthlyRent { get; set; }
    }
}
