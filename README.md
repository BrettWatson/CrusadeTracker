# CrusadeTracker

A .NET 9 application for tracking Warhammer 40k Crusade forces, units, battles, and campaigns. Includes a Web API backend and Blazor WebAssembly frontend.

## Features

- **User Authentication** - Register, login, JWT tokens with refresh token support
- **Crusade Forces** - Create and manage your Crusade forces with faction and points limits
- **Units** - Add units to forces with datasheets, points, experience, battle honours, and scars
- **Battles** - Record battles between forces, track participants and results
- **Web Frontend** - Blazor WebAssembly SPA for managing your Crusade

## Solution Structure

```
src/
├── CrusadeTracker.Domain        # Domain model (DDD) - entities, value objects, repository interfaces
├── CrusadeTracker.Application   # DTOs and service interfaces
├── CrusadeTracker.Infrastructure # EF Core, Identity, repositories, JWT services
├── CrusadeTracker.API           # HTTP API controllers
└── CrusadeTracker.Web           # Blazor WebAssembly frontend
```

## Requirements

- .NET 9 SDK
- SQL Server (LocalDB or Docker)

## Getting Started

### 1. Start Database

**Option A: Docker (recommended)**
```bash
docker-compose up -d
```
This starts SQL Server on port 1434 (to avoid conflicts with local SQL Server).

**Option B: LocalDB**

Update the connection string in `appsettings.Development.json`:
```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CrusadeTracker;Trusted_Connection=True;MultipleActiveResultSets=true"
```

### 2. Build
```bash
dotnet build
```

### 3. Apply Migrations
```bash
dotnet ef database update --project src/CrusadeTracker.Infrastructure --startup-project src/CrusadeTracker.API
```

### 4. Run API
```bash
dotnet run --project src/CrusadeTracker.API
```
The API runs on `http://localhost:5051` by default.

### 5. Run Web Frontend (separate terminal)
```bash
dotnet run --project src/CrusadeTracker.Web
```
The web app runs on `http://localhost:5052` by default.

### 6. Open Application
- **Web App**: Navigate to `http://localhost:5052`
- **Swagger API Docs**: Navigate to `http://localhost:5051/swagger`

## API Endpoints

### Authentication (`/api/auth`)
| Method | Route | Auth | Description |
|--------|-------|------|-------------|
| POST | `/api/auth/register` | No | Create account |
| POST | `/api/auth/login` | No | Get JWT tokens |
| POST | `/api/auth/refresh` | No | Refresh access token |
| POST | `/api/auth/revoke` | Yes | Revoke refresh token |

### Forces (`/api/forces`)
| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/forces` | List your forces |
| GET | `/api/forces/{id}` | Get force with units |
| POST | `/api/forces` | Create a force |
| PUT | `/api/forces/{id}` | Update name/faction |
| DELETE | `/api/forces/{id}` | Delete a force |

### Units (`/api/forces/{forceId}/units`)
| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/forces/{forceId}/units` | List units in force |
| GET | `/api/forces/{forceId}/units/{unitId}` | Get unit details |
| POST | `/api/forces/{forceId}/units` | Add unit to force |
| PUT | `/api/forces/{forceId}/units/{unitId}` | Rename unit |
| DELETE | `/api/forces/{forceId}/units/{unitId}` | Remove unit |
| POST | `/api/forces/{forceId}/units/{unitId}/honours` | Add battle honour |
| DELETE | `/api/forces/{forceId}/units/{unitId}/honours/{honour}` | Remove honour |
| POST | `/api/forces/{forceId}/units/{unitId}/scars` | Add battle scar |
| DELETE | `/api/forces/{forceId}/units/{unitId}/scars/{scar}` | Remove scar |

### Battles (`/api/battles`)
| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/battles` | List your battles |
| GET | `/api/battles/{id}` | Get battle details |
| POST | `/api/battles` | Record a battle |
| POST | `/api/battles/{id}/participants` | Add your force to battle |
| POST | `/api/battles/{id}/results` | Set result (Victory/Defeat/Draw) |
| POST | `/api/battles/{id}/finalize` | Finalize the battle |

## Configuration

JWT settings are configured in `appsettings.Development.json`:

```json
{
  "JwtSettings": {
    "Secret": "your-secret-key-at-least-32-characters",
    "Issuer": "CrusadeTracker",
    "Audience": "CrusadeTracker",
    "AccessTokenExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7
  }
}
```

## Technology Stack

- **.NET 9** - Runtime and SDK
- **ASP.NET Core** - Web API framework
- **Blazor WebAssembly** - SPA frontend (runs in browser)
- **Entity Framework Core 9** - ORM with SQL Server
- **ASP.NET Core Identity** - User management and authentication
- **JWT Bearer** - Token-based authentication
- **Swagger/OpenAPI** - API documentation
- **Blazored.LocalStorage** - Client-side token storage
