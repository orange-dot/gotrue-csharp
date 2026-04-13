using System;
using System.Collections.Generic;

namespace Supabase.Gotrue
{
internal sealed class QueryStringCollection
{
    private readonly List<KeyValuePair<string, string>> _items = new List<KeyValuePair<string, string>>();

    public static QueryStringCollection Parse(string? queryString)
    {
        var collection = new QueryStringCollection();
        var source = queryString ?? string.Empty;

        if (string.IsNullOrWhiteSpace(source))
        {
            return collection;
        }

        var trimmed = source[0] == '?' ? source.Substring(1) : source;

        if (trimmed.Length == 0)
        {
            return collection;
        }

        foreach (var segment in trimmed.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries))
        {
            var separatorIndex = segment.IndexOf('=');

            if (separatorIndex < 0)
            {
                collection.Add(Decode(segment), string.Empty);
                continue;
            }

            var key = segment.Substring(0, separatorIndex);
            var value = segment.Substring(separatorIndex + 1);
            collection.Add(Decode(key), Decode(value));
        }

        return collection;
    }

    public string? this[string key]
    {
        get => Get(key);
        set
        {
            RemoveAll(key);

            if (value != null)
            {
                Add(key, value);
            }
        }
    }

    public void Add(string key, string? value)
    {
        _items.Add(new KeyValuePair<string, string>(key, value ?? string.Empty));
    }

    public string? Get(string key)
    {
        foreach (var item in _items)
        {
            if (string.Equals(item.Key, key, StringComparison.Ordinal))
            {
                return item.Value;
            }
        }

        return null;
    }

    public override string ToString()
    {
        if (_items.Count == 0)
        {
            return string.Empty;
        }

        var segments = new string[_items.Count];

        for (var index = 0; index < _items.Count; index++)
        {
            var item = _items[index];
            segments[index] = $"{Encode(item.Key)}={Encode(item.Value)}";
        }

        return string.Join("&", segments);
    }

    private void RemoveAll(string key)
    {
        _items.RemoveAll(item => string.Equals(item.Key, key, StringComparison.Ordinal));
    }

    private static string Decode(string value)
    {
        return Uri.UnescapeDataString(value.Replace("+", " "));
    }

    private static string Encode(string value)
    {
        return Uri.EscapeDataString(value).Replace("%20", "+");
    }
}
}
