using SmartX.Api.Models;

namespace SmartX.Api.Services
{
    public class TelemetryService
    {
        // ============================================================
        // PART 1 COLLECTIONS
        // ============================================================

        private readonly List<object> _telemetry = new();
        private readonly List<SensorRegistration> _sensors = new();

        // ============================================================
        // PART 2 ADVANCED COLLECTIONS
        // ============================================================

        // Queue<T>
        private readonly Queue<object> _telemetryQueue = new();

        // PriorityQueue<TElement, TPriority>
        private readonly PriorityQueue<object, int> _criticalAlertQueue = new();

        // Dictionary<TKey, TValue>
        private readonly Dictionary<string, SensorRegistration>
            _sensorDictionary =
                new(StringComparer.OrdinalIgnoreCase);

        // SortedDictionary<TKey, TValue>
        private readonly SortedDictionary<DateTime, List<object>>
            _telemetryHistory = new();

        // HashSet<T>
        private readonly HashSet<string> _activeErrorStates =
            new(StringComparer.OrdinalIgnoreCase);

        // ============================================================
        // REAL-TIME PROCESSING
        // ============================================================

        private readonly RealTimeProcessingService
            _realTimeProcessingService;

        private const int OfflineThresholdSeconds = 15;

        private const double CriticalTelemetryThreshold = 40;

        // ============================================================
        // CONSTRUCTOR
        // ============================================================

        public TelemetryService(
            RealTimeProcessingService realTimeProcessingService)
        {
            _realTimeProcessingService =
                realTimeProcessingService;
        }

        // ============================================================
        // TELEMETRY INGESTION
        // ============================================================

        public void AddTelemetry<T>(
            TelemetryPacket<T> packet)
        {
            // Preserve existing Part 1 behaviour.
            _telemetry.Add(packet);

            // --------------------------------------------------------
            // QUEUE<T>
            // --------------------------------------------------------

            _telemetryQueue.Enqueue(packet);

            // --------------------------------------------------------
            // DICTIONARY<TKey,TValue>
            // --------------------------------------------------------

            if (_sensorDictionary.TryGetValue(
                packet.SensorId,
                out var registeredSensor))
            {
                registeredSensor.IsOnline = true;
                registeredSensor.LastSeen = DateTime.UtcNow;

                _activeErrorStates.Remove(
                    packet.SensorId);
            }
            else
            {
                var sensor =
                    _sensors.FirstOrDefault(
                        s => s.SensorId.Equals(
                            packet.SensorId,
                            StringComparison.OrdinalIgnoreCase));

                if (sensor != null)
                {
                    sensor.IsOnline = true;
                    sensor.LastSeen = DateTime.UtcNow;

                    _sensorDictionary[
                        packet.SensorId] = sensor;

                    _activeErrorStates.Remove(
                        packet.SensorId);
                }
            }

            // --------------------------------------------------------
            // SORTEDDICTIONARY
            // --------------------------------------------------------

            if (!_telemetryHistory.TryGetValue(
                packet.Timestamp,
                out var history))
            {
                history = new List<object>();

                _telemetryHistory[
                    packet.Timestamp] = history;
            }

            history.Add(packet);

            // --------------------------------------------------------
            // PRIORITYQUEUE
            // --------------------------------------------------------

            if (IsCriticalTelemetry(packet.Value))
            {
                _criticalAlertQueue.Enqueue(
                    packet,
                    1);
            }

            // --------------------------------------------------------
            // REAL-TIME PROCESSING SERVICE
            // --------------------------------------------------------

            // The processor receives the strongly typed packet and
            // places it into the appropriate processing collection.
            if (packet is TelemetryPacket<double> doublePacket)
            {
                _realTimeProcessingService.EnqueueTelemetry(
                    doublePacket);
            }
        }

        // ============================================================
        // EXISTING PART 1 TELEMETRY ACCESS
        // ============================================================

        public List<object> GetTelemetry()
        {
            return _telemetry;
        }

        // ============================================================
        // QUEUE ACCESS
        // ============================================================

        public int GetTelemetryQueueCount()
        {
            return _telemetryQueue.Count;
        }

        public object? DequeueTelemetry()
        {
            if (_telemetryQueue.Count == 0)
                return null;

            return _telemetryQueue.Dequeue();
        }

        // ============================================================
        // PRIORITY QUEUE ACCESS
        // ============================================================

        public int GetCriticalAlertCount()
        {
            return _criticalAlertQueue.Count;
        }

        public object? DequeueCriticalAlert()
        {
            if (_criticalAlertQueue.Count == 0)
                return null;

            return _criticalAlertQueue.Dequeue();
        }

        // ============================================================
        // SORTED TELEMETRY HISTORY
        // ============================================================

        public List<object> GetTelemetryHistory()
        {
            return _telemetryHistory
                .SelectMany(entry => entry.Value)
                .ToList();
        }

        // ============================================================
        // ACTIVE ERROR STATES
        // ============================================================

        public List<string> GetActiveErrorStates()
        {
            UpdateSensorStatuses();

            return _activeErrorStates.ToList();
        }

        // ============================================================
        // SENSOR REGISTRATION
        // ============================================================

        public void RegisterSensor(
            SensorRegistration sensor)
        {
            var existing =
                _sensors.FirstOrDefault(
                    s => s.SensorId.Equals(
                        sensor.SensorId,
                        StringComparison.OrdinalIgnoreCase));

            if (existing == null)
            {
                sensor.LastSeen =
                    DateTime.UtcNow;

                sensor.IsOnline = true;

                _sensors.Add(sensor);

                _sensorDictionary[
                    sensor.SensorId] = sensor;
            }
            else
            {
                existing.Location =
                    sensor.Location;

                existing.Category =
                    sensor.Category;

                existing.IsOnline = true;

                existing.LastSeen =
                    DateTime.UtcNow;

                _sensorDictionary[
                    existing.SensorId] = existing;
            }
        }

        public List<SensorRegistration> GetSensors()
        {
            UpdateSensorStatuses();

            return _sensors;
        }

        // ============================================================
        // FAST DICTIONARY LOOKUP
        // ============================================================

        public SensorRegistration? GetSensor(
            string sensorId)
        {
            UpdateSensorStatuses();

            if (_sensorDictionary.TryGetValue(
                sensorId,
                out var sensor))
            {
                return sensor;
            }

            return _sensors.FirstOrDefault(
                s => s.SensorId.Equals(
                    sensorId,
                    StringComparison.OrdinalIgnoreCase));
        }

        // ============================================================
        // SENSOR REMOVAL
        // ============================================================

        public bool RemoveSensor(
            string sensorId)
        {
            var sensor =
                GetSensor(sensorId);

            if (sensor == null)
                return false;

            _sensors.Remove(sensor);

            _sensorDictionary.Remove(
                sensorId);

            _activeErrorStates.Remove(
                sensorId);

            return true;
        }

        // ============================================================
        // STATUS MANAGEMENT
        // ============================================================

        private void UpdateSensorStatuses()
        {
            foreach (var sensor in _sensors)
            {
                var secondsSinceLastSeen =
                    (DateTime.UtcNow -
                     sensor.LastSeen).TotalSeconds;

                sensor.IsOnline =
                    secondsSinceLastSeen <=
                    OfflineThresholdSeconds;

                if (!sensor.IsOnline)
                {
                    _activeErrorStates.Add(
                        sensor.SensorId);
                }
                else
                {
                    _activeErrorStates.Remove(
                        sensor.SensorId);
                }
            }
        }

        // ============================================================
        // CRITICAL TELEMETRY DETECTION
        // ============================================================

        private bool IsCriticalTelemetry<T>(
            T value)
        {
            try
            {
                if (value == null)
                    return false;

                return Convert.ToDouble(value) >
                       CriticalTelemetryThreshold;
            }
            catch
            {
                return false;
            }
        }
    }
}