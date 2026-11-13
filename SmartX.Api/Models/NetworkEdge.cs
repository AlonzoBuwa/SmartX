namespace SmartX.Api.Models
{
    public class NetworkEdge
    {
        public string SourceSensorId { get; set; } = string.Empty;

        public string DestinationSensorId { get; set; } = string.Empty;

        public double EnergyCost { get; set; }

        public bool IsActive { get; set; } = true;
    }
}