namespace SmartX.Api.Models
{
    public class SensorRegistration
    {
        public string SensorId { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public bool IsOnline { get; set; } = true;

        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

        public DateTime LastSeen { get; set; } = DateTime.UtcNow;

        public List<string> Attachments { get; set; } = new();
    }
}