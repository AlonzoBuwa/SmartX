namespace SmartX.Api.Models
{
    public class DeviceCommand
    {
        public string CommandId { get; set; } = Guid.NewGuid().ToString();

        public string SensorId { get; set; } = string.Empty;

        public string Command { get; set; } = string.Empty;

        public string PreviousState { get; set; } = string.Empty;

        public string NewState { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public string Status { get; set; } = "Executed";
    }
}