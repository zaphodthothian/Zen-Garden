using DyadicMiddleware.Api.Models;

namespace DyadicMiddleware.Api.Services;

public class SourceRegistry
{
    private readonly Dictionary<string, SourceRegistration> _sources =
        new(StringComparer.OrdinalIgnoreCase);

    public bool TryUpsert(SourceRegistration registration, out bool created)
    {
        created = false;

        if (_sources.TryGetValue(registration.Key, out var existing))
        {
            if (!Matches(existing, registration))
            {
                return false;
            }

            _sources[registration.Key] = registration;
            return true;
        }

        _sources[registration.Key] = registration;
        created = true;
        return true;
    }

    public SourceRegistration? Get(string key)
    {
        return _sources.TryGetValue(key, out var registration) ? registration : null;
    }

    public bool Contains(string key) => _sources.ContainsKey(key);

    public IReadOnlyCollection<SourceRegistration> GetAll() => _sources.Values.ToArray();

    private static bool Matches(SourceRegistration existing, SourceRegistration incoming)
    {
        return string.Equals(existing.Description, incoming.Description, StringComparison.Ordinal) &&
               UriEquals(existing.CallbackUrl, incoming.CallbackUrl) &&
               DictionaryEquals(existing.FieldMappings, incoming.FieldMappings);
    }

    private static bool UriEquals(Uri? left, Uri? right)
    {
        if (left is null && right is null)
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        return left.Equals(right);
    }

    private static bool DictionaryEquals(Dictionary<string, string>? left, Dictionary<string, string>? right)
    {
        if (left is null && right is null)
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        if (left.Count != right.Count)
        {
            return false;
        }

        foreach (var pair in left)
        {
            if (!right.TryGetValue(pair.Key, out var value) || !string.Equals(pair.Value, value, StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }
}
