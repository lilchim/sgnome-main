# Sgnome Games Research

A gaming research tool that allows users to naturally explore relationships between games, developers, and publishers through an interactive graph interface.

## Architecture

### Monorepo Structure
```
Sgnome-Main/
├── src/
│   ├── Sgnome.Web/              # ASP.NET Core app hosting API + Svelte frontend
│   ├── Sgnome.Frontend/         # Svelte app 
│   └── services/                # Business-focused services
│       ├── UserLibraryService/  # Answers: "What games does user X own/play?"
│       ├── GameInfoService/     # Answers: "What are the details for game X?"
│       └── UserProfileService/  # Answers: "What's user X's gaming profile?"
├── docker/                      # Docker configuration
└── docs/                        # Documentation
```

### Technology Stack
- **Backend**: .NET 8, ASP.NET Core
- **Frontend**: Svelte + xyflow (graph visualization)
- **Data Sources**: Steam API, RAWG, Epic Games Store, etc.
- **Caching**: Redis
- **Deployment**: Docker

## Development

### Prerequisites
- .NET 8 SDK
- Node.js 18+
- Docker (optional)

### Local Development
1. **Backend**: `dotnet run` (runs on port 5000)
2. **Frontend**: `npm run dev` (runs on port 5173)

### Production
- Svelte builds to `wwwroot` and is served by ASP.NET Core
- Single Docker container deployment

## Features (Planned)
- Interactive graph exploration of gaming data
- Pin-based progressive disclosure of relationships
- Multi-source data aggregation (Steam, Epic, etc.)
- User library analysis and recommendations
- Collaborative research boards
- HTTP/2 streaming for real-time data loading

## Status
🚧 **In Development** - Setting up monorepo structure and initial services



