using Microsoft.AspNetCore.Mvc;
using SmartX.Api.Models;
using SmartX.Api.Services;

namespace SmartX.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommandStreamController : ControllerBase
    {
        private readonly CommandHistoryService _commandHistoryService;
        private readonly TelemetryService _telemetryService;

        public CommandStreamController(
            CommandHistoryService commandHistoryService,
            TelemetryService telemetryService)
        {
            _commandHistoryService = commandHistoryService;
            _telemetryService = telemetryService;
        }

        // ============================================================
        // GET COMMAND HISTORY
        // ============================================================

        [HttpGet("history")]
        public IActionResult GetCommandHistory()
        {
            return Ok(_commandHistoryService.GetHistory());
        }

        // ============================================================
        // GET LAST COMMAND
        // ============================================================

        [HttpGet("history/last")]
        public IActionResult GetLastCommand()
        {
            var command = _commandHistoryService.GetLastCommand();

            if (command == null)
            {
                return NotFound(new
                {
                    message = "No commands have been executed."
                });
            }

            return Ok(command);
        }

        // ============================================================
        // GET COMMAND COUNT
        // ============================================================

        [HttpGet("history/count")]
        public IActionResult GetCommandCount()
        {
            return Ok(new
            {
                count = _commandHistoryService.GetCommandCount()
            });
        }

        // ============================================================
        // EXECUTE DEVICE COMMAND
        // ============================================================

        [HttpPost("command")]
        public IActionResult ExecuteCommand(
            [FromBody] DeviceCommand command)
        {
            if (command == null)
            {
                return BadRequest(new
                {
                    message = "Command is required."
                });
            }

            if (string.IsNullOrWhiteSpace(command.SensorId))
            {
                return BadRequest(new
                {
                    message = "Sensor ID is required."
                });
            }

            if (string.IsNullOrWhiteSpace(command.Command))
            {
                return BadRequest(new
                {
                    message = "Command is required."
                });
            }

            // --------------------------------------------------------
            // FIND THE DEVICE
            // --------------------------------------------------------

            var sensor =
                _telemetryService.GetSensor(command.SensorId);

            if (sensor == null)
            {
                return NotFound(new
                {
                    message =
                        $"Sensor '{command.SensorId}' was not found."
                });
            }

            // --------------------------------------------------------
            // NORMALISE THE COMMAND
            // --------------------------------------------------------

            var requestedCommand =
                command.Command.Trim().ToUpperInvariant();

            if (requestedCommand != "ACTIVATE" &&
                requestedCommand != "DEACTIVATE")
            {
                return BadRequest(new
                {
                    message =
                        "Unsupported command. Use ACTIVATE or DEACTIVATE."
                });
            }

            // --------------------------------------------------------
            // CAPTURE THE CURRENT STATE BEFORE CHANGING IT
            // --------------------------------------------------------

            var previousState =
                sensor.DeviceState;

            // --------------------------------------------------------
            // DETERMINE THE NEW DEVICE STATE
            // --------------------------------------------------------

            var newState =
                requestedCommand == "ACTIVATE"
                    ? "ON"
                    : "OFF";

            // --------------------------------------------------------
            // PREVENT UNNECESSARY DUPLICATE STATE CHANGES
            // --------------------------------------------------------

            if (previousState == newState)
            {
                return BadRequest(new
                {
                    message =
                        $"Device '{sensor.SensorId}' is already {newState}."
                });
            }

            // --------------------------------------------------------
            // UPDATE ACTUAL DEVICE STATE
            // --------------------------------------------------------

            sensor.DeviceState = newState;

            // --------------------------------------------------------
            // CREATE THE COMMAND HISTORY RECORD
            // --------------------------------------------------------

            command.Command = requestedCommand;
            command.PreviousState = previousState;
            command.NewState = newState;
            command.Timestamp = DateTime.UtcNow;
            command.Status = "Executed";

            // --------------------------------------------------------
            // STORE COMMAND IN STACK
            // --------------------------------------------------------

            _commandHistoryService.AddCommand(command);

            return Ok(new
            {
                message =
                    $"{requestedCommand} command executed successfully.",
                command,
                device = new
                {
                    sensor.SensorId,
                    sensor.DeviceState,
                    sensor.IsOnline,
                    sensor.LastSeen
                }
            });
        }

        // ============================================================
        // UNDO LAST COMMAND
        // ============================================================

        [HttpPost("undo")]
        public IActionResult UndoLastCommand()
        {
            var command =
                _commandHistoryService.UndoLastCommand();

            if (command == null)
            {
                return NotFound(new
                {
                    message =
                        "There are no commands available to undo."
                });
            }

            // --------------------------------------------------------
            // FIND THE DEVICE
            // --------------------------------------------------------

            var sensor =
                _telemetryService.GetSensor(command.SensorId);

            if (sensor == null)
            {
                return NotFound(new
                {
                    message =
                        $"Sensor '{command.SensorId}' could not be found."
                });
            }

            // --------------------------------------------------------
            // RESTORE THE PREVIOUS STATE
            // --------------------------------------------------------

            sensor.DeviceState =
                command.PreviousState;

            command.Status = "Undone";

            return Ok(new
            {
                message =
                    "Last command was undone successfully.",
                undoneCommand = command,
                device = new
                {
                    sensor.SensorId,
                    sensor.DeviceState,
                    sensor.IsOnline,
                    sensor.LastSeen
                }
            });
        }

        // ============================================================
        // CLEAR COMMAND HISTORY
        // ============================================================

        [HttpDelete("history")]
        public IActionResult ClearCommandHistory()
        {
            _commandHistoryService.ClearHistory();

            return Ok(new
            {
                message =
                    "Command history cleared successfully."
            });
        }
    }
}