using DyadicMiddleware.Api.Models;

namespace DyadicMiddleware.Api.Services;

public interface IDataStore
{
    void Add(DataRecord record);
    IEnumerable<DataRecord> GetAll();
    IEnumerable<DataRecord> GetForSource(string sourceKey);
    DataRecord? Get(Guid id);
}
