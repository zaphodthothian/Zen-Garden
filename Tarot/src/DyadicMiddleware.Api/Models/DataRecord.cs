using System.Text.Json;

namespace DyadicMiddleware.Api.Models;

public record DataRecord(
    Guid Id,
    string SourceKey,
    DateTimeOffset ReceivedAt,
    Dictionary<string, JsonElement> Payload)
{
    public static DataRecord FromRequest(IngestRecordRequest request)
    {
        return new DataRecord(
            Guid.NewGuid(),
            request.SourceKey,
            DateTimeOffset.UtcNow,
            new Dictionary<string, JsonElement>(request.Payload ?? new()));
    }
}
