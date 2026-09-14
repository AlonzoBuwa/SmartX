using SmartX.Api.Models;

namespace SmartX.Api.Services
{
    public class TelemetryService
    {
        private readonly List<object> _telemetry = new();
        private readonly List<SensorRegistration> _sensors = new();

        private const int OfflineThresholdSeconds = 15;

        public void AddTelemetry<T>(TelemetryPacket<T> packet)
        {
            _telemetry.Add(packet);

            var sensor = _sensors.FirstOrDefault(
                s => s.SensorId == packet.SensorId);

            if (sensor != null)
            {
                sensor.IsOnline = true;
                sensor.LastSeen = DateTime.UtcNow;
            }
        }

        public List<object> GetTelemetry()
        {
            return _telemetry;
        }

        public void RegisterSensor(SensorRegistration sensor)
        {
            var existing = _sensors.FirstOrDefault(
                s => s.SensorId == sensor.SensorId);

            if (existing == null)
            {
                sensor.LastSeen = DateTime.UtcNow;
                sensor.IsOnline = true;

                _sensors.Add(sensor);
            }
        }

        public List<SensorRegistration> GetSensors()
        {
            UpdateSensorStatuses();

            return _sensors;
        }

        public SensorRegistration? GetSensor(string sensorId)
        {
            UpdateSensorStatuses();

            return _sensors.FirstOrDefault(
                s => s.SensorId == sensorId);
        }

        public bool RemoveSensor(string sensorId)
        {
            var sensor = GetSensor(sensorId);

            if (sensor == null)
                return false;

            _sensors.Remove(sensor);

            return true;
        }

        private void UpdateSensorStatuses()
        {
            foreach (var sensor in _sensors)
            {
                var secondsSinceLastSeen =
                    (DateTime.UtcNow - sensor.LastSeen).TotalSeconds;

                sensor.IsOnline =
                    secondsSinceLastSeen <= OfflineThresholdSeconds;
            }
        }
    }
}