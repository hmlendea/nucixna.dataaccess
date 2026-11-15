# Testing

## Overview

`NuciXNA.DataAccess.UnitTests` is an NUnit test project with 364 discovered test cases covering the content loading and bitmap subsystems. `NuciXNA.DataAccess.ArchitectureTests` is a separate NUnit fitness-test project with 78 fitness tests that enforces dependency boundaries, project and solution conventions, production and test source conventions, and C# coding rules using ArchUnitNET and Roslyn.

## Test Organisation

```
NuciXNA.DataAccess.UnitTests/
├── Content/
│   ├── ContentLoaderTests.cs      # ContentLoader base class behaviour
│   ├── NuciContentManagerTests.cs # Manager orchestration and policy
│   ├── ThrowingContentLoader.cs   # Throwing test double
│   ├── NullReturningContentLoader.cs # Null-returning test double
│   └── SpyContentLoader.cs        # Path-capturing test double
└── IO/
    ├── BitmapTests.cs             # Bitmap construction, pixel access, disposal
    ├── TempFile.cs                # Temporary file helper
    └── TempDirectory.cs           # Temporary directory helper

NuciXNA.DataAccess.ArchitectureTests/
├── ArchitectureTestPaths.cs       # Repository path resolution from assembly metadata
├── SourceFileIndex.cs             # Cached Roslyn parse of every handwritten source file
├── SourceText.cs                  # Raw source lines for formatting rules
├── DependencyArchitectureTests.cs # Assembly dependency and project-reference rules
├── ProjectConventionTests.cs      # Project and solution configuration conventions
├── ProductionCodeConventionTests.cs # C# coding conventions for production sources
├── SourceFileConventionTests.cs   # Source-file naming and namespace conventions
└── TestSourceConventionTests.cs   # Unit-test and architecture-test source conventions
```

## Test Framework

| Component | Version |
|-----------|---------|
| NUnit | 5.0.0 |
| NUnit3TestAdapter | 6.3.1 |
| Microsoft.NET.Test.Sdk | 18.10.1 |
| Moq | 4.21.0 |
| TngTech.ArchUnitNET | 0.13.5 |
| TngTech.ArchUnitNET.NUnit | 0.13.5 |

## Running Tests

```bash
dotnet test NuciXNA.DataAccess.slnx
```

Or for a specific test project:

```bash
dotnet test NuciXNA.DataAccess.UnitTests/NuciXNA.DataAccess.UnitTests.csproj
dotnet test NuciXNA.DataAccess.ArchitectureTests/NuciXNA.DataAccess.ArchitectureTests.csproj
```

## Coverage Summary

| Test File | Test Cases | Coverage Focus |
|-----------|------------|----------------|
| `ContentLoaderTests.cs` | ~90 | `TryLoad*` exception suppression, null handling, path forwarding |
| `NuciContentManagerTests.cs` | ~100 | Loader selection, fallback branches, placeholder policy, path forwarding |
| `BitmapTests.cs` | ~66 | Construction, pixel round-trips, indexer equivalence, disposal |

## Detailed Coverage

### ContentLoaderTests.cs

**Test categories:**
- `TryLoadSoundEffect` — IOException, ArgumentException, InvalidOperationException, OutOfMemoryException, NullReferenceException, null return, path forwarding
- `TryLoadSpriteFont` — Same exception types, null return, path forwarding
- `TryLoadTexture2D` — Same exception types, null return, path forwarding

**Key assertions:**
- All exception types are caught and return `null`
- Path is forwarded unchanged to `Load*` method
- No exception escapes `TryLoad*`

### NuciContentManagerTests.cs

**Test categories:**
- `LoadSoundEffect` — Pipeline success, pipeline null → plain-file load, exception propagation, path forwarding, call counts
- `LoadSpriteFont` — Pipeline success, exception propagation, plain-file never called, path forwarding
- `LoadTexture2D` (no placeholder) — Pipeline null → plain-file load, plain-file null → null return, exception propagation, path forwarding
- `LoadTexture2D` (empty/whitespace placeholder) — Treated as absent, plain-file load called
- `LoadTexture2D` (with placeholder) — Pipeline null → plain-file TryLoad, plain-file null → placeholder load, placeholder exception propagation, placeholder null return, path forwarding

**Key assertions:**
- Correct loader method called for each branch
- Path forwarded correctly
- Placeholder policy changes fallback from required to optional
- Empty/whitespace placeholder treated as disabled

### BitmapTests.cs

**Test categories:**
- Construction — dimensions, `Size2D`, `Image<Rgba32>` wrapping
- `GetPixel` — Default transparent black on new bitmap
- `SetPixel`/`GetPixel` round-trips — Coordinates, `Point2D`, multiple pixels, overwrites
- Indexers — `int,int` and `Point2D` equivalence
- Disposal — No exception for various construction paths

**Key assertions:**
- Size matches constructor arguments
- Default pixel is transparent black
- Round-trip preserves colour exactly
- Indexers and methods are equivalent
- Dispose does not throw

## Coverage Gaps

| Area | Missing Tests |
|------|---------------|
| `PipelineContentLoader` | No integration tests against real `ContentManager` |
| `PlainFileContentLoader` | No integration tests against real file system / `GraphicsDevice` |
| `Bitmap.Load` | No tests for file loading |
| `Bitmap.Save` | No tests for file saving |
| Invalid dimensions | No tests for negative/zero width/height |
| Invalid coordinates | No tests for out-of-bounds access |
| Codec failure | No tests for unsupported image formats |
| Post-disposal access | No tests for use after `Dispose` |
| Wrapped image ownership | No tests for wrapped image state after `Bitmap.Dispose` |
| Manager pre-initialisation | No tests for load calls before `LoadContent` |
| Concurrent initialisation | No tests for concurrent `LoadContent` calls |
| Singleton identity | No tests for `Instance` returning same instance |
| Placeholder thread isolation | No tests for placeholder state across threads |

## Architecture Tests

`NuciXNA.DataAccess.ArchitectureTests` uses `TngTech.ArchUnitNET`, `TngTech.ArchUnitNET.NUnit`, and Roslyn syntax analysis. It is cross-platform and resolves repository locations from build-time assembly metadata rather than the current directory. Source files are parsed once per run and cached in `SourceFileIndex`.

| Fixture | Focus |
|---------|-------|
| `DependencyArchitectureTests` | Type-dependency isolation between `Content` and `IO`, subsystem cycle freedom, `InternalsVisibleTo` absence, no dependency on test assemblies or test frameworks, no root-namespace types, contract implementation, un-nested public types, approved dependency namespaces, project references, and approved package references |
| `ProjectConventionTests` | Target framework, implicit usings, language version, nullable references, packability, test flags, `RootNamespace` matching, package ordering, sibling project folders, no `src` nesting, `.slnx`-only solutions, solution structure and ordering |
| `ProductionCodeConventionTests` | No regions, global usings, `#nullable`, file-scoped namespaces, `var`, ternaries, `const`, `init`, `ref`/`out`, optional parameters, tuples, `== null`, `??`, empty catches, exception rethrows, partials, underscore-prefixed fields, custom exceptions; explicit accessibility; sealed classes; XML documentation; built-in aliases; braced control flow; expression-bodied methods; using placement; closing-delimiter spacing; trailing whitespace |
| `SourceFileConventionTests` | One top-level type per file, exact filename-to-type matching, block namespaces, `#nullable` absence, namespace spacing, alphabetised using groups, no inline fully qualified types, `I`-prefixed interfaces |
| `TestSourceConventionTests` | No nested test types, `Tests`-suffixed sealed fixtures, `Given_When_Then` test names, at least two `TestCase` values, no legacy assertion API, no reflection |

Failures identify the violated boundary or convention and the offending file, project, or line.

## Test Design Principles

1. **Unit tests only** — No integration tests against real MonoGame or file system
2. **Mock external dependencies** — `IContentLoader` mocked via Moq
3. **Test behaviour, not implementation** — Focus on observable outcomes
4. **Exhaustive exception testing** — `ContentLoaderTests` covers 5 exception types × 3 asset types
5. **Path forwarding verified** — Spy loader captures and asserts path equality

## Related Documentation

- [Content Loading Component](../components/content-loading.md)
- [Bitmap Access Component](../components/bitmap-access.md)
- [Error Handling](../error-handling.md)
- [Invariants](../invariants.md)