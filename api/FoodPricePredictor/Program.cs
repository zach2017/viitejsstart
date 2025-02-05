using Microsoft.ML;
using Microsoft.ML.Data;

namespace FoodPricePredictor
{
    public class FoodPriceData
    {
        [LoadColumn(0)]
        public string FoodItem { get; set; }
        [LoadColumn(1)]
        public string FoodCategory { get; set; }
        [LoadColumn(2)]
        public float Month { get; set; }
        [LoadColumn(3)]
        public float Year { get; set; }
        [LoadColumn(4)]
        public string SourceCountry { get; set; }
        [LoadColumn(5)]
        public float TariffRate { get; set; }
        [LoadColumn(6)]
        public string DisasterType { get; set; }
        [LoadColumn(7)]
        public float DisasterSeverity { get; set; }
        [LoadColumn(8)]
        public float DisasterMultiplier { get; set; }
        [LoadColumn(9)]
        public float Price { get; set; }
    }

    public class FoodPricePrediction
    {
        [ColumnName("Score")]
        public float PredictedPrice { get; set; }
    }

    class Program
    {
        static readonly string _dataPath = Path.Combine(Environment.CurrentDirectory, "food-prices.csv");
        static readonly string _modelPath = Path.Combine(Environment.CurrentDirectory, "FoodPriceModel.zip");

        static void Main(string[] args)
        {
            var mlContext = new MLContext(seed: 0);
            var model = TrainModel(mlContext);
            
            while (true)
            {
                var inputData = GetUserInput();
                if (inputData == null) break;
                
                var predictor = mlContext.Model.CreatePredictionEngine<FoodPriceData, FoodPricePrediction>(model);
                var prediction = predictor.Predict(inputData);
                
                Console.WriteLine($"\nPredicted price for {inputData.FoodItem} ({inputData.FoodCategory}): ${prediction.PredictedPrice:F2}");
                Console.WriteLine($"Factors: {inputData.DisasterType} (Severity: {inputData.DisasterSeverity:F2}, Multiplier: {inputData.DisasterMultiplier:F1}x)");
                Console.WriteLine("\nPress any key to make another prediction or 'q' to quit.");
                if (Console.ReadKey().KeyChar == 'q') break;
                Console.Clear();
            }
        }

        static ITransformer TrainModel(MLContext mlContext)
        {
            var dataView = mlContext.Data.LoadFromTextFile<FoodPriceData>(_dataPath, hasHeader: true, separatorChar: ',');

            var pipeline = mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: "Price")
                .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "FoodItemEncoded", inputColumnName: "FoodItem"))
                .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "FoodCategoryEncoded", inputColumnName: "FoodCategory"))
                .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "SourceCountryEncoded", inputColumnName: "SourceCountry"))
                .Append(mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "DisasterTypeEncoded", inputColumnName: "DisasterType"))
                .Append(mlContext.Transforms.NormalizeMinMax("MonthNorm", "Month"))
                .Append(mlContext.Transforms.NormalizeMinMax("YearNorm", "Year"))
                .Append(mlContext.Transforms.NormalizeMinMax("TariffRateNorm", "TariffRate"))
                .Append(mlContext.Transforms.NormalizeMinMax("DisasterSeverityNorm", "DisasterSeverity"))
                .Append(mlContext.Transforms.NormalizeMinMax("DisasterMultiplierNorm", "DisasterMultiplier"))
                .Append(mlContext.Transforms.Concatenate("Features",
                    "FoodItemEncoded", "FoodCategoryEncoded", "MonthNorm", "YearNorm",
                    "SourceCountryEncoded", "TariffRateNorm", "DisasterTypeEncoded",
                    "DisasterSeverityNorm", "DisasterMultiplierNorm"))
                .Append(mlContext.Regression.Trainers.Sdca(maximumNumberOfIterations: 100));

            var trainTestSplit = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);
            var model = pipeline.Fit(trainTestSplit.TrainSet);

            var predictions = model.Transform(trainTestSplit.TestSet);
            var metrics = mlContext.Regression.Evaluate(predictions);
            Console.WriteLine($"Model Metrics:");
            Console.WriteLine($"R²: {metrics.RSquared:F2}");
            Console.WriteLine($"RMSE: {metrics.RootMeanSquaredError:F2}\n");

            return model;
        }

        static FoodPriceData GetUserInput()
        {
            Console.WriteLine("\nEnter food price prediction details:");
            
            Console.Write("Food Item (Beef, Chicken, Milk, Bread, etc.): ");
            string foodItem = Console.ReadLine();
            
            Console.Write("Food Category (Meat, Dairy, Grain): ");
            string foodCategory = Console.ReadLine();
            
            Console.Write("Month (1-12): ");
            float month = float.Parse(Console.ReadLine());
            
            Console.Write("Year: ");
            float year = float.Parse(Console.ReadLine());
            
            Console.Write("Source Country: ");
            string sourceCountry = Console.ReadLine();
            
            Console.Write("Tariff Rate (0.0-1.0): ");
            float tariffRate = float.Parse(Console.ReadLine());
            
            Console.Write("Disaster Type (None, Drought, Flood): ");
            string disasterType = Console.ReadLine();
            
            Console.Write("Disaster Severity (0.0-1.0): ");
            float disasterSeverity = float.Parse(Console.ReadLine());
            
            float disasterMultiplier = 1.0f;
            if (disasterType.ToLower() == "drought")
            {
                disasterMultiplier = foodCategory.ToLower() == "meat" ? 1.8f :
                                   foodCategory.ToLower() == "dairy" ? 1.5f :
                                   foodCategory.ToLower() == "grain" ? 1.6f : 1.0f;
            }

            return new FoodPriceData
            {
                FoodItem = foodItem,
                FoodCategory = foodCategory,
                Month = month,
                Year = year,
                SourceCountry = sourceCountry,
                TariffRate = tariffRate,
                DisasterType = disasterType,
                DisasterSeverity = disasterSeverity,
                DisasterMultiplier = disasterMultiplier
            };
        }
    }
}