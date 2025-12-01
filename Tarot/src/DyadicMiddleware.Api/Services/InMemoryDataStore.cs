using System.Collections.Concurrent;
using DyadicMiddleware.Api.Models;

namespace DyadicMiddleware.Api.Services;

public class InMemoryDataStore : IDataStore
{
    private readonly ConcurrentDictionary<Guid, DataRecord> _records = new();

    public void Add(DataRecord record)
    {
        _records[record.Id] = record;
    }

    public DataRecord? Get(Guid id)
    {
        return _records.TryGetValue(id, out var record) ? record : null;
    }

    public IEnumerable<DataRecord> GetAll()
    {
        return _records.Values
            .OrderByDescending(r => r.ReceivedAt)
            .ToList();
    }

    public IEnumerable<DataRecord> GetForSource(string sourceKey)
    {
        return _records.Values
            .Where(r => r.SourceKey.Equals(sourceKey, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(r => r.ReceivedAt)
            .ToList();
    }
}
