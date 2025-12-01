namespace DyadicMiddleware.Api.Models;

public record SourceRegistration
{
    public required string Key { get; init; }
    public string? Description { get; init; }
    public Dictionary<string, string>? FieldMappings { get; init; }
    public Uri? CallbackUrl { get; init; }
}
