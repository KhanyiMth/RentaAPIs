namespace RentaAPIs.DTOs
{
    public class RentaVisionPredictionDTO
    {
        public string PredictedCategory { get; set; } = string.Empty;
        public float Confidence { get; set; }
        public string SuggestedPriority { get; set; } = string.Empty;
        public string SuggestedDescription { get; set; } = string.Empty;
        public Dictionary<string, float> Probabilities { get; set; } = new();
        public bool RequiresReview { get; set; } = true;
    }
}
