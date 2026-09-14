using SmartX.Api.Models;

namespace SmartX.Api.Services
{
    public class TelemetryProcessor
    {
        public List<TelemetryPacket<double>> ProcessHistoricalData(
            double[][] historicalData)
        {
            var processedData =
                new List<TelemetryPacket<double>>();

            for (int i = 0; i < historicalData.Length; i++)
            {
                for (int j = 0; j < historicalData[i].Length; j++)
                {
                    var packet = new TelemetryPacket<double>
                    {
                        SensorId = $"HIST-{i + 1}",
                        Location = $"Zone {i + 1}",
                        Category = "Historical",
                        Value = historicalData[i][j],
                        Timestamp = DateTime.UtcNow.AddMinutes(-j)
                    };

                    processedData.Add(packet);
                }
            }

            return processedData;
        }
    }
}