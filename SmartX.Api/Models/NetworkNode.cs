namespace SmartX.Api.Models
{
    public class NetworkNode
    {
        public string SensorId { get; set; } = string.Empty;

        public string MacAddress { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        public string DeviceType { get; set; } = "ESP32";

        public bool IsOnline { get; set; } = true;

        public DateTime LastSeen { get; set; } = DateTime.UtcNow;
    }
}