using SmartX.Api.Models;

namespace SmartX.Api.Services
{
    public class AVLTree
    {
        private class AVLNode
        {
            public DateTime Timestamp { get; set; }

            public List<TelemetryPacket<double>> Packets { get; set; }

            public AVLNode? Left { get; set; }

            public AVLNode? Right { get; set; }

            public int Height { get; set; }

            public AVLNode(TelemetryPacket<double> packet)
            {
                Timestamp = packet.Timestamp;
                Packets = new List<TelemetryPacket<double>> { packet };
                Height = 1;
            }
        }

        private AVLNode? _root;

        // Inserts a telemetry reading into the AVL Tree.
        public void Insert(TelemetryPacket<double> packet)
        {
            if (packet == null)
                throw new ArgumentNullException(nameof(packet));

            _root = InsertNode(_root, packet);
        }

        private AVLNode InsertNode(
            AVLNode? node,
            TelemetryPacket<double> packet)
        {
            if (node == null)
                return new AVLNode(packet);

            if (packet.Timestamp < node.Timestamp)
            {
                node.Left = InsertNode(node.Left, packet);
            }
            else if (packet.Timestamp > node.Timestamp)
            {
                node.Right = InsertNode(node.Right, packet);
            }
            else
            {
                // Multiple readings can share the same timestamp.
                node.Packets.Add(packet);
                return node;
            }

            UpdateHeight(node);

            int balance = GetBalance(node);

            // Left-Left Case
            if (balance > 1 &&
                packet.Timestamp < node.Left!.Timestamp)
            {
                return RotateRight(node);
            }

            // Right-Right Case
            if (balance < -1 &&
                packet.Timestamp > node.Right!.Timestamp)
            {
                return RotateLeft(node);
            }

            // Left-Right Case
            if (balance > 1 &&
                packet.Timestamp > node.Left!.Timestamp)
            {
                node.Left = RotateLeft(node.Left!);
                return RotateRight(node);
            }

            // Right-Left Case
            if (balance < -1 &&
                packet.Timestamp < node.Right!.Timestamp)
            {
                node.Right = RotateRight(node.Right!);
                return RotateLeft(node);
            }

            return node;
        }

        private int GetHeight(AVLNode? node)
        {
            return node?.Height ?? 0;
        }

        private void UpdateHeight(AVLNode node)
        {
            node.Height = 1 + Math.Max(
                GetHeight(node.Left),
                GetHeight(node.Right));
        }

        private int GetBalance(AVLNode? node)
        {
            if (node == null)
                return 0;

            return GetHeight(node.Left) -
                   GetHeight(node.Right);
        }

        // Performs a right rotation.
        private AVLNode RotateRight(AVLNode y)
        {
            AVLNode x = y.Left!;
            AVLNode? temporary = x.Right;

            x.Right = y;
            y.Left = temporary;

            UpdateHeight(y);
            UpdateHeight(x);

            return x;
        }

        // Performs a left rotation.
        private AVLNode RotateLeft(AVLNode x)
        {
            AVLNode y = x.Right!;
            AVLNode? temporary = y.Left;

            y.Left = x;
            x.Right = temporary;

            UpdateHeight(x);
            UpdateHeight(y);

            return y;
        }

        // Retrieves readings within a timestamp range.
        public List<TelemetryPacket<double>> SearchRange(
            DateTime startTime,
            DateTime endTime,
            string? sensorId = null)
        {
            var results = new List<TelemetryPacket<double>>();

            if (startTime > endTime)
                throw new ArgumentException(
                    "Start time cannot be after end time.");

            SearchRangeRecursive(
                _root,
                startTime,
                endTime,
                sensorId,
                results);

            return results;
        }

        private void SearchRangeRecursive(
            AVLNode? node,
            DateTime startTime,
            DateTime endTime,
            string? sensorId,
            List<TelemetryPacket<double>> results)
        {
            if (node == null)
                return;

            // Search the left subtree when it may contain
            // timestamps within the requested range.
            if (startTime < node.Timestamp)
            {
                SearchRangeRecursive(
                    node.Left,
                    startTime,
                    endTime,
                    sensorId,
                    results);
            }

            // Include readings within the requested range.
            if (node.Timestamp >= startTime &&
                node.Timestamp <= endTime)
            {
                var matchingPackets = node.Packets.Where(packet =>
                    string.IsNullOrWhiteSpace(sensorId) ||
                    packet.SensorId.Equals(
                        sensorId,
                        StringComparison.OrdinalIgnoreCase));

                results.AddRange(matchingPackets);
            }

            // Search the right subtree when it may contain
            // timestamps within the requested range.
            if (endTime > node.Timestamp)
            {
                SearchRangeRecursive(
                    node.Right,
                    startTime,
                    endTime,
                    sensorId,
                    results);
            }
        }

        // Returns the total number of stored readings.
        public int Count()
        {
            return CountNodes(_root);
        }

        private int CountNodes(AVLNode? node)
        {
            if (node == null)
                return 0;

            return node.Packets.Count +
                   CountNodes(node.Left) +
                   CountNodes(node.Right);
        }
    }
}
