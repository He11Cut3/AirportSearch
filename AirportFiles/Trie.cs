using System.Collections.Generic;
using System.Linq;

namespace AirportSearch
{
    public class Trie
    {
        private class TrieNode
        {
            public Dictionary<char, TrieNode> Children { get; } = new Dictionary<char, TrieNode>();
            public List<long> Positions { get; } = new List<long>();
        }

        private readonly TrieNode _root = new TrieNode();

        public void Insert(string key, long position)
        {
            TrieNode current = _root;

            foreach (char c in key)
            {
                if (!current.Children.TryGetValue(c, out TrieNode node))
                {
                    node = new TrieNode();
                    current.Children[c] = node;
                }
                current = node;
            }

            current.Positions.Add(position);
        }

        public IEnumerable<long> Search(string prefix)
        {
            TrieNode current = _root;

            foreach (char c in prefix)
            {
                if (!current.Children.TryGetValue(c, out current))
                {
                    return Enumerable.Empty<long>();
                }
            }

            return CollectPositions(current);
        }

        private IEnumerable<long> CollectPositions(TrieNode node)
        {
            var positions = new List<long>();
            var stack = new Stack<TrieNode>();
            stack.Push(node);

            while (stack.Count > 0)
            {
                TrieNode current = stack.Pop();
                positions.AddRange(current.Positions);

                foreach (var child in current.Children.Values)
                {
                    stack.Push(child);
                }
            }

            return positions;
        }
    }
}