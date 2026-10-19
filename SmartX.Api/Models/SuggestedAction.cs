namespace SmartX.Api.Models
{
    public class SuggestedAction
    {
        public string SensorId { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string Recommendation { get; set; } = string.Empty;

        public string Reason { get; set; } = string.Empty;

        public string Priority { get; set; } = "Normal";

        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }
}