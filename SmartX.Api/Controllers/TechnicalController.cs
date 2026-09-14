using Microsoft.AspNetCore.Mvc;
using SmartX.Api.Models;
using SmartX.Api.Services;

namespace SmartX.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TechnicalController : ControllerBase
    {
        private readonly DeploymentValidator _validator;
        private readonly TelemetryProcessor _processor;

        public TechnicalController(
            DeploymentValidator validator,
            TelemetryProcessor processor)
        {
            _validator = validator;
            _processor = processor;
        }

        [HttpGet("operator")]
        public IActionResult OperatorOverloadingDemo()
        {
            var meter1 = new SensorReading(120);
            var meter2 = new SensorReading(80);

            var total = meter1 + meter2;
            var difference = meter1 - meter2;

            return Ok(new
            {
                meter1 = meter1.Value,
                meter2 = meter2.Value,
                total = total.Value,
                difference = difference.Value
            });
        }

        [HttpGet("arrays")]
        public IActionResult ArraysAndListsDemo()
        {
            double[][] historicalData =
            {
                new double[] { 21.5, 22.1, 23.4 },
                new double[] { 24.2, 25.8, 26.3 },
                new double[] { 27.1, 28.4, 29.0 }
            };

            var processed =
                _processor.ProcessHistoricalData(historicalData);

            return Ok(new
            {
                message = "Jagged array processed into List<T>.",
                numberOfReadings = processed.Count,
                readings = processed
            });
        }

        [HttpGet("recursion")]
        public IActionResult RecursionDemo()
        {
            var deployment = new DeploymentNode
            {
                Name = "Facility A",
                IsConfigured = true,
                Children =
                {
                    new DeploymentNode
                    {
                        Name = "Zone 1",
                        IsConfigured = true,
                        Children =
                        {
                            new DeploymentNode
                            {
                                Name = "Sub-Zone B",
                                IsConfigured = true,
                                Children =
                                {
                                    new DeploymentNode
                                    {
                                        Name = "Node 03",
                                        IsConfigured = true
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var valid = _validator.ValidateNode(deployment);

            return Ok(new
            {
                deployment = deployment.Name,
                recursivelyValidated = true,
                valid
            });
        }
    }
}