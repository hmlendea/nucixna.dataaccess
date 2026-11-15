# Dependencies

## External Dependencies

### Production Dependencies

| Package | Version | Purpose | Integration Boundary |
|---------|---------|---------|---------------------|
| `MonoGame.Framework.DesktopGL` | 3.8.4 | XNA/MonoGame asset, content, audio, and graphics types | Content namespace — `PipelineContentLoader`, `PlainFileContentLoader`, `NuciContentManager` |
| `NuciXNA.Primitives` | 2.1.8 | `Colour`, `Point2D`, `Size2D` value types | `Bitmap` public API |
| `SixLabors.ImageSharp` | 4.1.2 | Image decoding, encoding, buffer representation, pixel type | `Bitmap` — all image mechanics |

### Test Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| `Microsoft.CodeAnalysis.Common` | 4.14.0 | Roslyn analysis for source-file convention tests |
| `Microsoft.CodeAnalysis.CSharp` | 4.14.0 | C# syntax analysis for source-file convention tests |
| `Microsoft.NET.Test.Sdk` | 18.10.1 | Test discovery and execution |
| `MonoGame.Framework.DesktopGL` | 3.8.4 | MonoGame types for test mocks |
| `Moq` | 4.21.0 | Mocking `IContentLoader` for manager tests |
| `NUnit` | 5.0.0 | Test framework and assertions |
| `NUnit3TestAdapter` | 6.3.1 | NUnit integration with test SDK |
| `TngTech.ArchUnitNET` | 0.13.5 | Compiled dependency rules for architecture tests |
| `TngTech.ArchUnitNET.NUnit` | 0.13.5 | NUnit integration for ArchUnitNET rules |

## Internal Dependencies

```
NuciXNA.DataAccess.UnitTests
NuciXNA.DataAccess.ArchitectureTests
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
5. **Content and IO subsystems remain independent** and free of dependency cycles (enforced by `DependencyArchitectureTests`)

## Version Pinning

All dependencies are pinned to specific versions in the `.csproj` files. No floating versions or version ranges are used.

## Vulnerability Status

| Dependency | Current Version | Known Vulnerabilities |
|------------|-----------------|----------------------|
| `SixLabors.ImageSharp` | 4.1.2 | Check ImageSharp security advisories |
| `MonoGame.Framework.DesktopGL` | 3.8.4 | Check MonoGame security advisories |
| `NuciXNA.Primitives` | 2.1.8 | Check upstream advisories |

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