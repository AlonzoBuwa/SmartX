using Microsoft.AspNetCore.Mvc;
using SmartX.Api.Models;
using SmartX.Api.Services;

namespace SmartX.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SensorsController : ControllerBase
    {
        private readonly TelemetryService _telemetryService;

        public SensorsController(TelemetryService telemetryService)
        {
            _telemetryService = telemetryService;
        }

        [HttpGet]
        public IActionResult GetSensors()
        {
            return Ok(_telemetryService.GetSensors());
        }

        [HttpGet("{sensorId}")]
        public IActionResult GetSensor(string sensorId)
        {
            var sensor = _telemetryService.GetSensor(sensorId);

            if (sensor == null)
                return NotFound("Sensor not found.");

            return Ok(sensor);
        }

        [HttpPost]
        public IActionResult RegisterSensor(
            [FromBody] SensorRegistration sensor)
        {
            if (string.IsNullOrWhiteSpace(sensor.SensorId))
                return BadRequest("Sensor ID is required.");

            if (string.IsNullOrWhiteSpace(sensor.Location))
                return BadRequest("Sensor location is required.");

            if (string.IsNullOrWhiteSpace(sensor.Category))
                return BadRequest("Sensor category is required.");

            _telemetryService.RegisterSensor(sensor);

            return Ok(new
            {
                message = "Sensor registered successfully.",
                sensor
            });
        }

        [HttpDelete("{sensorId}")]
        public IActionResult DeleteSensor(string sensorId)
        {
            if (!_telemetryService.RemoveSensor(sensorId))
                return NotFound("Sensor not found.");

            return Ok(new
            {
                message = "Sensor deleted successfully."
            });
        }

        // Upload configuration files, deployment photos or hardware logs
        [HttpPost("{sensorId}/files")]
        public async Task<IActionResult> UploadFile(
            string sensorId,
            IFormFile file)
        {
            var sensor = _telemetryService.GetSensor(sensorId);

            if (sensor == null)
                return NotFound("Sensor not found.");

            if (file == null || file.Length == 0)
                return BadRequest("Please select a file.");

            var uploadFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Uploads");

            Directory.CreateDirectory(uploadFolder);

            var safeFileName = Path.GetFileName(file.FileName);

            var filePath = Path.Combine(
                uploadFolder,
                safeFileName);

            using (var stream = new FileStream(
                filePath,
                FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            sensor.Attachments.Add(safeFileName);

            return Ok(new
            {
                message = "File uploaded successfully.",
                fileName = safeFileName,
                sensorId
            });
        }
    }
}