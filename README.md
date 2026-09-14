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
