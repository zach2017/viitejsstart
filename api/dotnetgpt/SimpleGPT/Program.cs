using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.ML;
using Microsoft.ML.Data;

class Program
{
    private static string dataFile = "training_data.txt";
    private static List<TrainingData> data = new List<TrainingData>();
    private static MLContext mlContext = new MLContext();
    private static ITransformer model;
    private static PredictionEngine<TrainingData, Prediction> predictor;

    static void Main()
    {
        LoadTrainingData();  // Load existing data from file
        TrainModel();  // Train the model

        while (true)
        {
            Console.Write("\nEnter 1-3 words: ");
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input) || input.ToLower() == "exit")
                break;

            var prediction = predictor.Predict(new TrainingData { InputText = input });

            // Check if the response is in known training data
            var knownResponse = data.Find(d => d.InputText.Equals(input, StringComparison.OrdinalIgnoreCase));

            if (knownResponse != null)
            {
                Console.WriteLine($"Response: {prediction.ResponseText}");
            }
            else
            {
                Console.Write("I don't understand. Please teach me a response: ");
                var newResponse = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(newResponse))
                {
                    // Add new training data
                    data.Add(new TrainingData { InputText = input, ResponseText = newResponse });
                    SaveTrainingData();  // Save new response
                    TrainModel();  // Re-train model
                    Console.WriteLine("Thank you! I've learned a new response.");
                }
            }
        }
    }

    static void LoadTrainingData()
    {
        if (File.Exists(dataFile))
        {
            var lines = File.ReadAllLines(dataFile);
            foreach (var line in lines)
            {
                var parts = line.Split('|');
                if (parts.Length == 2)
                {
                    data.Add(new TrainingData { InputText = parts[0], ResponseText = parts[1] });
                }
            }
        }

        // If no training data exists, add default responses
        if (data.Count == 0)
        {
            data = new List<TrainingData>
            {
                new TrainingData { InputText = "hello", ResponseText = "Hi there! How can I help?" },
                new TrainingData { InputText = "good morning", ResponseText = "Good morning! Have a great day!" },
                new TrainingData { InputText = "how are you", ResponseText = "I'm doing great, thanks for asking!" },
                new TrainingData { InputText = "weather", ResponseText = "The weather is nice today!" },
                new TrainingData { InputText = "goodbye", ResponseText = "Goodbye! Have a wonderful day!" },
                new TrainingData { InputText = "help", ResponseText = "Sure, what do you need help with?" },
                new TrainingData { InputText = "who are you", ResponseText = "I'm a simple AI trained to chat with you!" },
                new TrainingData { InputText = "thanks", ResponseText = "You're welcome!" }
            };
            SaveTrainingData(); // Save default responses to file
        }
    }

    static void SaveTrainingData()
    {
        var lines = new List<string>();
        foreach (var entry in data)
        {
            lines.Add($"{entry.InputText}|{entry.ResponseText}");
        }
        File.WriteAllLines(dataFile, lines);
    }

    static void TrainModel()
    {
        var trainingData = mlContext.Data.LoadFromEnumerable(data);

        var pipeline = mlContext.Transforms.Text.FeaturizeText("Features", nameof(TrainingData.InputText))
            .Append(mlContext.Transforms.Conversion.MapValueToKey("Label", nameof(TrainingData.ResponseText)))
            .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy())
            .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

        model = pipeline.Fit(trainingData);

        predictor = mlContext.Model.CreatePredictionEngine<TrainingData, Prediction>(model);
    }
}

// Define the input/output data classes
public class TrainingData
{
    [LoadColumn(0)]
    public string InputText { get; set; }

    [LoadColumn(1)]
    public string ResponseText { get; set; }
}

public class Prediction
{
    [ColumnName("PredictedLabel")]
    public string ResponseText { get; set; }
}
