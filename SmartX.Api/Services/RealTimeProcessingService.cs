using SmartX.Api.Models;

namespace SmartX.Api.Services
{
    public class RealTimeProcessingService
    {
        // ============================================================
        // NORMAL TELEMETRY QUEUE
        // ============================================================

        private readonly Queue<TelemetryPacket<double>> _telemetryQueue = new();

        // ============================================================
        // CRITICAL ALERT PRIORITY QUEUE
        // ============================================================

        private readonly PriorityQueue<
            TelemetryPacket<double>,
            int> _criticalAlertQueue = new();

        // ============================================================
        // PROCESSED TELEMETRY
        // ============================================================

        private readonly List<TelemetryPacket<double>> _processedTelemetry =
            new();

        // ============================================================
        // PROCESSED CRITICAL ALERTS
        // ============================================================

        private readonly List<TelemetryPacket<double>> _processedAlerts =
            new();

        // ============================================================
        // THRESHOLD
        // ============================================================

        private const double CriticalThreshold = 40;

        // ============================================================
        // ADD TELEMETRY
        // ============================================================

        public void EnqueueTelemetry(
            TelemetryPacket<double> packet)
        {
            if (packet == null)
                return;

            // Critical telemetry receives priority.
            if (packet.Value > CriticalThreshold)
            {
                // Priority 1 = critical.
                _criticalAlertQueue.Enqueue(
                    packet,
                    1);
            }
            else
            {
                // Normal telemetry follows FIFO order.
                _telemetryQueue.Enqueue(packet);
            }
        }

        // ============================================================
        // PROCESS NEXT CRITICAL ALERT
        // ============================================================

        public TelemetryPacket<double>? ProcessNextCriticalAlert()
        {
            if (_criticalAlertQueue.Count == 0)
                return null;

            var packet =
                _criticalAlertQueue.Dequeue();

            _processedAlerts.Add(packet);

            return packet;
        }

        // ============================================================
        // PROCESS NEXT NORMAL TELEMETRY
        // ============================================================

        public TelemetryPacket<double>? ProcessNextTelemetry()
        {
            if (_telemetryQueue.Count == 0)
                return null;

            var packet =
                _telemetryQueue.Dequeue();

            _processedTelemetry.Add(packet);

            return packet;
        }

        // ============================================================
        // PROCESS ALL CRITICAL ALERTS FIRST
        // ============================================================

        public List<TelemetryPacket<double>>
            ProcessPendingTelemetry()
        {
            var processed =
                new List<TelemetryPacket<double>>();

            // Critical alerts always get processed first.
            while (_criticalAlertQueue.Count > 0)
            {
                var critical =
                    ProcessNextCriticalAlert();

                if (critical != null)
                {
                    processed.Add(critical);
                }
            }

            // Then process normal telemetry FIFO.
            while (_telemetryQueue.Count > 0)
            {
                var normal =
                    ProcessNextTelemetry();

                if (normal != null)
                {
                    processed.Add(normal);
                }
            }

            return processed;
        }

        // ============================================================
        // QUEUE COUNTS
        // ============================================================

        public int GetNormalQueueCount()
        {
            return _telemetryQueue.Count;
        }

        public int GetCriticalQueueCount()
        {
            return _criticalAlertQueue.Count;
        }

        // ============================================================
        // PROCESSED COUNTS
        // ============================================================

        public int GetProcessedTelemetryCount()
        {
            return _processedTelemetry.Count;
        }

        public int GetProcessedAlertCount()
        {
            return _processedAlerts.Count;
        }

        // ============================================================
        // PROCESSED DATA
        // ============================================================

        public List<TelemetryPacket<double>>
            GetProcessedTelemetry()
        {
            return _processedTelemetry.ToList();
        }

        public List<TelemetryPacket<double>>
            GetProcessedAlerts()
        {
            return _processedAlerts.ToList();
        }
    }
}