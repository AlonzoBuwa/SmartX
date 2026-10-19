Smart-X IoT Gateway --- PROG7312 Part 2

1. Project Overview

Smart-X is an IoT gateway and management dashboard developed for the
PROG7312 Programming 3B / Advanced Application Development PoE.

Part 2 extends the Part 1 telemetry ingestion gateway into a real-time
processing and device-management system. The application provides:

Sensor telemetry ingestion

Real-time telemetry processing

FIFO telemetry queues

Priority-based critical alert processing

Live device registration and O(1) dictionary lookup

Timestamp-sorted historical telemetry

HashSet-based active error/disconnection tracking

Manual device commands

Command history and Undo functionality using a Stack

Device searching and filtering

Search-history tracking

Predictive Suggested Actions / Automated Insights

A navigation structure that reserves Network Topology and Mesh
Routing for the final PoE

2. Project Structure

The solution is separated into two main application layers:

Smart-X
├── SmartX.Api
│   ├── Controllers
│   ├── Models
│   └── Services
│
└── SmartX.Client
    └── index.html

Backend --- SmartX.Api

The ASP.NET Core Web API receives telemetry, manages devices, processes
commands, maintains the required data structures, and generates
recommendations.

Frontend --- SmartX.Client

The client is a browser-based dashboard that communicates with the API
asynchronously through HTTP requests.

3. Requirements

Install the following before running the project:

.NET 8 SDK

Visual Studio 2022 or another .NET 8 compatible IDE

A modern web browser such as Microsoft Edge, Google Chrome, or
Firefox

Git for version control

Check the installed .NET SDK:

dotnet --version

The project should be restored and built using the .NET 8 SDK configured
by the solution.

4. Running the Backend API

Option A --- Visual Studio

Open the Smart-X solution/project in Visual Studio.

Set SmartX.Api as the startup project.

Restore NuGet packages.

Build the solution.

Run the API using HTTPS.

Confirm that Swagger opens successfully.

The current client is configured to communicate with:

https://localhost:7178/api

If Visual Studio assigns a different HTTPS port, update the API_URL
value in SmartX.Client/index.html.

Option B --- Command Line

Open PowerShell in the SmartX.Api directory:

dotnet restore
dotnet build
dotnet run

Keep the API running while using the frontend.

5. Running the Frontend Client

The client is contained in:

SmartX.Client/index.html

The simplest option is to open the HTML file in a browser while the API
is running.

For a local development server, VS Code users can use an appropriate
local HTML server extension such as Live Server.

Make sure the API is running before using features that load devices,
telemetry, commands, processing statistics, or recommendations.

6. Application Navigation

The main navigation contains three architectural pillars:

Sensor Data Ingestion and Telemetry

Implemented in Part 1 and retained for Part 2.

Real-Time Command Stream and History

Implemented in Part 2.

This area provides:

Device searching/filtering

Queue and alert statistics

Telemetry processing

Manual device commands

Command history

Undo

Suggested Actions / Automated Insights

Network Topology and Mesh Routing

Reserved for the final PoE as required by the brief.

The navigation item is intentionally disabled until the final phase.

7. Part 2 Data Structures

The following data structures are explicitly implemented in the backend.

Data Structure                        Implementation                Purpose

Queue<T>                            TelemetryService /          FIFO handling of normal
RealTimeProcessingService   incoming telemetry

PriorityQueue<TElement,TPriority>   TelemetryService /          Prioritising critical
RealTimeProcessingService   telemetry alerts

Stack<T>                            CommandHistoryService       Stores manual command
history and supports
Undo

Dictionary<TKey,TValue>             TelemetryService            Live registered-device
lookup using the unique
sensor ID

SortedDictionary<TKey,TValue>       TelemetryService            Stores historical
telemetry sequentially
by timestamp

8. Testing the Queue<T>{=html}

The standard telemetry queue uses FIFO processing.

Step 1 --- Send normal telemetry

Use Swagger or another API client.

POST /api/Telemetry

Example:

{
  "sensorId": "TEMP-QUEUE-001",
  "location": "Test Room",
  "category": "Temperature",
  "value": 25,
  "timestamp": "2026-10-19T18:00:00Z"
}

Use a value at or below the critical threshold for normal telemetry.

Step 2 --- Check processing status

Use:

GET /api/Telemetry/processing/status

The normal queue count should increase.

Step 3 --- Process the next normal packet

Use:

POST /api/Telemetry/processing/normal

The queue count should decrease and the processed telemetry count should
increase.

This demonstrates FIFO queue processing.

9. Testing the PriorityQueue

Critical telemetry must bypass normal processing and enter the priority
alert queue.

Step 1 --- Send critical telemetry

POST /api/Telemetry

Example:

{
  "sensorId": "TEMP-CRITICAL-001",
  "location": "Server Room",
  "category": "Temperature",
  "value": 75,
  "timestamp": "2026-10-19T18:00:00Z"
}

The current implementation treats values above 40 as critical.

Step 2 --- Check processing status

GET /api/Telemetry/processing/status

Confirm that a critical alert is waiting.

Step 3 --- Process the critical alert

POST /api/Telemetry/processing/critical

Confirm that the critical alert is processed.

This demonstrates the PriorityQueue requirement.

10. Testing the Stack and Undo

Manual commands are stored using Stack<DeviceCommand>.

Step 1 --- Register a device

Register a sensor through the existing sensor registration
functionality.

Step 2 --- Activate the device

POST /api/CommandStream/command

Example:

{
  "sensorId": "TEMP-001",
  "command": "ACTIVATE"
}

Step 3 --- View command history

GET /api/CommandStream/history

The most recent command should appear first.

Step 4 --- Undo the command

POST /api/CommandStream/undo

The device should return to its previous state.

This demonstrates LIFO command history and the Undo operation.

11. Testing Dictionary Device Lookup

Registered devices are stored in a:

Dictionary<string, SensorRegistration>

The sensor's unique identifier is used as the dictionary key.

This allows direct lookup through:

GET /api/Telemetry/sensor/{sensorId}

The dictionary provides average O(1) lookup rather than requiring a
complete list scan.

12. Testing SortedDictionary Historical Logs

Historical telemetry is stored using:

SortedDictionary<DateTime, List<object>>

Telemetry records are grouped by timestamp and returned in chronological
order.

Use:

GET /api/Telemetry/history

Send multiple telemetry packets with different timestamps and confirm
that the returned history is ordered by timestamp.

13. Testing HashSet Error States

Disconnected/error states are tracked using:

HashSet<string>

This ensures that the same disconnected sensor is not repeatedly
inserted as a duplicate active error.

Test

Register a device.

Stop sending telemetry from that device.

Allow more than the configured offline threshold to pass.

Request:

GET /api/Telemetry/errors

The sensor should appear as an active error/disconnected state.

Request the endpoint again. The same sensor should remain a single entry
rather than appearing multiple times.

14. Testing Search and Filtering

Open:

Real-Time Command Stream & History

Use the device search field to search by:

Sensor ID

Location

Category

Operational information

The frontend filters the visible devices without requiring a page
reload.

Search activity is also recorded through:

POST /api/SearchHistory

The search history can be inspected with:

GET /api/SearchHistory

15. Testing Predictive Suggested Actions

The predictive recommendation engine analyses:

Device status

Critical telemetry

Repeated manual commands

Repeated user searches

Relationships between searches and critical sensor activity

The results are displayed in the:

Suggested Actions / Automated Insights

section of the Command Stream dashboard.

Recommended demonstration

Step 1 --- Send critical telemetry

{
  "sensorId": "TEMP-001",
  "location": "Server Room",
  "category": "Temperature",
  "value": 75,
  "timestamp": "2026-10-19T18:00:00Z"
}

Step 2 --- Open Command Stream & History

Search for:

TEMP-001

Step 3 --- Repeat the search

Search for TEMP-001 a second time.

Expected result

The Suggested Actions section should identify the repeated search
pattern and the critical telemetry associated with the same device.

A high-priority recommendation similar to:

Investigate TEMP-001 before issuing another command.

should be generated.

The reason should indicate that the device was repeatedly searched and
has critical telemetry.

This demonstrates proactive analysis rather than waiting for the user to
manually request a recommendation.

16. Important API Endpoints

Endpoint                                       Method                  Purpose

/api/Telemetry                               GET/POST                Retrieve or submit
telemetry

/api/Telemetry/queue/count                   GET                     View telemetry queue
size

/api/Telemetry/queue/next                    GET                     Remove/read next FIFO
telemetry

/api/Telemetry/alerts/count                  GET                     View critical alert
queue size

/api/Telemetry/alerts/next                   GET                     Remove/read next
critical alert

/api/Telemetry/history                       GET                     Retrieve
timestamp-sorted
history

/api/Telemetry/errors                        GET                     Retrieve active error
states

/api/Telemetry/processing/status             GET                     View processing queue
statistics

/api/Telemetry/processing/critical           POST                    Process next critical
alert

/api/Telemetry/processing/normal             POST                    Process next normal
telemetry item

/api/Telemetry/processing/process-all        POST                    Process pending
telemetry

/api/Telemetry/processing/processed          GET                     View processed normal
telemetry

/api/Telemetry/processing/processed-alerts   GET                     View processed critical
alerts

/api/CommandStream/history                   GET                     View command history

/api/CommandStream/history/last              GET                     View latest command

/api/CommandStream/history/count             GET                     View command count

/api/CommandStream/command                   POST                    Execute device command

/api/CommandStream/undo                      POST                    Undo latest command

/api/SearchHistory                           GET/POST/DELETE         Manage search history

17. Manual Override Commands

Supported device commands are:

ACTIVATE
DEACTIVATE

The command system records:

Command ID

Sensor ID

Command

Previous state

New state

Timestamp

Execution status

This information is used by the command-history Stack and Undo feature.

18. Dependency Injection

The backend registers the major services as singleton services so that
their in-memory collections remain available throughout the running API
session.

The main registered services include:

TelemetryService
DeploymentValidator
TelemetryProcessor
CommandHistoryService
SearchHistoryService
RecommendationService
RealTimeProcessingService

19. Troubleshooting

Swagger does not open

Confirm that SmartX.Api is running and check the HTTPS URL shown by
Visual Studio.

Frontend cannot retrieve API data

Check that:

The backend is running.

The frontend API_URL matches the backend HTTPS address.

The browser is allowed to access the API.

The API has not stopped because of a build/runtime exception.

No recommendations appear

Confirm that:

The API is running.

The /api/Recommendation endpoint responds.

A device exists.

Search history has been generated.

Critical telemetry has been submitted if testing the high-priority
recommendation.

Data disappears after restarting the API

The Part 2 queues, command history, search history, device state, and
recommendations use in-memory services. Restarting the API clears the
in-memory runtime state. Persistent storage can be introduced in a later
project phase if required.

20. Demonstration / Evidence Checklist

Recommended screenshots for the submission:

Main Smart-X navigation menu

Sensor Telemetry dashboard

Real-Time Command Stream & History dashboard

Queue processing statistics

Critical alert / PriorityQueue processing

Command history

Successful Undo

Device search/filter

Active disconnected/error state

Timestamp-sorted telemetry history

Suggested Actions / Automated Insights

Swagger API endpoints

Successful build/run

GitHub repository and commit history

21. GitHub Submission Checklist

Before submission:

Complete source code is committed.

Backend API builds successfully.

Frontend loads successfully.

API and frontend communicate correctly.

README is included.

Lecturer feedback update file is included.

Required screenshots/evidence are captured.

GitHub repository contains the latest code.

Required commit history is present.

YouTube demonstration/presentation is prepared.

The Part 2 brief requires the source code to be stored on GitHub and
specifies that each project should have over 20 commits.

22. Part 2 Implementation Summary

Part 2 transforms the original Smart-X ingestion gateway into a
real-time device-management system.

The main improvements are:

Real-time command stream and history navigation.

FIFO telemetry processing using Queue<T>.

Critical alert processing using PriorityQueue<TElement,TPriority>.

Command history and Undo using Stack<T>.

O(1) live device lookup using Dictionary<TKey,TValue>.

Timestamp-ordered historical telemetry using
SortedDictionary<TKey,TValue>.

Unique active error tracking using HashSet<T>.

Search and operational filtering.

Search-history tracking.

Predictive Suggested Actions based on repeated searches, commands,
device state, and critical telemetry.

Asynchronous frontend/API communication.

23. Final Note

Part 2 builds on the Part 1 Smart-X application. The Network Topology
and Mesh Routing module is intentionally reserved for the final PoE and
should not be treated as a completed Part 2 feature.