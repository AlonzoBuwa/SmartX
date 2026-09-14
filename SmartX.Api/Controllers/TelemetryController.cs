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

        public TelemetryController(TelemetryService telemetryService)
        {
            _telemetryService = telemetryService;
        }

        [HttpGet]
        public IActionResult GetTelemetry()
        {
            return Ok(_telemetryService.GetTelemetry());
        }

        [HttpPost]
        public IActionResult AddTelemetry(
            [FromBody] TelemetryPacket<double> packet)
        {
            if (string.IsNullOrWhiteSpace(packet.SensorId))
            {
                return BadRequest("Sensor ID is required.");
            }

            _telemetryService.AddTelemetry(packet);

            return Ok(new
            {
                message = "Telemetry received successfully.",
                packet
            });
        }
    }
}