# Release Guide

This guide covers how to set up automated releases and versioning for Sgnome.

## Initial Setup (One-time)

### 1. GitHub Secrets Setup

Add these secrets to your GitHub repository (Settings → Secrets and variables → Actions):

#### Required Secrets:
- **`DOCKER_USERNAME`** = Docker Hub username
- **`DOCKER_PASSWORD`** =  Docker Hub access token


## Creating Releases

### Development Builds
- **On push to main/develop** - GitHub Actions automatically:
  - Calculates version (e.g., `0.1.0-123` for build #123)
  - Updates all .NET projects via `Directory.Build.props`
  - Updates `package.json`
  - Builds and tests everything
  - Creates Docker image with version

### Production Releases
1. **Create a git tag**: 
   ```bash
   git tag v1.2.3
   git push origin v1.2.3
   ```

2. **GitHub Actions automatically**:
   - Uses the tag version (`1.2.3`)
   - Updates all projects
   - Builds and pushes Docker image to `lilchim/sgnome-web:1.2.3`
   - Also tags as `lilchim/sgnome-web:latest`

## Available Images

After a release, images will be available at:
- `docker pull lilchim/sgnome-web:1.2.3` (specific version)
- `docker pull lilchim/sgnome-web:latest` (latest version)

## Versioning Strategy

- **Development**: `0.1.0-{build_number}` (e.g., `0.1.0-123`)
- **Releases**: Use semantic versioning (e.g., `v1.2.3`)
- **All .NET projects**: Versioned via `Directory.Build.props`
- **Frontend**: Versioned via `package.json`
- **Docker**: Tagged with the same version 