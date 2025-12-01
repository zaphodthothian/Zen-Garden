using System.Text.Json;

namespace DyadicMiddleware.Api.Models;

public record IngestRecordRequest
{
    public required string SourceKey { get; init; }
    public Dictionary<string, JsonElement>? Payload { get; init; }
}
