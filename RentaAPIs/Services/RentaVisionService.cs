using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using RentaAPIs.DTOs;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace RentaAPIs.Services
{
    public class RentaVisionService : IDisposable
    {
        private readonly InferenceSession _session;

        private static readonly string[] Classes =
        {
            "cracks",
            "undamaged",
            "water_damage"
        };

        private static readonly float[] Mean =
        {
            0.485f, 0.456f, 0.406f
        };

        private static readonly float[] Std =
        {
            0.229f, 0.224f, 0.225f
        };

        public RentaVisionService(IWebHostEnvironment environment)
        {
            var modelPath = Path.Combine(
                environment.ContentRootPath,
                "ML",
                "Models",
                "RentaVision_MobileNetV3.onnx"
            );

            if (!File.Exists(modelPath))
            {
                throw new FileNotFoundException(
                    "RentaVision ONNX model was not found.",
                    modelPath
                );
            }

            _session = new InferenceSession(modelPath);
        }

        public async Task<RentaVisionPredictionDTO> PredictAsync(
            IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException(
                    "Please upload a valid maintenance image."
                );

            if (file.Length > 10 * 1024 * 1024)
                throw new ArgumentException(
                    "The image must not exceed 10 MB."
                );

            var allowedExtensions = new[]
            {
                ".jpg", ".jpeg", ".png", ".webp"
            };

            var extension = Path.GetExtension(
                file.FileName
            ).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                throw new ArgumentException(
                    "Only JPG, PNG and WEBP images are supported."
                );

            using var stream = file.OpenReadStream();

            using var image = await Image.LoadAsync<Rgb24>(
                stream
            );

            // Match the PyTorch evaluation preprocessing.
            image.Mutate(x => x.Resize(
                new ResizeOptions
                {
                    Size = new Size(224, 224),
                    Mode = ResizeMode.Stretch,
                    Sampler = KnownResamplers.Triangle
                }
            ));

            var tensor = new DenseTensor<float>(
                new[] { 1, 3, 224, 224 }
            );

            for (int y = 0; y < 224; y++)
            {
                for (int x = 0; x < 224; x++)
                {
                    var pixel = image[x, y];

                    tensor[0, 0, y, x] =
                        ((pixel.R / 255f) - Mean[0]) / Std[0];

                    tensor[0, 1, y, x] =
                        ((pixel.G / 255f) - Mean[1]) / Std[1];

                    tensor[0, 2, y, x] =
                        ((pixel.B / 255f) - Mean[2]) / Std[2];
                }
            }

            var inputName = _session.InputMetadata.Keys.First();

            using var results = _session.Run(
                new[]
                {
                    NamedOnnxValue.CreateFromTensor(
                        inputName,
                        tensor
                    )
                }
            );

            var logits = results.First()
                .AsEnumerable<float>()
                .ToArray();

            if (logits.Length != Classes.Length)
                throw new InvalidOperationException(
                    "Unexpected CNN output dimensions."
                );

            // Numerically stable softmax.
            float maxLogit = logits.Max();

            var exponentials = logits
                .Select(v => MathF.Exp(v - maxLogit))
                .ToArray();

            float sum = exponentials.Sum();

            var probabilities = exponentials
                .Select(v => v / sum)
                .ToArray();

            int predictedIndex = Array.IndexOf(
                probabilities,
                probabilities.Max()
            );

            string category = Classes[predictedIndex];
            float confidence = probabilities[predictedIndex];

            string priority;
            string description;

            switch (category)
            {
                case "water_damage":
                    priority = "Needs Review";
                    description =
                        "Possible water-related damage detected. " +
                        "Inspect the affected area to determine " +
                        "the source and urgency.";
                    break;

                case "cracks":
                    priority = "Needs Review";
                    description =
                        "Visible cracking may be present. " +
                        "Inspect the affected surface to determine " +
                        "the extent and required repairs.";
                    break;

                default:
                    priority = "Needs Review";
                    description =
                        "No damage was identified by the image " +
                        "classifier. Please describe the issue " +
                        "and request an inspection if necessary.";
                    break;
            }

            return new RentaVisionPredictionDTO
            {
                PredictedCategory = category,
                Confidence = confidence,
                SuggestedPriority = priority,
                SuggestedDescription = description,
                RequiresReview = true,
                Probabilities = Classes
                    .Select((name, index) => new
                    {
                        name,
                        probability = probabilities[index]
                    })
                    .ToDictionary(
                        x => x.name,
                        x => x.probability
                    )
            };
        }

        public void Dispose()
        {
            _session.Dispose();
        }
    }
}
