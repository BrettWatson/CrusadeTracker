# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

CrusadeTracker is a .NET 9 Web API for tracking Warhammer 40k Crusade forces, units, battles, and campaigns. It follows Domain-Driven Design (DDD) with Clean Architecture.

## Build and Run Commands

```bash
dotnet build                                    # Build entire solution
dotnet run --project src/CrusadeTracker.API     # Run the API server
dotnet ef migrations add <MigrationName> --project src/CrusadeTracker.Infrastructure --startup-project src/CrusadeTracker.API
dotnet ef database update --project src/CrusadeTracker.Infrastructure --startup-project src/CrusadeTracker.API
```

## Architecture

**Clean Architecture Layers (dependency flows inward):**

- **Domain** (`src/CrusadeTracker.Domain`) - Core business logic, entities, value objects, repository interfaces. No external dependencies.
- **Application** (`src/CrusadeTracker.Application`) - Use cases and orchestration. Depends only on Domain.
- **Infrastructure** (`src/CrusadeTracker.Infrastructure`) - EF Core persistence, repository implementations. Depends on Domain and Application.
- **API** (`src/CrusadeTracker.API`) - HTTP endpoints, DI configuration. References all layers.

## Domain Patterns

**Strongly-Typed IDs:** All entities use record struct IDs (ForceId, UnitId, BattleId, CampaignId, UserId) wrapping Guid.

**Aggregate Roots:** CrusadeForce and Battle implement IAggregateRoot. Create via static factory methods:
```csharp
CrusadeForce.Create(userId, name, faction, supplyLimit)
Battle.Record(...)
```

**Value Objects:** Points, ExperiencePoints, SupplyLimit are readonly structs with validation. Use implicit conversion from int.

**Encapsulated Collections:** Aggregates expose IReadOnlyCollection properties backed by private List fields. Modifications go through aggregate methods.

## Database

- SQL Server with EF Core 9.0
- Entity configurations in `Infrastructure/Persistence/Configurations/`
- Value objects mapped via HasConversion
- Battle honours/scars stored as owned collections in separate tables

## Code Style

- Prefer `var` for built-in and apparent types only
- 4-space indentation, LF line endings
- System usings sorted first
