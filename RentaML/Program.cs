using Microsoft.ML;
using Microsoft.ML.Data;
using RentaML;
using static RentaML.RentaMatch;

class Program
{
    static void Main(string[] args)
    {
        var mlContext = new MLContext(seed: 42);

        string modelPath = Path.Combine(
            AppContext.BaseDirectory,
            "RentaMatch.mlnet"
        );

        string testDataPath = Path.Combine(
            AppContext.BaseDirectory,
            "RentaMatch_Test.csv"
        );

        // ============================================
        // CHECK FILES
        // ============================================

        if (!File.Exists(modelPath))
        {
            Console.WriteLine("ERROR: Model file not found:");
            Console.WriteLine(modelPath);
            return;
        }

        if (!File.Exists(testDataPath))
        {
            Console.WriteLine("ERROR: Test dataset not found:");
            Console.WriteLine(testDataPath);
            return;
        }

        Console.WriteLine("============================================");
        Console.WriteLine("RENTA MATCH ML.NET EVALUATION");
        Console.WriteLine("============================================");

        Console.WriteLine();
        Console.WriteLine("Model:");
        Console.WriteLine(modelPath);

        Console.WriteLine();
        Console.WriteLine("Test dataset:");
        Console.WriteLine(testDataPath);

        // ============================================
        // LOAD TEST DATA
        // ============================================

        IDataView testData = mlContext.Data.LoadFromTextFile<ModelInput>(
            testDataPath,
            hasHeader: true,
            separatorChar: ','
        );

        // ============================================
        // LOAD TRAINED MODEL
        // ============================================

        ITransformer model = mlContext.Model.Load(
            modelPath,
            out DataViewSchema modelSchema
        );

        // ============================================
        // MAKE PREDICTIONS
        // ============================================

        IDataView predictions = model.Transform(testData);

        // ============================================
        // EVALUATE REGRESSION MODEL
        // ============================================

        RegressionMetrics metrics = mlContext.Regression.Evaluate(
            predictions,
            labelColumnName: "MonthlyRent",
            scoreColumnName: "Score"
        );

        // ============================================
        // DISPLAY RESULTS
        // ============================================

        Console.WriteLine();
        Console.WriteLine("============================================");
        Console.WriteLine("MODEL PERFORMANCE");
        Console.WriteLine("============================================");

        Console.WriteLine(
            $"Mean Absolute Error (MAE): R{metrics.MeanAbsoluteError:N2}"
        );

        Console.WriteLine(
            $"Root Mean Squared Error (RMSE): R{metrics.RootMeanSquaredError:N2}"
        );

        Console.WriteLine(
            $"R²: {metrics.RSquared:F4}"
        );

        Console.WriteLine();
        Console.WriteLine("============================================");
        Console.WriteLine("INTERPRETATION");
        Console.WriteLine("============================================");

        Console.WriteLine(
            $"On average, the model's prediction differs from the actual rent by approximately R{metrics.MeanAbsoluteError:N2}."
        );

        Console.WriteLine(
            $"The model explains approximately {metrics.RSquared * 100:F2}% of the variation in rental levels."
        );

        // ============================================
        // DISPLAY SAMPLE PREDICTIONS
        // ============================================

        Console.WriteLine();
        Console.WriteLine("============================================");
        Console.WriteLine("SAMPLE PREDICTIONS");
        Console.WriteLine("============================================");

        var predictionRows = mlContext.Data
            .CreateEnumerable<ModelInput>(testData, reuseRowObject: false)
            .ToList();

        var predictionResults = mlContext.Data
            .CreateEnumerable<ModelOutput>(
                predictions,
                reuseRowObject: false
            )
            .ToList();

        int sampleCount = Math.Min(
            10,
            predictionRows.Count
        );

        for (int i = 0; i < sampleCount; i++)
        {
            float actualRent = predictionRows[i].MonthlyRent;

            float predictedRent = predictionResults[i].Score;

            Console.WriteLine();
            Console.WriteLine($"Record {i + 1}");
            Console.WriteLine($"Actual Rent:    R{actualRent:N2}");
            Console.WriteLine($"Predicted Rent: R{predictedRent:N2}");
            Console.WriteLine(
                $"Difference:     R{Math.Abs(actualRent - predictedRent):N2}"
            );
        }

        Console.WriteLine();
        Console.WriteLine("============================================");
        Console.WriteLine("EVALUATION COMPLETE");
        Console.WriteLine("============================================");
    }
}