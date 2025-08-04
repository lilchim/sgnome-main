# Build stage for frontend
FROM node:18-alpine AS frontend-build
WORKDIR /app
COPY src/Sgnome.Frontend/package*.json ./
RUN npm ci
COPY src/Sgnome.Frontend/ .
RUN npm run build

# Build stage for backend
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backend-build
WORKDIR /app
COPY *.sln ./
COPY src/Sgnome.Web/*.csproj ./src/Sgnome.Web/
COPY src/Sgnome.Models/*.csproj ./src/Sgnome.Models/
COPY src/services/PlayerService/*.csproj ./src/services/PlayerService/
COPY src/services/LibraryService/*.csproj ./src/services/LibraryService/
COPY src/services/GamesService/*.csproj ./src/services/GamesService/
COPY src/Sgnome.Clients/*.csproj ./src/Sgnome.Clients/
RUN dotnet restore

COPY . .
# Copy built frontend to wwwroot
COPY --from=frontend-build /app/dist ./src/Sgnome.Web/wwwroot
RUN dotnet publish src/Sgnome.Web/Sgnome.Web.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=backend-build /app/publish .
EXPOSE 80

# Add version info
ARG VERSION=0.1.0
LABEL version="${VERSION}"
LABEL maintainer="lilchim@protonmail.com"

ENTRYPOINT ["dotnet", "Sgnome.Web.dll"] 