# Dependencies

## External Dependencies

### Production Dependencies

| Package | Version | Purpose | Integration Boundary |
|---------|---------|---------|---------------------|
| `MonoGame.Framework.DesktopGL` | 3.8.4 | XNA/MonoGame asset, content, audio, and graphics types | Content namespace — `PipelineContentLoader`, `PlainFileContentLoader`, `NuciContentManager` |
| `NuciXNA.Primitives` | 2.1.7 | `Colour`, `Point2D`, `Size2D` value types | `Bitmap` public API |
| `SixLabors.ImageSharp` | 3.1.6 | Image decoding, encoding, buffer representation, pixel type | `Bitmap` — all image mechanics |

### Test Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| `Microsoft.NET.Test.Sdk` | 18.8.1 | Test discovery and execution |
| `MonoGame.Framework.DesktopGL` | 3.8.4 | MonoGame types for test mocks |
| `Moq` | 4.20.72 | Mocking `IContentLoader` for manager tests |
| `NUnit` | 4.6.1 | Test framework and assertions |
| `NUnit3TestAdapter` | 6.2.0 | NUnit integration with test SDK |

## Internal Dependencies

```
NuciXNA.DataAccess.UnitTests
         │
         ▼
NuciXNA.DataAccess (production)
         │
         ├──► MonoGame.Framework.DesktopGL
         ├──► NuciXNA.Primitives
         └──► SixLabors.ImageSharp
```

## Dependency Direction Rules

1. **Production code depends outward** on external libraries
2. **Test code depends inward** on production code
3. **Production code does not reference** the test project
4. **No circular dependencies** within production code

## Version Pinning

All dependencies are pinned to specific versions in the `.csproj` files. No floating versions or version ranges are used.

## Vulnerability Status

| Dependency | Current Version | Known Vulnerabilities |
|------------|-----------------|----------------------|
| `SixLabors.ImageSharp` | 3.1.6 | Restore reports known high- and moderate-severity vulnerabilities |
| `MonoGame.Framework.DesktopGL` | 3.8.4 | Check MonoGame security advisories |
| `NuciXNA.Primitives` | 2.1.7 | Check upstream advisories |

**Action required:** Regular dependency updates and security monitoring.

## Transitive Dependencies

Key transitive dependencies (from `dotnet list package --include-transitive`):

- `System.Memory` (via ImageSharp)
- `System.Buffers` (via ImageSharp)
- `System.Runtime.CompilerServices.Unsafe` (via ImageSharp)
- `Microsoft.Xna.Framework` (via MonoGame)

## Dependency Update Policy

1. **Patch updates:** Apply when available, test, and release
2. **Minor updates:** Evaluate breaking changes, test thoroughly
3. **Major updates:** Plan migration, update architecture docs if needed
4. **Security updates:** Prioritise immediately

## Related Documentation

- [Architecture](./architecture.md) — Dependency direction diagram
- [Security](./security.md) — Vulnerability management
- [Build and Deployment](./build-and-deployment.md) — CI dependency restore