# PART 3 – PROOF OF EVIDENCE 

## 1. Part 3 Overview

Part 3 extends the Smart-X IoT Gateway application by introducing advanced data structures and algorithms for network topology management and historical telemetry processing.

The implementation focuses on two main components:

- **Network Topology Management:** Represents IoT devices and their wireless connections using graph data structures.
- **Historical Telemetry Management:** Uses an AVL tree to organise and retrieve historical sensor readings efficiently.

These enhancements build on the existing Smart-X architecture and extend its ability to monitor connected devices, analyse network relationships and retrieve historical telemetry information.

---

## 2. Network Topology Management

The network topology component represents the IoT environment as a weighted graph.

Each registered IoT device is represented as a vertex (node), while wireless connections between devices are represented as edges. Each connection has an associated energy cost.

### Implemented functionality

- Register IoT devices.
- Store device identifiers, MAC addresses, deployment locations and device types.
- Create wireless connections between registered devices.
- Assign energy costs to network connections.
- Display registered network nodes and connections.
- Perform Breadth-First Search (BFS).
- Perform Depth-First Search (DFS).
- Identify disconnected devices.
- Calculate a Minimum Spanning Tree (MST) using Prim's algorithm.

### Demonstration network

The following demonstration devices were registered:

| Device ID | MAC Address | Deployment Location | Device Type |
|---|---|---|---|
| ESP32-01 | AA:BB:CC:DD:EE:01 | Main Building | ESP32 |
| ESP32-02 | AA:BB:CC:DD:EE:02 | Laboratory | ESP32 |
| ESP32-03 | AA:BB:CC:DD:EE:03 | Server Room | ESP32 |

### Wireless connections

| Source | Destination | Energy Cost |
|---|---|---:|
| ESP32-01 | ESP32-02 | 5 |
| ESP32-01 | ESP32-03 | 10 |
| ESP32-02 | ESP32-03 | 3 |

### Graph algorithms

**Breadth-First Search (BFS)**

BFS traverses the network level by level from a selected starting device. It is used to explore connected devices and determine network reachability.

**Depth-First Search (DFS)**

DFS explores a path through the network as deeply as possible before backtracking. It provides an alternative method of traversing and analysing the network structure.

**Disconnected Node Detection**

This functionality identifies devices that are not connected to the network's connected component, helping highlight potentially isolated devices.

**Prim's Minimum Spanning Tree**

Prim's algorithm determines a minimum-cost set of connections that links all devices without creating cycles.

For the demonstration network, the algorithm selected:

- ESP32-01 → ESP32-02: Energy cost 5.
- ESP32-02 → ESP32-03: Energy cost 3.

**Total minimum energy cost: 8.00**

---

## 3. AVL Tree – Historical Telemetry

Part 3 introduces an AVL tree to manage historical numeric telemetry readings.

An AVL tree is a self-balancing Binary Search Tree (BST). It maintains balance through rotations during insertion, helping ensure efficient searching and retrieval as the number of readings increases.

### Implementation features

- Store telemetry readings according to their timestamps.
- Insert new numeric telemetry packets into the AVL tree.
- Maintain tree balance through AVL rotations.
- Support multiple telemetry packets with identical timestamps.
- Search for readings within a specified date and time range.
- Filter historical readings using a sensor ID.
- Count the total number of stored telemetry packets.

### Historical data retrieval

The historical telemetry service supports queries using:

- Start timestamp.
- End timestamp.
- Optional sensor ID.

This allows users to retrieve readings within a specific period and optionally restrict the results to a particular IoT device.

---

## 4. Historical Telemetry API Endpoints

The following endpoints were introduced through `HistoricalDataController`.

| HTTP Method | Endpoint | Description |
|---|---|---|
| GET | `/api/HistoricalData` | Retrieves historical telemetry within a specified date range. |
| GET | `/api/HistoricalData/count` | Returns the total number of stored historical readings. |

### Historical data query parameters

The `/api/HistoricalData` endpoint accepts:

- `startTime` – beginning of the requested period.
- `endTime` – end of the requested period.
- `sensorId` – optional sensor identifier used to filter the results.

### Example telemetry request

```json
{
  "sensorId": "ESP32-01",
  "location": "Main Building",
  "category": "Temperature",
  "value": 25.5,
  "timestamp": "2026-11-13T08:00:00Z"
}
```

This example demonstrates the structure of a numeric telemetry packet submitted to the application.

---

## 5. Frontend Integration

The Part 3 functionality is integrated into the existing Smart-X browser client.

The frontend provides access to the network topology and historical telemetry functionality through the application's interface.

Users can interact with the topology management features, register devices, establish wireless connections and access graph operations.

The historical telemetry functionality communicates with the backend API to retrieve stored readings and display relevant information.

The integration maintains the separation between the frontend presentation layer and backend processing services.

---

## 6. Architecture and Data Structures

Part 3 makes use of the following data structures and algorithms:

| Data Structure / Algorithm | Purpose |
|---|---|
| Graph | Represents IoT devices and wireless connections. |
| Vertices | Represent registered IoT devices. |
| Weighted Edges | Represent connections and their energy costs. |
| BFS | Traverses the network level by level. |
| DFS | Performs depth-first network traversal. |
| Prim's Algorithm | Calculates a minimum spanning tree. |
| AVL Tree | Stores historical telemetry readings by timestamp. |
| AVL Rotations | Maintain tree balance during insertion. |

These structures improve the organisation and processing of network and historical telemetry information.

---

## 7. Challenges and Lessons Learned

During Part 3 development, several important technical concepts were applied.

One of the main challenges was representing IoT devices and their wireless relationships using graph structures. Connections needed to reference valid devices, while their associated energy costs had to be considered during graph operations.

Another challenge involved implementing an AVL tree for historical telemetry. Unlike a standard Binary Search Tree, an AVL tree requires balancing operations to maintain efficient performance. Implementing rotations and supporting duplicate timestamps required careful handling of the stored telemetry packets.

The development process also reinforced the importance of maintaining separation between frontend components, API controllers and backend services.

---

## 8. Future Improvements and Scalability

Although the current implementation provides network topology management and historical telemetry functionality, several improvements could support larger IoT deployments.

Potential improvements include:

- Integrating a dedicated time-series database for long-term telemetry storage.
- Introducing MQTT messaging for more efficient device communication.
- Implementing background telemetry processing.
- Improving network monitoring and device-health reporting.
- Expanding automated testing and API integration testing.
- Introducing additional security controls and role-based access management.
- Optimising graph operations for larger IoT networks.

These improvements would help prepare Smart-X for more demanding real-world IoT environments.

---

## 9. Part 3 Conclusion

Part 3 extends the Smart-X IoT Gateway through the implementation of graph-based network topology management and AVL-tree-based historical telemetry processing.

The network topology component provides device registration, wireless connection management, graph traversal and minimum spanning tree functionality. The historical telemetry component introduces an AVL tree for timestamp-based storage and retrieval of numeric sensor readings.

Together, these enhancements improve the application's ability to represent IoT network relationships, analyse device connectivity and manage historical telemetry data.

The completed work establishes a foundation for further development, scalability and future enhancements to the Smart-X IoT Gateway.
