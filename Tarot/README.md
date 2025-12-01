# Dyadic Field Middleware

A lightweight RESTful middleware layer built with ASP.NET Core for normalizing and exposing data from upstream systems so it can be consumed by the Dyadic Field. The service provides registration of external sources and ingestion/query endpoints backed by an in-memory store (swap in your preferred persistence layer later).

## Features
- Register source systems with optional field mappings and callback URLs.
- Ingest normalized payloads for registered sources.
- Query payloads across all sources or by source key.
- Minimal API surface that is easy to containerize and deploy to AWS.

## Project layout
```
src/
  DyadicMiddleware.Api/
    DyadicMiddleware.Api.csproj
    Program.cs                 # Minimal API endpoints and wiring
    Models/                    # DTOs for requests and stored records
    Services/                  # In-memory registry and data store abstractions
Dockerfile                     # Container image suitable for AWS ECS/EKS/App Runner
```

## Running locally
1. Install the .NET 8 SDK.
2. Restore and run the API:
   ```bash
   cd src/DyadicMiddleware.Api
   dotnet restore
   dotnet run
   ```
3. The service listens on `http://localhost:5080` by default; adjust with `ASPNETCORE_URLS` if needed.

## REST endpoints
### Register or update a source
- **POST** `/sources`
```json
{
  "key": "crm",
  "description": "Salesforce CRM feed",
  "fieldMappings": { "firstName": "given_name" },
  "callbackUrl": "https://hooks.example.com/dyadic/ack"
}
```
- Returns `201 Created` for new sources, `200 OK` when an identical registration already exists, or `409 Conflict` when attempting to change an existing source in-place.

### List sources
- **GET** `/sources`

### Ingest a record
- **POST** `/records`
```json
{
  "sourceKey": "crm",
  "payload": {
    "firstName": "Ada",
    "lastName": "Lovelace",
    "email": "ada@example.com"
  }
}
```
- Requires the source to be registered first. Returns `201 Created` with the stored record envelope.

### Query records
- **GET** `/records` — All records (newest first).
- **GET** `/records?source=crm` — Records for a specific source.
- **GET** `/records/{id}` — Fetch a single record by identifier.

## Deployment (AWS-focused)
- **Container image**: Build with the included `Dockerfile` and push to ECR.
- **App Runner/ECS/Fargate**: Configure port `8080` (see `Dockerfile`), set `ASPNETCORE_URLS=http://+:8080`, and plug in secrets via environment variables.
- **Observability**: Add AWS X-Ray/OpenTelemetry and a persistent store (Aurora DynamoDB/S3) as follow-ups. The current in-memory store is a starter implementation.

## Next steps
- Replace the in-memory store with DynamoDB, PostgreSQL, or another managed store.
- Add authentication/authorization (e.g., Amazon Cognito or custom JWTs).
- Wire in source-specific adapters for transformation before persisting to the Dyadic Field.
