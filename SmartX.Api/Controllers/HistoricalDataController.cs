using Microsoft.AspNetCore.Mvc;
using SmartX.Api.Services;

namespace SmartX.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HistoricalDataController : ControllerBase
    {
        private readonly TelemetryService _telemetryService;

        public HistoricalDataController(
            TelemetryService telemetryService)
        {
            _telemetryService = telemetryService;
        }

        // ============================================================
        // PART 3 - SEARCH HISTORICAL TELEMETRY USING AVL TREE
        // ============================================================

        [HttpGet]
        public IActionResult GetHistoricalReadings(
            [FromQuery] DateTime startTime,
            [FromQuery] DateTime endTime,
            [FromQuery] string? sensorId = null)
        {
            if (startTime == default || endTime == default)
            {
                return BadRequest(new
                {
                    message = "Start time and end time are required."
                });
            }

            if (startTime > endTime)
            {
                return BadRequest(new
                {
                    message = "Start time cannot be after end time."
                });
            }

            var readings =
                _telemetryService.GetHistoricalReadings(
                    startTime,
                    endTime,
                    sensorId);

            return Ok(new
            {
                readings,
                totalReadings = readings.Count,
                sensorId
            });
        }

        // ============================================================
        // PART 3 - GET AVL TREE READING COUNT
        // ============================================================

        [HttpGet("count")]
        public IActionResult GetHistoricalReadingCount()
        {
            return Ok(new
            {
                totalReadings =
                    _telemetryService.GetHistoricalReadingCount()
            });
        }
    }
}