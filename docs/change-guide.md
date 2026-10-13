# Change Guide

## Overview

This guide documents how to safely modify common areas of the codebase.

## Modifying Content Loading Policy

### Changing Fallback Order

**Files to modify:**
- `NuciContentManager.cs` — `LoadSoundEffect`, `LoadTexture2D` methods
- `NuciContentManagerTests.cs` — Update tests for new order

**Steps:**
1. Update the method logic in `NuciContentManager.cs`
2. Update corresponding tests in `NuciContentManagerTests.cs`
3. Update `ARCHITECTURE.md` and `docs/flows/load-asset.md`
4. Run tests: `dotnet test`

### Adding New Asset Type

**Files to modify:**
- `IContentLoader.cs` — Add `LoadNewType` and `TryLoadNewType`
- `ContentLoader.cs` — Add abstract `LoadNewType` and `TryLoadNewType` wrapper
- `PipelineContentLoader.cs` — Implement `LoadNewType`
- `PlainFileContentLoader.cs` — Implement `LoadNewType` (or throw `NotImplementedException`)
- `NuciContentManager.cs` — Add `LoadNewType` method with policy
- `NuciContentManagerTests.cs` — Add tests for new type

**Steps:**
1. Add to interface and base class
2. Implement in both adapters
3. Add manager method with fallback policy
4. Add comprehensive tests
5. Update documentation

### Modifying Placeholder Policy

**Files to modify:**
- `NuciContentManager.cs` — `LoadTexture2D` method
- `NuciContentManagerTests.cs` — Placeholder tests

**Steps:**
1. Update `LoadTexture2D` logic
2. Update placeholder tests
3. Update `docs/flows/load-asset.md` and `docs/configuration.md`

## Modifying Bitmap Behaviour

### Changing Channel Ordering

**Files to modify:**
- `Bitmap.cs` — `GetPixel` and `SetPixel` methods
- `BitmapTests.cs` — Round-trip tests

**⚠️ Breaking change:** This affects all callers. Consider:
- Adding new methods instead of changing existing
- Version bump (major)
- Migration guide

### Adding Image Processing Operations

**Files to modify:**
- `Bitmap.cs` — Add new methods
- `BitmapTests.cs` — Add tests

**Guidelines:**
- Keep `Bitmap` as a thin façade over ImageSharp
- Delegate to ImageSharp where possible
- Maintain `Colour`/`Point2D`/`Size2D` API

### Changing Disposal Behaviour

**Files to modify:**
- `Bitmap.cs` — `Dispose` method
- `BitmapTests.cs` — Disposal tests

**⚠️ Breaking change:** Affects resource lifetime. Ensure:
- `IDisposable` contract maintained
- Wrapped image ownership documented
- Tests cover ownership transfer

## Modifying Configuration

### Adding New Configuration Option

**Files to modify:**
- `NuciContentManager.cs` — Add static property or method parameter
- `NuciContentManagerTests.cs` — Test new option
- `docs/configuration.md` — Document new option

**Guidelines:**
- Keep configuration programmatic (no config files)
- Document scope (process-wide vs per-instance)
- Document override behaviour

## Adding Tests

### New ContentLoader Test

**File:** `ContentLoaderTests.cs`

**Pattern:**
```csharp
[TestCase("path1")]
[TestCase("path2")]
public void GivenLoadThrowsExceptionType_WhenTryLoadIsCalled_ThenNullIsReturned(string path)
{
    var loader = new ThrowingContentLoader(new ExceptionType("message"));
    Assert.That(loader.TryLoadSoundEffect(path), Is.Null);
}
```

### New Manager Test

**File:** `NuciContentManagerTests.cs`

**Pattern:**
```csharp
[Test]
public void GivenCondition_WhenMethodIsCalled_ThenExpectedBehaviour()
{
    // Arrange
    pipelineContentLoaderMock.Setup(...);
    plainFileContentLoaderMock.Setup(...);

    // Act
    var result = NuciContentManager.Instance.LoadTexture2D("path");

    // Assert
    Assert.That(result, ...);
    pipelineContentLoaderMock.Verify(...);
}
```

### New Bitmap Test

**File:** `BitmapTests.cs`

**Pattern:**
```csharp
[TestCase(10, 20)]
public void GivenCondition_WhenOperation_ThenExpectedResult(int width, int height)
{
    subject.Dispose();
    subject = new Bitmap(width, height);
    Assert.That(subject.Size.Width, Is.EqualTo(width));
}
```

## Updating Dependencies

### Patch/Minor Updates

1. Update version in `.csproj`
2. Run `dotnet restore`
3. Run `dotnet build`
4. Run `dotnet test`
5. Check for breaking changes in release notes

### Major Updates

1. Review breaking changes
2. Update code for API changes
3. Update tests
4. Update `ARCHITECTURE.md` if architectural boundaries change
5. Consider major version bump for library

## Documentation Updates

When changing code, update:
- `ARCHITECTURE.md` (root) — if architecture changes
- `docs/architecture.md` — if architecture details change
- `docs/components/*.md` — if component behaviour changes
- `docs/flows/*.md` — if execution flows change
- `docs/api-reference/*.md` — if public API changes
- `docs/configuration.md` — if configuration changes
- `docs/error-handling.md` — if error behaviour changes
- `docs/invariants.md` — if invariants change
- `docs/testing.md` — if test coverage changes

## Versioning

**Scheme:** Semantic Versioning (MAJOR.MINOR.PATCH)

| Change Type | Version Bump |
|-------------|--------------|
| Breaking API change | MAJOR |
| New functionality (backward compatible) | MINOR |
| Bug fix (backward compatible) | PATCH |
| Dependency update (no API change) | PATCH |

**Location:** `<Version>` in `NuciXNA.DataAccess.csproj`

## Related Documentation

- [Architecture](./architecture.md)
- [Testing](./testing.md)
- [Dependencies](./dependencies.md)