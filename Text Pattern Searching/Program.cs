using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

var summary = BenchmarkRunner.Run<PatternSearchBenchmark>();
Console.WriteLine(summary);

public static class HashSetPatternSearcher
{
    public static bool Search(string text, string pattern)
    {
        if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(pattern)) 
            return false;

        var patternSet = new HashSet<string> { pattern };
        var patternLength = pattern.Length;

        for (int i = 0; i < text.Length - pattern.Length; i++)
        {
            var subString = text.Substring(i, pattern.Length);
            if (patternSet.Contains(subString))
            {
                return true;
            }
        }
        return false;
    }

}

public class TrieNode
{
    public bool IsWord { get; set; }
    public Dictionary<char, TrieNode> Children { get; } = new();
}

public class Trie
{
    private readonly TrieNode _root = new();

    public void AddWord(string word)
    {
        var node = _root;
        foreach (char ch in word)
        {
            if (!node.Children.ContainsKey(ch))
            {
                node.Children[ch] = new();
            }
            node = node.Children[ch];
        }
        node.IsWord = true;
    }

    public bool Search(string word)
    {
        var node = _root;
        foreach(char ch in word)
        {
            if (!node.Children.ContainsKey(ch))
            {
                return false;
            }
            node = node.Children[ch];
        }
        return node.IsWord;
    }
}

public class PatternSearchBenchmark
{
    private const string _text = "asldjfasfoiausaslfjaksjiuqerjfqwcsncmanxzcl";
    private const string _pattern = "zc";
    private const int _iterations = 1000000;
    private readonly Trie _trie;

    public PatternSearchBenchmark()
    {
        _trie = new Trie();
        _trie.AddWord(_pattern);
    }


    [Benchmark]
    public void TrieSearch()
    {
        for (int i = 0; i < _iterations; i++)
        {
            _trie.Search(_text);
        }
    }

    [Benchmark]
    public void HashSetSearch()
    {
        for (int i = 0; i < _iterations; i++)
        {
            HashSetPatternSearcher.Search(_text, _pattern);
        }
    }
}