using SmartX.Api.Models;

namespace SmartX.Api.Services
{
    public class GraphTraversalService
    {
        private readonly NetworkGraph _graph;

        public GraphTraversalService(NetworkGraph graph)
        {
            _graph = graph;
        }

        // Breadth-First Search
        // Explores neighbouring devices level by level.
        public List<NetworkNode> BreadthFirstSearch(string startSensorId)
        {
            var visited = new HashSet<string>();
            var result = new List<NetworkNode>();
            var queue = new Queue<string>();

            var startNode = _graph.GetNode(startSensorId);

            if (startNode == null || !startNode.IsOnline)
                return result;

            visited.Add(startSensorId);
            queue.Enqueue(startSensorId);

            while (queue.Count > 0)
            {
                string currentId = queue.Dequeue();

                var currentNode = _graph.GetNode(currentId);

                if (currentNode == null)
                    continue;

                result.Add(currentNode);

                foreach (var neighbour in _graph.GetNeighbours(currentId))
                {
                    if (neighbour.IsOnline &&
                        visited.Add(neighbour.SensorId))
                    {
                        queue.Enqueue(neighbour.SensorId);
                    }
                }
            }

            return result;
        }

        // Depth-First Search
        // Explores one connection path before backtracking.
        public List<NetworkNode> DepthFirstSearch(string startSensorId)
        {
            var visited = new HashSet<string>();
            var result = new List<NetworkNode>();
            var stack = new Stack<string>();

            var startNode = _graph.GetNode(startSensorId);

            if (startNode == null || !startNode.IsOnline)
                return result;

            stack.Push(startSensorId);

            while (stack.Count > 0)
            {
                string currentId = stack.Pop();

                if (!visited.Add(currentId))
                    continue;

                var currentNode = _graph.GetNode(currentId);

                if (currentNode == null || !currentNode.IsOnline)
                    continue;

                result.Add(currentNode);

                var neighbours = _graph.GetNeighbours(currentId);

                // Reverse the order to make traversal predictable.
                for (int i = neighbours.Count - 1; i >= 0; i--)
                {
                    var neighbour = neighbours[i];

                    if (neighbour.IsOnline &&
                        !visited.Contains(neighbour.SensorId))
                    {
                        stack.Push(neighbour.SensorId);
                    }
                }
            }

            return result;
        }

        // Identifies devices that cannot be reached from a gateway.
        public List<NetworkNode> GetDisconnectedNodes(string gatewaySensorId)
        {
            var reachableNodes = BreadthFirstSearch(gatewaySensorId);

            var reachableIds = new HashSet<string>(
                reachableNodes.Select(node => node.SensorId));

            return _graph.GetAllNodes()
                .Where(node =>
                    !node.IsOnline ||
                    !reachableIds.Contains(node.SensorId))
                .ToList();
        }
    }
}