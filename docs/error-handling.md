# Error Handling

## Overview

`NuciXNA.DataAccess` employs a dual error-handling strategy: **required-load boundaries** that propagate exceptions, and **optional-load boundaries** that suppress all exceptions and return `null`. The `NuciContentManager` combines these to implement fallback and placeholder policies.

## Error Taxonomy

### 1. Content Loading Errors

| Error Type | Source | Boundary | Behaviour |
|------------|--------|----------|-----------|
| `ContentLoadException` | MonoGame `ContentManager.Load<T>` | Required (`Load*`) | Propagates |
| `ContentLoadException` | MonoGame `ContentManager.Load<T>` | Optional (`TryLoad*`) | Suppressed → `null` |
| `FileNotFoundException` | `File.OpenRead` | Required (`Load*`) | Propagates |
| `FileNotFoundException` | `File.OpenRead` | Optional (`TryLoad*`) | Suppressed → `null` |
| `IOException` | File system | Required (`Load*`) | Propagates |
| `IOException` | File system | Optional (`TryLoad*`) | Suppressed → `null` |
| `ArgumentException` | MonoGame / ImageSharp | Required (`Load*`) | Propagates |
| `ArgumentException` | MonoGame / ImageSharp | Optional (`TryLoad*`) | Suppressed → `null` |
| `InvalidOperationException` | MonoGame / ImageSharp | Required (`Load*`) | Propagates |
| `InvalidOperationException` | MonoGame / ImageSharp | Optional (`TryLoad*`) | Suppressed → `null` |
| `OutOfMemoryException` | MonoGame / ImageSharp | Required (`Load*`) | Propagates |
| `OutOfMemoryException` | MonoGame / ImageSharp | Optional (`TryLoad*`) | Suppressed → `null` |
| `NullReferenceException` | Any | Required (`Load*`) | Propagates |
| `NullReferenceException` | Any | Optional (`TryLoad*`) | Suppressed → `null` |
| `NotImplementedException` | `PlainFileContentLoader.LoadSpriteFont` | Required | Propagates (never called by manager) |

### 2. Bitmap Errors

| Error Type | Source | Behaviour |
|------------|--------|-----------|
| `IndexOutOfRangeException` | ImageSharp pixel access | Propagates |
| `FileNotFoundException` | `Image.Load` | Propagates |
| `UnknownImageFormatException` | ImageSharp codec | Propagates |
| `IOException` | File system | Propagates |
| `ObjectDisposedException` | Post-disposal access | Propagates |

## Error Handling Patterns

### Pattern 1: Required-Load Boundary

**Location:** `ContentLoader.Load*` methods, `NuciContentManager.Load*` methods

**Behaviour:** Exceptions propagate directly to the caller. No catching, no translation.

**Example:**
```csharp
// PipelineContentLoader
public override SoundEffect LoadSoundEffect(string contentPath)
    => content.Load<SoundEffect>(contentPath);  // Exceptions propagate
```

### Pattern 2: Optional-Load Boundary

**Location:** `ContentLoader.TryLoad<T>` method

**Behaviour:** Catches **all** exceptions and returns `null`.

**Code:**
```csharp
private static T TryLoad<T>(Func<T> load) where T : class
{
    try
    {
        return load();
    }
    catch
    {
        return null;
    }
}
```

**Key design decision:** The catch-all (`catch` without exception type) is intentional and tested. It suppresses even `OutOfMemoryException` and `NullReferenceException`. This is observable, tested behaviour.

### Pattern 3: Fallback Composition

**Location:** `NuciContentManager.LoadSoundEffect`, `NuciContentManager.LoadTexture2D`

**Behaviour:** Combines optional and required boundaries to implement fallback:

```csharp
// SoundEffect: pipeline optional → plain-file required
SoundEffect soundEffect = pipelineContentLoader.TryLoadSoundEffect(contentPath);
soundEffect ??= plainFileContentLoader.LoadSoundEffect(contentPath);
return soundEffect;
```

### Pattern 4: Placeholder Composition

**Location:** `NuciContentManager.LoadTexture2D`

**Behaviour:** Three-stage fallback with mixed optional/required boundaries:

```csharp
// Stage 1: pipeline optional
Texture2D texture = pipelineContentLoader.TryLoadTexture2D(contentPath);

// Stage 2: plain-file optional (if placeholder enabled) or required (if disabled)
if (texture is null)
{
    if (string.IsNullOrWhiteSpace(MissingTexturePlaceholder))
        texture = plainFileContentLoader.LoadTexture2D(contentPath);  // required
    else
        texture = plainFileContentLoader.TryLoadTexture2D(contentPath);  // optional
}

// Stage 3: placeholder required (if enabled and still null)
if (texture is null && !string.IsNullOrWhiteSpace(MissingTexturePlaceholder))
    texture = pipelineContentLoader.LoadTexture2D(MissingTexturePlaceholder);  // required

return texture;
```

## Error Recovery

| Scenario | Recovery Strategy |
|----------|-------------------|
| Pipeline asset missing | Falls back to plain-file loader |
| Plain-file asset missing | Falls back to placeholder (if configured) |
| Placeholder missing | Exception propagates to caller |
| All loaders fail | Exception propagates (required loads) or `null` returned (optional loads) |
| Loaders not initialised | No recovery — `NullReferenceException` |

## Error Information Loss

**Critical limitation:** The `TryLoad*` boundary catches all exceptions and returns `null`, losing all diagnostic information. A host cannot distinguish between:
- Missing file
- Corrupt file
- Out of memory
- Null reference
- Any other exception

This is a deliberate design trade-off for simplicity of the optional-load contract.

## Logging

**No logging is implemented.** See [Logging](./logging.md) for details.

## Tests

| Test File | Coverage |
|-----------|----------|
| `ContentLoaderTests.cs` | `TryLoad*` exception suppression for IOException, ArgumentException, InvalidOperationException, OutOfMemoryException, NullReferenceException; null return handling; path forwarding |
| `NuciContentManagerTests.cs` | Loader selection, exception propagation, placeholder branches, path forwarding |
| `BitmapTests.cs` | Disposal without exception (no error-path tests) |

## Related Documentation

- [Content Loading Component](../components/content-loading.md)
- [Load Asset Flow](../flows/load-asset.md)
- [Logging](./logging.md)
- [Testing](./testing.md)