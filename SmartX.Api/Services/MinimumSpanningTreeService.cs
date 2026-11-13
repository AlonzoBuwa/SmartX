using SmartX.Api.Models;

namespace SmartX.Api.Services
{
    public class MinimumSpanningTreeService
    {
        private readonly NetworkGraph _graph;

        public MinimumSpanningTreeService(NetworkGraph graph)
        {
            _graph = graph;
        }

        public List<NetworkEdge> CalculatePrimMST(string startSensorId)
        {
            var mst = new List<NetworkEdge>();

            var startNode = _graph.GetNode(startSensorId);

            if (startNode == null || !startNode.IsOnline)
                return mst;

            var visited = new HashSet<string>();

            var priorityQueue =
                new PriorityQueue<NetworkEdge, double>();

            visited.Add(startSensorId);

            AddAvailableEdges(startSensorId, visited, priorityQueue);

            while (priorityQueue.Count > 0)
            {
                var edge = priorityQueue.Dequeue();

                string nextNodeId =
                    visited.Contains(edge.SourceSensorId)
                        ? edge.DestinationSensorId
                        : edge.SourceSensorId;

                if (visited.Contains(nextNodeId))
                    continue;

                var nextNode = _graph.GetNode(nextNodeId);

                if (nextNode == null || !nextNode.IsOnline)
                    continue;

                mst.Add(edge);

                visited.Add(nextNodeId);

                AddAvailableEdges(
                    nextNodeId,
                    visited,
                    priorityQueue);
            }

            return mst;
        }

        private void AddAvailableEdges(
            string sensorId,
            HashSet<string> visited,
            PriorityQueue<NetworkEdge, double> priorityQueue)
        {
            var edges = _graph.GetAllEdges();

            foreach (var edge in edges)
            {
                if (!edge.IsActive)
                    continue;

                string neighbourId = string.Empty;

                if (edge.SourceSensorId == sensorId)
                {
                    neighbourId = edge.DestinationSensorId;
                }
                else if (edge.DestinationSensorId == sensorId)
                {
                    neighbourId = edge.SourceSensorId;
                }
                else
                {
                    continue;
                }

                var neighbour = _graph.GetNode(neighbourId);

                if (neighbour == null ||
                    !neighbour.IsOnline ||
                    visited.Contains(neighbourId))
                {
                    continue;
                }

                priorityQueue.Enqueue(edge, edge.EnergyCost);
            }
        }

        public double CalculateTotalEnergyCost(
            List<NetworkEdge> connections)
        {
            return connections.Sum(edge => edge.EnergyCost);
        }
    }
}