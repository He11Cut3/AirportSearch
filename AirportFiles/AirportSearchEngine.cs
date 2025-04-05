using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;

namespace AirportSearch
{
    public class AirportSearchEngine
    {
        private readonly string _filePath;
        private readonly int _columnIndex;
        private readonly Lazy<Trie> _trie;
        private readonly Lazy<bool> _isNumericColumn;

        public AirportSearchEngine(string filePath, int columnIndex)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            _columnIndex = columnIndex;
            _trie = new Lazy<Trie>(BuildTrie, LazyThreadSafetyMode.ExecutionAndPublication);
            _isNumericColumn = new Lazy<bool>(() => DetermineIfNumericColumn(), LazyThreadSafetyMode.ExecutionAndPublication);
        }

        private Trie BuildTrie()
        {
            var trie = new Trie();
            long position = 0;

            using (var mmf = MemoryMappedFile.CreateFromFile(_filePath, FileMode.Open, null, 0, MemoryMappedFileAccess.Read))
            using (var accessor = mmf.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read))
            {
                byte[] buffer;
                long fileSize = new FileInfo(_filePath).Length;
                long bufferSize = Math.Min(fileSize, 1024 * 1024);
                buffer = new byte[bufferSize];

                long bytesRead = 0;
                StringBuilder lineBuilder = new StringBuilder();
                List<long> linePositions = new List<long>();

                while (bytesRead < fileSize)
                {
                    long bytesToRead = Math.Min(bufferSize, fileSize - bytesRead);
                    accessor.ReadArray(bytesRead, buffer, 0, (int)bytesToRead);

                    for (int i = 0; i < bytesToRead; i++)
                    {
                        char c = (char)buffer[i];
                        if (c == '\n')
                        {
                            string line = lineBuilder.ToString();
                            if (!string.IsNullOrEmpty(line))
                            {
                                var fields = ParseCsvLine(line);
                                if (_columnIndex < fields.Count)
                                {
                                    string columnValue = fields[_columnIndex];
                                    trie.Insert(columnValue.ToLowerInvariant(), position);
                                }
                                linePositions.Add(position);
                            }
                            position = bytesRead + i + 1;
                            lineBuilder.Clear();
                        }
                        else
                        {
                            lineBuilder.Append(c);
                        }
                    }

                    bytesRead += bytesToRead;
                }

                if (lineBuilder.Length > 0)
                {
                    string line = lineBuilder.ToString();
                    var fields = ParseCsvLine(line);
                    if (_columnIndex < fields.Count)
                    {
                        string columnValue = fields[_columnIndex];
                        trie.Insert(columnValue.ToLowerInvariant(), position);
                    }
                    linePositions.Add(position);
                }
            }

            return trie;
        }

        private bool DetermineIfNumericColumn()
        {
            using (var reader = new StreamReader(_filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var fields = ParseCsvLine(line);
                    if (_columnIndex < fields.Count)
                    {
                        string value = fields[_columnIndex];
                        if (value.Length > 0 && value[0] == '"' && value[value.Length - 1] == '"')
                        {
                            return false;
                        }
                        if (double.TryParse(fields[_columnIndex], out _))
                        {
                            return true;
                        }
                        return false;
                    }
                }
            }
            return false;
        }

        public List<SearchResult> Search(string searchText)
        {
            if (string.IsNullOrEmpty(searchText))
                return new List<SearchResult>();

            var results = new List<SearchResult>();
            var matches = _trie.Value.Search(searchText.ToLowerInvariant());

            using (var mmf = MemoryMappedFile.CreateFromFile(_filePath, FileMode.Open, null, 0, MemoryMappedFileAccess.Read))
            using (var accessor = mmf.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read))
            {
                foreach (var position in matches)
                {
                    string line = ReadLineAtPosition(accessor, position);
                    if (!string.IsNullOrEmpty(line))
                    {
                        var fields = ParseCsvLine(line);
                        if (_columnIndex < fields.Count)
                        {
                            results.Add(new SearchResult(
                                fields[_columnIndex],
                                line
                            ));
                        }
                    }
                }
            }

            if (_isNumericColumn.Value)
            {
                results = results.OrderBy(r =>
                {
                    if (double.TryParse(r.Key, out double num))
                        return num;
                    return double.MaxValue;
                }).ToList();
            }
            else
            {
                results = results.OrderBy(r => r.Key, StringComparer.OrdinalIgnoreCase).ToList();
            }

            return results;
        }

        private string ReadLineAtPosition(MemoryMappedViewAccessor accessor, long position)
        {
            const int bufferSize = 1024;
            byte[] buffer = new byte[bufferSize];
            StringBuilder sb = new StringBuilder();

            long offset = position;
            while (true)
            {
                int bytesToRead = (int)Math.Min(bufferSize, accessor.Capacity - offset);
                if (bytesToRead <= 0) break;

                accessor.ReadArray(offset, buffer, 0, bytesToRead);
                for (int i = 0; i < bytesToRead; i++)
                {
                    char c = (char)buffer[i];
                    if (c == '\n' || c == '\r')
                    {
                        return sb.ToString();
                    }
                    sb.Append(c);
                }
                offset += bytesToRead;
            }

            return sb.ToString();
        }

        private List<string> ParseCsvLine(string line)
        {
            var fields = new List<string>();
            bool inQuotes = false;
            StringBuilder currentField = new StringBuilder();

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    if (inQuotes && i < line.Length - 1 && line[i + 1] == '"')
                    {
                        currentField.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (c == ',' && !inQuotes)
                {
                    fields.Add(currentField.ToString());
                    currentField.Clear();
                }
                else
                {
                    currentField.Append(c);
                }
            }

            fields.Add(currentField.ToString());
            return fields;
        }
    }
}