# Invariants

## System-Wide Invariants

### 1. Loader Ordering Invariant

**Statement:** The pipeline loader is always attempted before the plain-file loader for `SoundEffect` and `Texture2D` assets.

**Enforced by:** `NuciContentManager.LoadSoundEffect` and `NuciContentManager.LoadTexture2D`

**Verified by:** `NuciContentManagerTests.cs` — tests verify pipeline `TryLoad` is called before plain-file `Load`/`TryLoad`

### 2. Font Exclusivity Invariant

**Statement:** `SpriteFont` is loaded exclusively from the pipeline loader. No fallback to plain-file loader is attempted.

**Enforced by:** `NuciContentManager.LoadSpriteFont` — calls only `pipelineContentLoader.LoadSpriteFont`

**Verified by:** `NuciContentManagerTests.cs` — tests verify plain-file loader is never called for fonts

### 3. Placeholder Invariant

**Statement:** When `MissingTexturePlaceholder` is set (non-null, non-empty, non-whitespace), the texture fallback policy changes:
- Plain-file fallback uses `TryLoad` (optional) instead of `Load` (required)
- If both optional loads return `null`, the placeholder is loaded from the pipeline via required `Load`

**Enforced by:** `NuciContentManager.LoadTexture2D`

**Verified by:** `NuciContentManagerTests.cs` — tests for placeholder enabled, disabled, empty, and whitespace

### 4. Bitmap Channel Ordering Invariant

**Statement:** `Colour` ARGB values round-trip through ImageSharp `Rgba32` without loss:
- `GetPixel`: `Rgba32(R, G, B, A)` → `Colour.FromArgb(A, R, G, B)`
- `SetPixel`: `Colour(A, R, G, B)` → `Rgba32(R, G, B, A)`

**Enforced by:** `Bitmap.GetPixel` and `Bitmap.SetPixel`

**Verified by:** `BitmapTests.cs` — round-trip tests for multiple colours

### 5. Bitmap Disposal Invariant

**Statement:** `Bitmap.Dispose()` disposes the wrapped `Image<Rgba32>`. The bitmap must not be used after disposal.

**Enforced by:** `Bitmap.Dispose()` → `sourceImage.Dispose()`

**Verified by:** `BitmapTests.cs` — disposal without exception tests

### 6. Path Pass-Through Invariant

**Statement:** Content paths are forwarded to loaders without normalisation (no trimming, case adjustment, or separator conversion).

**Enforced by:** `NuciContentManager` — passes `contentPath` directly to loader methods

**Verified by:** `NuciContentManagerTests.cs` — path forwarding tests with various path formats

### 7. TryLoad Exception Suppression Invariant

**Statement:** `ContentLoader.TryLoad<T>` catches **all** exceptions (including `OutOfMemoryException`, `NullReferenceException`) and returns `null`.

**Enforced by:** `ContentLoader.TryLoad<T>` — bare `catch` block

**Verified by:** `ContentLoaderTests.cs` — tests for IOException, ArgumentException, InvalidOperationException, OutOfMemoryException, NullReferenceException

### 8. Default Pixel Value Invariant

**Statement:** New bitmaps created from dimensions or `Size2D` have all pixels initialised to transparent black (`Colour.FromArgb(0, 0, 0, 0)`).

**Enforced by:** ImageSharp `new Image<Rgba32>(width, height)` initialises to zero

**Verified by:** `BitmapTests.cs` — `GetPixel` on new bitmap returns transparent black

### 9. Size Immutability Invariant

**Statement:** `Bitmap.Size` is set once during construction and never changes.

**Enforced by:** `Size` is a read-only auto-property

**Verified by:** `BitmapTests.cs` — size matches constructor arguments

### 10. Singleton Identity Invariant

**Statement:** `NuciContentManager.Instance` returns the same instance for all calls within a process.

**Enforced by:** `volatile` double-checked locking with `Lock`

**Verified by:** Not directly tested (coverage gap)

## Contract Invariants

### IContentLoader Contract

| Method | Contract |
|--------|----------|
| `LoadSoundEffect` | Required load — may throw |
| `TryLoadSoundEffect` | Optional load — returns `null` on failure |
| `LoadSpriteFont` | Required load — may throw |
| `TryLoadSpriteFont` | Optional load — returns `null` on failure |
| `LoadTexture2D` | Required load — may throw |
| `TryLoadTexture2D` | Optional load — returns `null` on failure |

### ContentLoader Base Class

| Method | Contract |
|--------|----------|
| `TryLoad<T>` | Catches all exceptions, returns `null` |
| `Load*` (abstract) | Must be implemented by derived classes |

### PlainFileContentLoader Conventions

| Asset Type | File Suffix | Loader Method |
|------------|-------------|---------------|
| `SoundEffect` | `.wav` | `LoadSoundEffect` |
| `Texture2D` | `.png` | `LoadTexture2D` |
| `SpriteFont` | N/A | `LoadSpriteFont` throws `NotImplementedException` |

## Related Documentation

- [Architecture](./architecture.md)
- [Error Handling](./error-handling.md)
- [Content Loading Component](./components/content-loading.md)
- [Bitmap Access Component](./components/bitmap-access.md)
- [Testing](./testing.md)