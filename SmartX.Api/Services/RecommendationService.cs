using SmartX.Api.Models;

namespace SmartX.Api.Services
{
    public class RecommendationService
    {
        private readonly TelemetryService _telemetryService;
        private readonly CommandHistoryService _commandHistoryService;
        private readonly SearchHistoryService _searchHistoryService;

        private const double CriticalThreshold = 40;
        private const int RepeatedSearchThreshold = 2;

        public RecommendationService(
            TelemetryService telemetryService,
            CommandHistoryService commandHistoryService,
            SearchHistoryService searchHistoryService)
        {
            _telemetryService = telemetryService;
            _commandHistoryService = commandHistoryService;
            _searchHistoryService = searchHistoryService;
        }

        public List<SuggestedAction> GenerateRecommendations()
        {
            var recommendations = new List<SuggestedAction>();

            var sensors = _telemetryService.GetSensors();
            var telemetry = _telemetryService.GetTelemetry();
            var commandHistory = _commandHistoryService.GetHistory();
            var searchHistory = _searchHistoryService.GetHistory();

            // ============================================================
            // 1. OFFLINE DEVICE DETECTION
            // ============================================================

            foreach (var sensor in sensors)
            {
                if (!sensor.IsOnline)
                {
                    recommendations.Add(new SuggestedAction
                    {
                        SensorId = sensor.SensorId,
                        Category = sensor.Category,
                        Recommendation = "Check the device connection.",
                        Reason = "The sensor has stopped sending telemetry.",
                        Priority = "High",
                        GeneratedAt = DateTime.UtcNow
                    });
                }
            }

            // ============================================================
            // 2. REPEATED COMMAND PATTERN ANALYSIS
            // ============================================================

            var repeatedCommands =
                commandHistory
                    .GroupBy(command => new
                    {
                        command.SensorId,
                        command.Command
                    })
                    .Where(group =>
                        group.Count() >= RepeatedSearchThreshold);

            foreach (var group in repeatedCommands)
            {
                var sensor =
                    sensors.FirstOrDefault(item =>
                        item.SensorId.Equals(
                            group.Key.SensorId,
                            StringComparison.OrdinalIgnoreCase));

                recommendations.Add(new SuggestedAction
                {
                    SensorId = group.Key.SensorId,
                    Category = sensor?.Category ?? "Unknown",
                    Recommendation =
                        $"Review repeated '{group.Key.Command}' commands.",
                    Reason =
                        $"The same command has been executed {group.Count()} times for this device.",
                    Priority = "Normal",
                    GeneratedAt = DateTime.UtcNow
                });
            }

            // ============================================================
            // 3. REPEATED SEARCH PATTERN ANALYSIS
            // ============================================================

            var repeatedSearches =
                searchHistory
                    .GroupBy(
                        item => item.SearchTerm,
                        StringComparer.OrdinalIgnoreCase)
                    .Where(group =>
                        group.Count() >= RepeatedSearchThreshold);

            foreach (var searchGroup in repeatedSearches)
            {
                var searchTerm = searchGroup.Key;

                var matchingSensors =
                    sensors
                        .Where(sensor =>
                            sensor.SensorId.Contains(
                                searchTerm,
                                StringComparison.OrdinalIgnoreCase)
                            ||
                            sensor.Location.Contains(
                                searchTerm,
                                StringComparison.OrdinalIgnoreCase)
                            ||
                            sensor.Category.Contains(
                                searchTerm,
                                StringComparison.OrdinalIgnoreCase))
                        .ToList();

                // --------------------------------------------------------
                // 3A. Search + critical telemetry correlation
                // --------------------------------------------------------

                foreach (var sensor in matchingSensors)
                {
                    var recentCriticalTelemetry =
                        telemetry
                            .Where(item =>
                            {
                                if (item is not TelemetryPacket<double> packet)
                                    return false;

                                return packet.SensorId.Equals(
                                           sensor.SensorId,
                                           StringComparison.OrdinalIgnoreCase)
                                       &&
                                       packet.Value > CriticalThreshold;
                            })
                            .ToList();

                    if (recentCriticalTelemetry.Count > 0)
                    {
                        recommendations.Add(new SuggestedAction
                        {
                            SensorId = sensor.SensorId,
                            Category = sensor.Category,
                            Recommendation =
                                $"Investigate {sensor.SensorId} before issuing another command.",
                            Reason =
                                $"This device has been searched {searchGroup.Count()} times and has recorded {recentCriticalTelemetry.Count} critical telemetry reading(s).",
                            Priority = "High",
                            GeneratedAt = DateTime.UtcNow
                        });
                    }
                    else
                    {
                        recommendations.Add(new SuggestedAction
                        {
                            SensorId = sensor.SensorId,
                            Category = sensor.Category,
                            Recommendation =
                                $"Review the activity of {sensor.SensorId}.",
                            Reason =
                                $"The device or category has been searched {searchGroup.Count()} times.",
                            Priority = "Normal",
                            GeneratedAt = DateTime.UtcNow
                        });
                    }
                }

                // --------------------------------------------------------
                // 3B. Search term doesn't directly match a device
                // --------------------------------------------------------

                if (matchingSensors.Count == 0)
                {
                    recommendations.Add(new SuggestedAction
                    {
                        SensorId = "SYSTEM",
                        Category = "Search Activity",
                        Recommendation =
                            $"Review devices related to '{searchTerm}'.",
                        Reason =
                            $"The search term has been used {searchGroup.Count()} times, indicating repeated operator interest.",
                        Priority = "Normal",
                        GeneratedAt = DateTime.UtcNow
                    });
                }
            }

            // ============================================================
            // 4. CRITICAL ALERT ANALYSIS
            // ============================================================

            var criticalCount =
                _telemetryService.GetCriticalAlertCount();

            if (criticalCount > 0)
            {
                recommendations.Add(new SuggestedAction
                {
                    SensorId = "SYSTEM",
                    Category = "Alert",
                    Recommendation =
                        "Review the critical telemetry alerts.",
                    Reason =
                        $"{criticalCount} critical alert(s) are waiting in the priority queue.",
                    Priority = "High",
                    GeneratedAt = DateTime.UtcNow
                });
            }

            // ============================================================
            // 5. DEFAULT SYSTEM INSIGHT
            // ============================================================

            if (recommendations.Count == 0)
            {
                recommendations.Add(new SuggestedAction
                {
                    SensorId = "SYSTEM",
                    Category = "Monitoring",
                    Recommendation =
                        "Continue monitoring device activity.",
                    Reason =
                        "No significant activity patterns currently require action.",
                    Priority = "Normal",
                    GeneratedAt = DateTime.UtcNow
                });
            }

            // Remove duplicate recommendations
            return recommendations
                .GroupBy(item => new
                {
                    item.SensorId,
                    item.Recommendation,
                    item.Reason
                })
                .Select(group => group.First())
                .ToList();
        }
    }
}