using Microsoft.ML;
using RentaAPIs.ML.Models;
using static RentaAPIs.DTOs.RentaMatchDTOs;

namespace RentaAPIs.Services
{
    public class RentaMatchService
    {
        private readonly MLContext _mlContext;
        private readonly ITransformer _model;

        public RentaMatchService(IWebHostEnvironment environment)
        {
            _mlContext = new MLContext(seed: 42);

            var modelPath =
                @"D:\Console Apps\RentaAPIs\RentaAPIs\ML\Models\RentaMatch.mlnet";

            if (!File.Exists(modelPath))
            {
                throw new FileNotFoundException(
                    $"RentaMatch ML model was not found at: {modelPath}"
                );
            }

            _model = _mlContext.Model.Load(
                modelPath,
                out _
            );
        }

        public decimal PredictExpectedRent(
            decimal monthlyIncome,
            decimal monthlyExpenses,
            int householdSize)
        {
            if (monthlyIncome <= 0)
            {
                throw new ArgumentException(
                    "Monthly income must be greater than zero."
                );
            }

            if (monthlyExpenses < 0)
            {
                throw new ArgumentException(
                    "Monthly expenses cannot be negative."
                );
            }

            if (householdSize <= 0)
            {
                throw new ArgumentException(
                    "Household size must be greater than zero."
                );
            }

            var input = new ModelInput
            {
                MonthlyIncome = (float)monthlyIncome,
                MonthlyExpenses = (float)monthlyExpenses,
                HouseholdSize = householdSize
            };

            var predictionEngine =
                _mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(
                    _model
                );

            var prediction = predictionEngine.Predict(input);

            var predictedRent = (decimal)prediction.Score;

            if (predictedRent < 0)
            {
                predictedRent = 0;
            }

            return Math.Round(
                predictedRent,
                2,
                MidpointRounding.AwayFromZero
            );
        }
    }
}

