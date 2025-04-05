using System;

namespace AirportSearch
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 1 || !int.TryParse(args[0], out int columnIndex) || columnIndex < 1)
            {
                Console.WriteLine("Usage: AirportSearch.exe <column_number>");
                Console.WriteLine("Column number must be a positive integer (1-based index)");
                return;
            }

            try
            {
                var searchEngine = new AirportSearchEngine("airports.dat", columnIndex - 1);

                Console.WriteLine("Enter search text (or 'quit' to exit):");
                while (true)
                {
                    Console.Write("> ");
                    string searchText = Console.ReadLine()?.Trim();

                    if (string.Equals(searchText, "quit", StringComparison.OrdinalIgnoreCase))
                        break;

                    if (string.IsNullOrEmpty(searchText))
                    {
                        Console.WriteLine("Please enter search text");
                        continue;
                    }

                    var results = searchEngine.Search(searchText);
                    foreach (var result in results)
                    {
                        Console.WriteLine($"{result.Key}[{result.Line}]");
                    }
                    Console.WriteLine($"Found {results.Count} matches");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}