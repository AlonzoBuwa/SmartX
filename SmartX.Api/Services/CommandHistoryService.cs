using SmartX.Api.Models;

namespace SmartX.Api.Services
{
    public class CommandHistoryService
    {
        // ============================================================
        // PART 2 REQUIREMENT
        // Stack<T> - LIFO command history
        // ============================================================

        private readonly Stack<DeviceCommand> _commandHistory = new();

        // ============================================================
        // ADD COMMAND
        // ============================================================

        public void AddCommand(DeviceCommand command)
        {
            _commandHistory.Push(command);
        }

        // ============================================================
        // GET COMMAND HISTORY
        // ============================================================

        public List<DeviceCommand> GetHistory()
        {
            return _commandHistory.ToList();
        }

        // ============================================================
        // GET MOST RECENT COMMAND
        // ============================================================

        public DeviceCommand? GetLastCommand()
        {
            if (_commandHistory.Count == 0)
                return null;

            return _commandHistory.Peek();
        }

        // ============================================================
        // UNDO LAST COMMAND
        // ============================================================

        public DeviceCommand? UndoLastCommand()
        {
            if (_commandHistory.Count == 0)
                return null;

            return _commandHistory.Pop();
        }

        // ============================================================
        // COMMAND COUNT
        // ============================================================

        public int GetCommandCount()
        {
            return _commandHistory.Count;
        }

        // ============================================================
        // CLEAR HISTORY
        // ============================================================

        public void ClearHistory()
        {
            _commandHistory.Clear();
        }
    }
}
