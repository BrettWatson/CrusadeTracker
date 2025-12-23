# CrusadeTracker

A .NET 9 Web API for tracking Warhammer 40k Crusade forces, units, battles, and campaigns.

## Solution structure
- CrusadeTracker.Domain — domain model (DDD)
- CrusadeTracker.Application — use cases / orchestration
- CrusadeTracker.Infrastructure — EF Core, persistence, repositories
- CrusadeTracker.Api — HTTP API

## Requirements
- .NET 9 SDK
- SQL Server (localdb or Docker)

## Build
dotnet build

## Run
dotnet run --project src/CrusadeTracker.Api
