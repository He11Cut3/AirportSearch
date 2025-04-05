namespace AirportSearch
{
    public class SearchResult
    {
        public string Key { get; }
        public string Line { get; }

        public SearchResult(string key, string line)
        {
            Key = key;
            Line = line;
        }
    }
}