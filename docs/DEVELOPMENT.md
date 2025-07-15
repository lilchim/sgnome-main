# Development Guide

## Prerequisites

- .NET 8 SDK
- Node.js 18+
- Docker (optional, for Steam API service)

## Local Development Setup

### Option 1: Full Local Development

1. **Start Steam API Service** (if you have the container):
   ```bash
   docker run -p 8080:80 lilchim/steam-api-dotnet:latest
   ```

2. **Start Backend**:
   ```bash
   cd src/Sgnome.Web
   dotnet run
   ```
   The API will be available at `http://localhost:5000`

3. **Start Frontend** (in a new terminal):
   ```bash
   cd src/Sgnome.Frontend
   npm install
   npm run dev
   ```
   The frontend will be available at `http://localhost:5173`

### Option 2: Docker Development

1. **Start all services**:
   ```bash
   cd docker
   docker-compose up --build
   ```

2. **Access the application**:
   - Frontend: `http://localhost:5000`
   - API: `http://localhost:5000/api`
   - Swagger: `http://localhost:5000/swagger`

## Project Structure

```
Sgnome-Games-Research/
├── src/
│   ├── Sgnome.Web/              # ASP.NET Core API + static file hosting
│   │   ├── Controllers/         # API endpoints
│   │   ├── wwwroot/            # Built Svelte app (production)
│   │   └── Program.cs          # App configuration
│   ├── Sgnome.Frontend/        # Svelte + xyflow frontend
│   │   ├── src/
│   │   │   ├── App.svelte      # Main app component
│   │   │   └── main.ts         # Entry point
│   │   └── package.json
│   └── services/               # Business logic services
│       └── UserLibraryService/ # Steam library functionality
├── docker/                     # Docker configuration
└── docs/                       # Documentation
```

## Development Workflow

### Adding New Services

1. Create a new service directory in `src/services/`
2. Follow the pattern of `UserLibraryService/`
3. Add the service to `Program.cs` dependency injection
4. Create API controllers as needed

### Frontend Development

- The frontend uses Vite for fast development
- API calls are proxied to the backend during development
- Build output goes to `src/Sgnome.Web/wwwroot/`

### API Development

- Controllers are in `src/Sgnome.Web/Controllers/`
- Business logic is in `src/services/`
- Swagger documentation is auto-generated

## Testing

### Backend Testing
```bash
cd src/Sgnome.Web
dotnet test
```

### Frontend Testing
```bash
cd src/Sgnome.Frontend
npm run check
```

## Building for Production

### Full Build
```bash
# Build frontend
cd src/Sgnome.Frontend
npm run build

# Build backend
cd ../Sgnome.Web
dotnet publish -c Release
```

### Docker Build
```bash
cd docker
docker-compose -f docker-compose.yml build
```

## Configuration

### Environment Variables

- `STEAM_API_KEY`: Your Steam API key
- `SteamApi__BaseUrl`: Steam API service URL
- `SteamApi__ApiKey`: Steam API key (alternative format)

### Development Settings

The app uses `appsettings.json` for configuration. For development secrets, use:
```bash
dotnet user-secrets set "SteamApi:ApiKey" "your-steam-api-key"
``` 