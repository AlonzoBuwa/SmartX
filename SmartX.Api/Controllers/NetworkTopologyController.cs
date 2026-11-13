using Microsoft.AspNetCore.Mvc;
using SmartX.Api.Models;
using SmartX.Api.Services;

namespace SmartX.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NetworkTopologyController : ControllerBase
    {
        private readonly NetworkGraph _graph;
        private readonly GraphTraversalService _traversalService;
        private readonly MinimumSpanningTreeService _mstService;

        public NetworkTopologyController(
            NetworkGraph graph,
            GraphTraversalService traversalService,
            MinimumSpanningTreeService mstService)
        {
            _graph = graph;
            _traversalService = traversalService;
            _mstService = mstService;
        }

        // GET: api/NetworkTopology
        [HttpGet]
        public IActionResult GetNetwork()
        {
            return Ok(new
            {
                totalNodes = _graph.GetNodeCount(),
                totalConnections = _graph.GetEdgeCount(),
                nodes = _graph.GetAllNodes(),
                connections = _graph.GetAllEdges()
            });
        }

        // GET: api/NetworkTopology/nodes
        [HttpGet("nodes")]
        public IActionResult GetNodes()
        {
            return Ok(_graph.GetAllNodes());
        }

        // POST: api/NetworkTopology/nodes
        [HttpPost("nodes")]
        public IActionResult AddNode([FromBody] NetworkNode node)
        {
            if (node == null ||
                string.IsNullOrWhiteSpace(node.SensorId))
            {
                return BadRequest("A valid sensor ID is required.");
            }

            if (string.IsNullOrWhiteSpace(node.MacAddress))
            {
                return BadRequest("A MAC address is required.");
            }

            if (!_graph.AddNode(node))
            {
                return Conflict("Node already exists or is invalid.");
            }

            return Ok(new
            {
                message = "Network node added successfully.",
                node
            });
        }

        // GET: api/NetworkTopology/nodes/{sensorId}
        [HttpGet("nodes/{sensorId}")]
        public IActionResult GetNode(string sensorId)
        {
            var node = _graph.GetNode(sensorId);

            if (node == null)
                return NotFound("Network node not found.");

            return Ok(node);
        }

        // DELETE: api/NetworkTopology/nodes/{sensorId}
        [HttpDelete("nodes/{sensorId}")]
        public IActionResult DeleteNode(string sensorId)
        {
            if (!_graph.RemoveNode(sensorId))
                return NotFound("Network node not found.");

            return Ok(new
            {
                message = "Network node removed successfully."
            });
        }

        // POST: api/NetworkTopology/connections
        [HttpPost("connections")]
        public IActionResult AddConnection(
            [FromBody] NetworkEdge edge)
        {
            if (!_graph.AddEdge(edge))
            {
                return BadRequest(
                    "Connection could not be created. Check the sensor IDs and energy cost.");
            }

            return Ok(new
            {
                message = "Network connection created successfully.",
                connection = edge
            });
        }

        // DELETE: api/NetworkTopology/connections
        [HttpDelete("connections")]
        public IActionResult DeleteConnection(
            [FromQuery] string sourceId,
            [FromQuery] string destinationId)
        {
            if (!_graph.RemoveEdge(sourceId, destinationId))
                return NotFound("Network connection not found.");

            return Ok(new
            {
                message = "Network connection removed successfully."
            });
        }

        // GET: api/NetworkTopology/neighbours/{sensorId}
        [HttpGet("neighbours/{sensorId}")]
        public IActionResult GetNeighbours(string sensorId)
        {
            if (_graph.GetNode(sensorId) == null)
                return NotFound("Sensor not found.");

            return Ok(_graph.GetNeighbours(sensorId));
        }

        // GET: api/NetworkTopology/bfs/{sensorId}
        [HttpGet("bfs/{sensorId}")]
        public IActionResult BreadthFirstSearch(string sensorId)
        {
            if (_graph.GetNode(sensorId) == null)
                return NotFound("Starting sensor not found.");

            var result =
                _traversalService.BreadthFirstSearch(sensorId);

            return Ok(new
            {
                algorithm = "Breadth-First Search",
                startingNode = sensorId,
                visitedNodes = result.Count,
                traversal = result
            });
        }

        // GET: api/NetworkTopology/dfs/{sensorId}
        [HttpGet("dfs/{sensorId}")]
        public IActionResult DepthFirstSearch(string sensorId)
        {
            if (_graph.GetNode(sensorId) == null)
                return NotFound("Starting sensor not found.");

            var result =
                _traversalService.DepthFirstSearch(sensorId);

            return Ok(new
            {
                algorithm = "Depth-First Search",
                startingNode = sensorId,
                visitedNodes = result.Count,
                traversal = result
            });
        }

        // GET: api/NetworkTopology/disconnected/{sensorId}
        [HttpGet("disconnected/{sensorId}")]
        public IActionResult GetDisconnectedNodes(string sensorId)
        {
            if (_graph.GetNode(sensorId) == null)
                return NotFound("Gateway not found.");

            var disconnected =
                _traversalService.GetDisconnectedNodes(sensorId);

            return Ok(new
            {
                gateway = sensorId,
                disconnectedCount = disconnected.Count,
                disconnectedNodes = disconnected
            });
        }

        // GET: api/NetworkTopology/mst/{sensorId}
        [HttpGet("mst/{sensorId}")]
        public IActionResult CalculateMST(string sensorId)
        {
            if (_graph.GetNode(sensorId) == null)
                return NotFound("Starting sensor not found.");

            var connections =
                _mstService.CalculatePrimMST(sensorId);

            var totalEnergy =
                _mstService.CalculateTotalEnergyCost(connections);

            return Ok(new
            {
                algorithm = "Prim's Minimum Spanning Tree",
                startingNode = sensorId,
                connectionCount = connections.Count,
                totalEnergyCost = totalEnergy,
                connections
            });
        }
    }
}