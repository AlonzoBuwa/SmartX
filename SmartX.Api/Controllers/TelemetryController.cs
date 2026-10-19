using Microsoft.AspNetCore.Mvc;
using SmartX.Api.Models;
using SmartX.Api.Services;

namespace SmartX.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TelemetryController : ControllerBase
    {
        private readonly TelemetryService _telemetryService;
        private readonly RealTimeProcessingService _realTimeProcessingService;

        public TelemetryController(
            TelemetryService telemetryService,
            RealTimeProcessingService realTimeProcessingService)
        {
            _telemetryService = telemetryService;
            _realTimeProcessingService = realTimeProcessingService;
        }

        // ============================================================
        // EXISTING PART 1 - GET TELEMETRY
        // ============================================================

        [HttpGet]
        public IActionResult GetTelemetry()
        {
            return Ok(_telemetryService.GetTelemetry());
        }

        // ============================================================
        // EXISTING PART 1 - POST TELEMETRY
        // ============================================================

        [HttpPost]
        public IActionResult AddTelemetry(
            [FromBody] TelemetryPacket<double> packet)
        {
            if (packet == null)
            {
                return BadRequest(new
                {
                    message = "Telemetry packet is required."
                });
            }

            if (string.IsNullOrWhiteSpace(packet.SensorId))
            {
                return BadRequest(new
                {
                    message = "Sensor ID is required."
                });
            }

            _telemetryService.AddTelemetry(packet);

            return Ok(new
            {
                message = "Telemetry received successfully.",
                packet
            });
        }

        // ============================================================
        // PART 2 - TELEMETRY QUEUE
        // ============================================================

        [HttpGet("queue/count")]
        public IActionResult GetTelemetryQueueCount()
        {
            return Ok(new
            {
                count =
                    _telemetryService.GetTelemetryQueueCount()
            });
        }

        [HttpGet("queue/next")]
        public IActionResult DequeueTelemetry()
        {
            var telemetry =
                _telemetryService.DequeueTelemetry();

            if (telemetry == null)
            {
                return NotFound(new
                {
                    message =
                        "The telemetry queue is empty."
                });
            }

            return Ok(telemetry);
        }

        // ============================================================
        // PART 2 - CRITICAL PRIORITY QUEUE
        // ============================================================

        [HttpGet("alerts/count")]
        public IActionResult GetCriticalAlertCount()
        {
            return Ok(new
            {
                count =
                    _telemetryService.GetCriticalAlertCount()
            });
        }

        [HttpGet("alerts/next")]
        public IActionResult DequeueCriticalAlert()
        {
            var alert =
                _telemetryService.DequeueCriticalAlert();

            if (alert == null)
            {
                return NotFound(new
                {
                    message =
                        "There are no critical alerts."
                });
            }

            return Ok(alert);
        }

        // ============================================================
        // PART 2 - SORTED TELEMETRY HISTORY
        // ============================================================

        [HttpGet("history")]
        public IActionResult GetTelemetryHistory()
        {
            return Ok(
                _telemetryService.GetTelemetryHistory()
            );
        }

        // ============================================================
        // PART 2 - ACTIVE ERROR STATES
        // ============================================================

        [HttpGet("errors")]
        public IActionResult GetActiveErrorStates()
        {
            return Ok(
                _telemetryService.GetActiveErrorStates()
            );
        }

        // ============================================================
        // PART 2 - REAL-TIME PROCESSING QUEUES
        // ============================================================

        [HttpGet("processing/status")]
        public IActionResult GetProcessingStatus()
        {
            return Ok(new
            {
                normalQueue =
                    _realTimeProcessingService
                        .GetNormalQueueCount(),

                criticalQueue =
                    _realTimeProcessingService
                        .GetCriticalQueueCount(),

                processedTelemetry =
                    _realTimeProcessingService
                        .GetProcessedTelemetryCount(),

                processedAlerts =
                    _realTimeProcessingService
                        .GetProcessedAlertCount()
            });
        }

        // ============================================================
        // PROCESS NEXT CRITICAL ALERT
        // ============================================================

        [HttpPost("processing/critical")]
        public IActionResult ProcessCriticalAlert()
        {
            var alert =
                _realTimeProcessingService
                    .ProcessNextCriticalAlert();

            if (alert == null)
            {
                return NotFound(new
                {
                    message =
                        "No critical alerts are waiting."
                });
            }

            return Ok(new
            {
                message =
                    "Critical alert processed.",
                alert
            });
        }

        // ============================================================
        // PROCESS NEXT NORMAL TELEMETRY
        // ============================================================

        [HttpPost("processing/normal")]
        public IActionResult ProcessNormalTelemetry()
        {
            var telemetry =
                _realTimeProcessingService
                    .ProcessNextTelemetry();

            if (telemetry == null)
            {
                return NotFound(new
                {
                    message =
                        "No normal telemetry is waiting."
                });
            }

            return Ok(new
            {
                message =
                    "Normal telemetry processed.",
                telemetry
            });
        }

        // ============================================================
        // PROCESS ALL PENDING TELEMETRY
        // ============================================================

        [HttpPost("processing/process-all")]
        public IActionResult ProcessAllTelemetry()
        {
            var processed =
                _realTimeProcessingService
                    .ProcessPendingTelemetry();

            return Ok(new
            {
                message =
                    "Pending telemetry processed.",
                count = processed.Count,
                processed
            });
        }

        // ============================================================
        // PROCESSED TELEMETRY
        // ============================================================

        [HttpGet("processing/processed")]
        public IActionResult GetProcessedTelemetry()
        {
            return Ok(
                _realTimeProcessingService
                    .GetProcessedTelemetry()
            );
        }

        // ============================================================
        // PROCESSED CRITICAL ALERTS
        // ============================================================

        [HttpGet("processing/processed-alerts")]
        public IActionResult GetProcessedAlerts()
        {
            return Ok(
                _realTimeProcessingService
                    .GetProcessedAlerts()
            );
        }
    }
}