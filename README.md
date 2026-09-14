# Smart-X IoT Data Ingestion and Validation Gateway

## Overview

Smart-X is an IoT data ingestion and validation gateway developed using C# and .NET 8. The application allows sensor devices to be registered, telemetry data to be received and monitored, and configuration or hardware log files to be attached to sensor profiles.

The project focuses on the Sensor Data Ingestion and Telemetry architectural pillar of the Smart-X ecosystem.

## Features

- Sensor registration and management
- MAC address / unique sensor identification
- Sensor location and category selection
- REST API telemetry ingestion
- Generic `TelemetryPacket<T>` implementation
- Real-time telemetry dashboard
- Visual telemetry monitoring
- Anomaly detection
- Sensor disconnect detection
- Configuration, deployment and hardware log file attachments
- Operator overloading
- Jagged arrays and `List<T>` collections
- Recursive deployment validation
- Swagger / OpenAPI API documentation
- Frontend and backend API integration

## Technologies Used

- C#
- .NET 8
- ASP.NET Core Web API
- HTML5
- CSS3
- JavaScript
- Swagger / OpenAPI
- Git and GitHub

## Project Structure

```text
SmartX
│
├── SmartX.Api
│   │
│   ├── Controllers
│   │   ├── SensorsController.cs
│   │   ├── TelemetryController.cs
│   │   └── TechnicalController.cs
│   │
│   ├── Models
│   │   ├── SensorRegistration.cs
│   │   ├── SensorReading.cs
│   │   ├── TelemetryPacket.cs
│   │   └── DeploymentNode.cs
│   │
│   ├── Services
│   │   ├── TelemetryService.cs
│   │   ├── TelemetryProcessor.cs
│   │   └── DeploymentValidator.cs
│   │
│   ├── Uploads
│   │
│   ├── Program.cs
│   └── appsettings.json
│
├── SmartX.Client
│   │
│   └── wwwroot
│       └── index.html
│
└── README.md
How to Run
Clone or download the repository.
Open the SmartX solution in Visual Studio.
Make sure the required .NET 8 dependencies are installed.
Restore the project dependencies.
Set both SmartX.Api and SmartX.Client as startup projects.
Run the solution using Ctrl + F5.
The ASP.NET Core API will start together with the client dashboard.
Open the Smart-X dashboard in the browser.
The dashboard should display the API status as Online when the backend is running correctly.
Sensor Management

The dashboard allows users to register sensor devices using:

Device MAC address or unique identifier
Sensor deployment location
Sensor category

Example:

Device ID: AA:BB:CC:DD:EE:01
Location: Room A
Category: Environmental

Registered sensors are displayed on the dashboard together with their current online or offline status.

Telemetry Ingestion

Telemetry data is submitted through the ASP.NET Core Web API.

Example telemetry request:

{
  "sensorId": "AA:BB:CC:DD:EE:01",
  "location": "Room A",
  "category": "Environmental",
  "value": 25.5,
  "timestamp": "2026-09-14T10:00:00Z"
}

The dashboard retrieves telemetry data from the API and displays it using visual telemetry feedback.

Generic Telemetry Packets

The project uses a generic class called TelemetryPacket<T> to allow different sensor data types to be handled without unnecessary type conversion.

Examples include:

TelemetryPacket<float>
TelemetryPacket<int>
TelemetryPacket<bool>
TelemetryPacket<double>

This allows the ingestion layer to work with different types of sensor measurements.

Dashboard Engagement Strategy

The dashboard uses a combination of real-time visual telemetry and proactive alerts.

Telemetry values are presented visually so users can quickly identify changes in sensor behaviour. The system also identifies abnormal readings and displays alerts when a value exceeds the configured threshold.

This approach helps users identify and troubleshoot abnormal sensor behaviour without having to manually inspect large amounts of raw telemetry data.

Anomaly Detection

The dashboard checks incoming telemetry values for abnormal readings.

For example, environmental telemetry values above the configured threshold can generate an alert similar to:

ANOMALY DETECTED

Sensor: AA:BB:CC:DD:EE:01
Value: 45.8

This provides immediate visual feedback to the user.

Sensor Disconnect Detection

The system records the last time telemetry was received from a sensor.

If a sensor does not send telemetry within the configured timeout period, it is automatically marked as offline.

The dashboard then displays a disconnect alert containing information such as the sensor ID, location and last-seen time.

File Attachments

Users can attach configuration files, deployment photographs or hardware log files to a specific sensor profile.

The upload functionality uses a multipart file upload through the API.

Uploaded files are stored in the application's Uploads directory.

Advanced C# Concepts
Generics

Generics are implemented using:

TelemetryPacket<T>

This allows different telemetry data types to be processed using the same reusable structure.

Operator Overloading

The SensorReading class overloads arithmetic operators such as + and -.

Example:

SensorReading meter1 = new SensorReading(120);
SensorReading meter2 = new SensorReading(80);

SensorReading total = meter1 + meter2;

The resulting value is:

200

This demonstrates direct aggregation of sensor measurements.

Jagged Arrays and Lists

Historical telemetry is processed using jagged arrays:

double[][]

The processed values are then transferred into:

List<TelemetryPacket<double>>

This demonstrates the use of advanced arrays together with generic collections.

Recursion

The project includes a recursive deployment validation algorithm.

Deployment structures can contain multiple nested levels, for example:

Facility A
└── Zone 1
    └── Sub-Zone B
        └── Node 03

The recursive validator checks each node and its child nodes to ensure that the deployment structure has been configured correctly.

API Endpoints
Sensors
GET    /api/Sensors
POST   /api/Sensors
GET    /api/Sensors/{sensorId}
DELETE /api/Sensors/{sensorId}
POST   /api/Sensors/{sensorId}/files
Telemetry
GET  /api/Telemetry
POST /api/Telemetry
Technical Demonstrations
GET /api/Technical/operator
GET /api/Technical/arrays
GET /api/Technical/recursion
Swagger / OpenAPI

Swagger is enabled for testing and documenting the API.

When the API is running, the Swagger interface can be accessed through the local API Swagger URL.

Swagger can be used to test sensor registration, telemetry ingestion, file uploads and the technical demonstration endpoints.

Testing

The application should be tested by:

Starting both the API and client.
Confirming that the dashboard reports the API as Online.
Registering multiple sensors.
Sending telemetry through the API.
Confirming that telemetry appears on the dashboard.
Sending an abnormal telemetry value and confirming that an anomaly alert appears.
Allowing a sensor to stop sending telemetry and confirming that it becomes Offline.
Uploading a configuration or log file to a sensor.
Testing the operator overloading endpoint.
Testing the arrays and collections endpoint.
Testing the recursive validation endpoint.
Architecture

The Smart-X application follows a simple client-server architecture.

The frontend dashboard acts as the client and communicates with the ASP.NET Core Web API through HTTP requests.

The API contains controllers responsible for handling requests, models representing sensor and telemetry data, and services containing application logic.

Smart-X Client Dashboard
          │
          │ HTTP / JSON
          ▼
   ASP.NET Core Web API
          │
    ┌─────┴─────┐
    │           │
Controllers   Services
    │           │
    └─────┬─────┘
          │
        Models
          │
          ▼
 Sensor & Telemetry Data
Smart-X Architectural Pillars
1. Sensor Data Ingestion and Telemetry

Status: Active in Part 1

This part provides sensor registration, telemetry ingestion, visual monitoring, anomaly detection and sensor status monitoring.

2. Real-Time Command Stream and History

Status: Reserved for Part 2

This functionality is intentionally disabled in Part 1 and will be implemented in the next stage of the Smart-X application.

3. Network Topology and Mesh Routing

Status: Reserved for Final PoE

This functionality is intentionally disabled in Part 1 and will be implemented in the final stage of the project.

Repository

The complete source code for the Smart-X application is stored in GitHub.

The repository contains both the ASP.NET Core API and the client dashboard.

Project Status

Part 1 of the Smart-X project has been implemented with the focus on the Sensor Data Ingestion and Telemetry pillar.

The application provides a working API and client-facing dashboard with sensor registration, telemetry processing, visual feedback, anomaly detection, disconnect detection, file attachments and demonstrations of the required advanced C# concepts.

Future Development

Future versions of the application will extend the existing system with:

Real-time command streams
Command history
Network topology visualisation
Mesh routing
Additional IoT device management features
