using System.Text.Json;
using DyadicMiddleware.Api.Models;
using DyadicMiddleware.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<SourceRegistry>();
builder.Services.AddSingleton<IDataStore, InMemoryDataStore>();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.SerializerOptions.WriteIndented = true;
});

var app = builder.Build();

var sources = app.MapGroup("/sources");

sources.MapPost("", (SourceRegistration request, SourceRegistry registry) =>
{
    if (string.IsNullOrWhiteSpace(request.Key))
    {
        return Results.BadRequest("A non-empty source key is required.");
    }

    if (!registry.TryUpsert(request, out var created))
    {
        return Results.Conflict($"Source '{request.Key}' already exists with different settings.");
    }

    return created
        ? Results.Created($"/sources/{request.Key}", registry.Get(request.Key)!)
        : Results.Ok(registry.Get(request.Key));
});

sources.MapGet("", (SourceRegistry registry) => Results.Ok(registry.GetAll()));
sources.MapGet("{key}", (string key, SourceRegistry registry) =>
{
    var source = registry.Get(key);
    return source is null ? Results.NotFound() : Results.Ok(source);
});

var records = app.MapGroup("/records");

records.MapPost("", (IngestRecordRequest request, SourceRegistry registry, IDataStore store) =>
{
    if (string.IsNullOrWhiteSpace(request.SourceKey))
    {
        return Results.BadRequest("SourceKey is required.");
    }

    if (request.Payload is null || request.Payload.Count == 0)
    {
        return Results.BadRequest("Payload is required and cannot be empty.");
    }

    if (!registry.Contains(request.SourceKey))
    {
        return Results.NotFound($"Source '{request.SourceKey}' is not registered.");
    }

    var record = DataRecord.FromRequest(request);
    store.Add(record);
    return Results.Created($"/records/{record.Id}", record);
});

records.MapGet("", (IDataStore store, string? source) =>
{
    var results = string.IsNullOrWhiteSpace(source)
        ? store.GetAll()
        : store.GetForSource(source);

    return Results.Ok(results);
});

records.MapGet("{id:guid}", (Guid id, IDataStore store) =>
{
    var record = store.Get(id);
    return record is null ? Results.NotFound() : Results.Ok(record);
});

app.MapGet("/", () => Results.Json(new
{
    name = "Dyadic Field Middleware",
    status = "ready",
    endpoints = new[] { "/sources", "/records" },
    docs = "Register external sources, ingest records, and query normalized payloads."
}));

app.Run();
