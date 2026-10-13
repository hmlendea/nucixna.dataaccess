# Build and Deployment

## Build Pipeline

### Local Build

```bash
# Restore dependencies
dotnet restore

# Build solution
dotnet build NuciXNA.DataAccess.sln

# Run tests
dotnet test NuciXNA.DataAccess.sln
```

### CI Pipeline

**File:** [`.github/workflows/dotnet.yml`](../.github/workflows/dotnet.yml)

**Triggers:**
- Push to `master`
- Pull request to `master`

**Environment:** `ubuntu-latest`

**Steps:**
1. Checkout repository
2. Setup .NET 10.0.x
3. Install Microsoft core fonts (required for MonoGame SpriteFont tests)
4. Restore dependencies
5. Build (no restore)
6. Test (no build, normal verbosity)

**Requirements:**
- .NET 10.0 SDK
- `apt` package installation capability (for fonts)
- NuGet package availability

## Deployment

### Artifact

The distributable unit is the **NuGet package** produced from `NuciXNA.DataAccess.csproj`.

**Package metadata:**
- Target framework: `net10.0`
- Package ID: `NuciXNA.DataAccess`
- Version: From `<Version>` in csproj (currently 4.0.4)
- License: GPL-3.0-or-later
- Authors: Horațiu Mlendea
- Repository: https://github.com/hmlendea/nucixna.dataaccess

### Package Creation

```bash
dotnet pack NuciXNA.DataAccess/NuciXNA.DataAccess.csproj -c Release -o ./artifacts
```

### Publishing

Publishing to NuGet.org is a manual or separate CI step not defined in this repository.

## Environments

| Environment | Purpose | Configuration |
|-------------|---------|---------------|
| Development | Local development | `dotnet build` (Debug) |
| CI | Validation | `dotnet build --no-restore` + `dotnet test --no-build` (Release) |
| Package | Distribution | `dotnet pack -c Release` |

## Build Configurations

| Configuration | Define Constants | Optimisation |
|---------------|------------------|--------------|
| Debug | `DEBUG`, `TRACE` | Disabled |
| Release | `TRACE` | Enabled |

## Target Framework

- **Single TFM:** `net10.0`
- **No multi-targeting**

## Dependencies at Build Time

| Dependency | Source |
|------------|--------|
| `MonoGame.Framework.DesktopGL` 3.8.4 | NuGet |
| `NuciXNA.Primitives` 2.1.7 | NuGet |
| `SixLabors.ImageSharp` 3.1.6 | NuGet |

## Known Build Issues

1. **ImageSharp vulnerabilities:** Restore reports known high- and moderate-severity vulnerabilities for pinned version 3.1.6
2. **Font installation:** CI requires `ttf-mscorefonts-installer` for SpriteFont rendering in tests

## Related Documentation

- [Dependencies](./dependencies.md)
- [Testing](./testing.md)
- [Repository Structure](./repository-structure.md)