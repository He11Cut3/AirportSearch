using System;

namespace AirportSearch
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length != 1 || !int.TryParse(args[0], out int columnIndex) || columnIndex < 1)
            {
                Console.WriteLine("Использование: AirportSearch.exe <номер_колонки>");
                Console.WriteLine("Номер колонки должен быть положительным целым числом (начиная с 1)");
                return;
            }

            try
            {
                var searchEngine = new AirportSearchEngine("airports.dat", columnIndex - 1);

                Console.WriteLine("Введите текст для поиска (или 'quit' для выхода):");
                while (true)
                {
                    Console.Write("> ");
                    string searchText = Console.ReadLine()?.Trim();

                    if (string.Equals(searchText, "quit", StringComparison.OrdinalIgnoreCase))
                        break;

                    if (string.IsNullOrEmpty(searchText))
                    {
                        Console.WriteLine("Пожалуйста, введите текст для поиска");
                        continue;
                    }

                    var results = searchEngine.Search(searchText);
                    foreach (var result in results)
                    {
                        Console.WriteLine($"{result.Key}[{result.Line}]");
                    }
                    Console.WriteLine($"Найдено совпадений: {results.Count}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }
}