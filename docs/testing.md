# Testing

## Overview

`NuciXNA.DataAccess.UnitTests` is an NUnit test project with 256 discovered test cases covering the content loading and bitmap subsystems.

## Test Organisation

```
NuciXNA.DataAccess.UnitTests/
├── Content/
│   ├── ContentLoaderTests.cs      # ContentLoader base class behaviour
│   └── NuciContentManagerTests.cs # Manager orchestration and policy
└── IO/
    └── BitmapTests.cs             # Bitmap construction, pixel access, disposal
```

## Test Framework

| Component | Version |
|-----------|---------|
| NUnit | 4.6.1 |
| NUnit3TestAdapter | 6.2.0 |
| Microsoft.NET.Test.Sdk | 18.8.1 |
| Moq | 4.20.72 |

## Running Tests

```bash
dotnet test NuciXNA.DataAccess.sln
```

Or for the test project only:
```bash
dotnet test NuciXNA.DataAccess.UnitTests/NuciXNA.DataAccess.UnitTests.csproj
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