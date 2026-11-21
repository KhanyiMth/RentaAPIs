namespace RentaAPIs.Services
{
    public class MaintenanceRequestTriageService
    {
        public string DeterminePriority(
            string? predictedCategory,
            float? confidence,
            string? category,
            string? title,
            string? description)
        {
            string text = string.Join(" ",
                category ?? "",
                title ?? "",
                description ?? ""
            ).ToLowerInvariant();

            // Potential immediate safety hazards.
            string[] emergencyIndicators =
            {
                "exposed wiring",
                "electric shock",
                "sparking wires",
                "smell of gas",
                "gas leak",
                "ceiling collapsing",
                "wall collapsing",
                "structural collapse",
                "fire",
                "burnt",
            };

            if (emergencyIndicators.Any(x => text.Contains(x)))
                return "Emergency";

            // Significant issues requiring prompt attention.
            string[] urgentIndicators =
            {
                "burst pipe",
                "major leak",
                "flooding",
                "leaking",
                "water pouring",
                "no electricity",
                "no running water",
                "blocked only toilet",
                "door cannot lock",
                "ceiling leaking"
            };

            if (urgentIndicators.Any(x => text.Contains(x)))
                return "Urgent";

            // Image classification provides supporting evidence,
            // but does not establish damage severity.
            bool reliablePrediction =
                confidence.HasValue &&
                confidence.Value >= 0.80f;

            if (reliablePrediction &&
                predictedCategory == "water_damage" &&
                text.Contains("leak"))
            {
                return "Urgent";
            }

            // Unknown or ordinary maintenance issues receive
            // normal scheduling, with landlord review.
            return "Routine";
        }

    }
}
