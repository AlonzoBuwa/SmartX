namespace SmartX.Api.Services
{
    public class SearchHistoryEntry
    {
        public string SearchTerm { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class SearchHistoryService
    {
        private readonly List<SearchHistoryEntry> _searchHistory = new();

        public void AddSearch(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return;

            _searchHistory.Add(new SearchHistoryEntry
            {
                SearchTerm = searchTerm.Trim(),
                Timestamp = DateTime.UtcNow
            });
        }

        public List<SearchHistoryEntry> GetHistory()
        {
            return _searchHistory
                .OrderByDescending(item => item.Timestamp)
                .ToList();
        }

        public int GetSearchCount()
        {
            return _searchHistory.Count;
        }

        public void ClearHistory()
        {
            _searchHistory.Clear();
        }
    }
}