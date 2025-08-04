# Sgnome Games Research

A gaming research tool that allows users to naturally explore relationships between games, developers, and publishers through an interactive graph interface.

[Demo Here](https://sgnome-web-app.fly.dev)

## Architecture

### Monorepo Structure
```
Sgnome-Main/
├── src/
│   ├── Sgnome.Web/              # ASP.NET Core app hosting API + Svelte frontend
│   ├── Sgnome.Frontend/         # Svelte app 
│   ├── Sgnome.Models/           # Core graph data models
│   └── services/                # Business-focused services
│       ├── PlayerService/       # Answers: "What's player X's profile?"
│       ├── LibraryService/      # Answers: "What games does player X own?"
│       └── OrganizedLibraryService/ # Answers: "How is player X's library organized?"
├── docker/                      # Docker configuration
└── docs/                        # Documentation
```

### Technology Stack
- **Backend**: .NET 8, ASP.NET Core
- **Frontend**: Svelte 5 + xyflow (graph visualization)
- **Data Sources**: Steam API, RAWG, Epic Games Store, etc.
- **Caching**: Redis
- **Deployment**: Docker

## Development

### Prerequisites
- .NET 8 SDK
- Node.js 18+
- Docker (optional)

### Local Development
1. **Backend**: `dotnet run --project src/Sgnome.Web` (runs on port 5000)
2. **Frontend**: `cd src/Sgnome.Frontend && npm run dev` (runs on port 5173)

### Docker Development
```bash
# Start all services (Steam API, Redis, Sgnome Web)
docker-compose -f docker/docker-compose.yml up

# Build and start in detached mode
docker-compose -f docker/docker-compose.yml up -d

# View logs
docker-compose -f docker/docker-compose.yml logs -f

# Stop all services
docker-compose -f docker/docker-compose.yml down
```

### Production
- Svelte builds to `dist` and is copied to `wwwroot` by Docker
- Single Docker container deployment
- Access at `http://localhost:5000`

## Features (Planned)
- Interactive graph exploration of gaming data
- Pin-based progressive disclosure of relationships
- Multi-source data aggregation (Steam, Epic, etc.)
- User library analysis and recommendations
- Collaborative research boards
- HTTP/2 streaming for real-time data loading

## Status
🚧 **In Development** - Backend service patterns established, Initial frontend prototyping



