using SmartX.Api.Models;

namespace SmartX.Api.Services
{
    public class NetworkGraph
    {
        // Stores every network node using its unique sensor ID.
        private readonly Dictionary<string, NetworkNode> _nodes = new();

        // Stores the wireless connections belonging to each node.
        private readonly Dictionary<string, List<NetworkEdge>> _adjacencyList = new();

        // Adds a new sensor or gateway to the graph.
        public bool AddNode(NetworkNode node)
        {
            if (node == null ||
                string.IsNullOrWhiteSpace(node.SensorId))
            {
                return false;
            }

            if (_nodes.ContainsKey(node.SensorId))
            {
                return false;
            }

            _nodes.Add(node.SensorId, node);
            _adjacencyList.Add(node.SensorId, new List<NetworkEdge>());

            return true;
        }

        // Creates an undirected wireless connection between two nodes.
        public bool AddEdge(NetworkEdge edge)
        {
            if (edge == null ||
                string.IsNullOrWhiteSpace(edge.SourceSensorId) ||
                string.IsNullOrWhiteSpace(edge.DestinationSensorId))
            {
                return false;
            }

            if (!_nodes.ContainsKey(edge.SourceSensorId) ||
                !_nodes.ContainsKey(edge.DestinationSensorId))
            {
                return false;
            }

            if (edge.SourceSensorId == edge.DestinationSensorId ||
                edge.EnergyCost < 0 ||
                double.IsNaN(edge.EnergyCost) ||
                double.IsInfinity(edge.EnergyCost))
            {
                return false;
            }

            var existingConnection = _adjacencyList[edge.SourceSensorId]
                .Any(e =>
                    (e.SourceSensorId == edge.SourceSensorId &&
                     e.DestinationSensorId == edge.DestinationSensorId) ||
                    (e.SourceSensorId == edge.DestinationSensorId &&
                     e.DestinationSensorId == edge.SourceSensorId));

            if (existingConnection)
            {
                return false;
            }

            edge.IsActive = true;

            // Store the connection for both nodes because
            // this is an undirected graph.
            _adjacencyList[edge.SourceSensorId].Add(edge);
            _adjacencyList[edge.DestinationSensorId].Add(edge);

            return true;
        }

        // Returns all nodes in the network.
        public List<NetworkNode> GetAllNodes()
        {
            return _nodes.Values.ToList();
        }

        // Returns all unique wireless connections.
        public List<NetworkEdge> GetAllEdges()
        {
            return _adjacencyList.Values
                .SelectMany(edges => edges)
                .Distinct()
                .ToList();
        }

        // Retrieves a specific node using its sensor ID.
        public NetworkNode? GetNode(string sensorId)
        {
            _nodes.TryGetValue(sensorId, out var node);

            return node;
        }

        // Retrieves the direct neighbours of a selected node.
        public List<NetworkNode> GetNeighbours(string sensorId)
        {
            var neighbours = new List<NetworkNode>();

            if (!_adjacencyList.ContainsKey(sensorId))
            {
                return neighbours;
            }

            foreach (var edge in _adjacencyList[sensorId])
            {
                if (!edge.IsActive)
                {
                    continue;
                }

                var neighbourId =
                    edge.SourceSensorId == sensorId
                        ? edge.DestinationSensorId
                        : edge.SourceSensorId;

                if (_nodes.TryGetValue(neighbourId, out var neighbour))
                {
                    neighbours.Add(neighbour);
                }
            }

            return neighbours;
        }

        // Removes a wireless connection between two nodes.
        public bool RemoveEdge(string sourceId, string destinationId)
        {
            if (!_adjacencyList.ContainsKey(sourceId) ||
                !_adjacencyList.ContainsKey(destinationId))
            {
                return false;
            }

            var sourceEdges = _adjacencyList[sourceId];
            var destinationEdges = _adjacencyList[destinationId];

            var edge = sourceEdges.FirstOrDefault(e =>
                (e.SourceSensorId == sourceId &&
                 e.DestinationSensorId == destinationId) ||
                (e.SourceSensorId == destinationId &&
                 e.DestinationSensorId == sourceId));

            if (edge == null)
            {
                return false;
            }

            sourceEdges.Remove(edge);
            destinationEdges.Remove(edge);

            return true;
        }

        // Removes a node and all its wireless connections.
        public bool RemoveNode(string sensorId)
        {
            if (!_nodes.ContainsKey(sensorId))
            {
                return false;
            }

            var connections = _adjacencyList[sensorId].ToList();

            foreach (var edge in connections)
            {
                var neighbourId =
                    edge.SourceSensorId == sensorId
                        ? edge.DestinationSensorId
                        : edge.SourceSensorId;

                if (_adjacencyList.ContainsKey(neighbourId))
                {
                    _adjacencyList[neighbourId].Remove(edge);
                }
            }

            _adjacencyList.Remove(sensorId);
            _nodes.Remove(sensorId);

            return true;
        }

        // Returns the number of nodes in the graph.
        public int GetNodeCount()
        {
            return _nodes.Count;
        }

        // Returns the number of unique connections in the graph.
        public int GetEdgeCount()
        {
            return GetAllEdges().Count;
        }
    }
}