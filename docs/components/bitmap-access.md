# Bitmap Access Component

## Purpose

The bitmap access subsystem provides a mutable, pixel-addressable façade over ImageSharp's `Image<Rgba32>`, exposing pixel operations through `NuciXNA.Primitives` value types (`Colour`, `Point2D`, `Size2D`).

## Scope

**Owns:**
- Pixel-level read/write API (`GetPixel`, `SetPixel`, indexers)
- Image loading and saving via ImageSharp
- Construction from dimensions, `Size2D`, or existing `Image<Rgba32>`
- Disposal of the wrapped image

**Does not own:**
- ImageSharp's codec selection, memory allocation, or coordinate validation
- File system access control or path validation
- Image processing operations beyond pixel access (no filters, transforms, compositing)
- Thread safety

## Location

| File | Role |
|------|------|
| [`Bitmap.cs`](../NuciXNA.DataAccess/IO/Bitmap.cs) | Complete implementation |

## Entry Points

1. **`new Bitmap(int width, int height)`** — Create blank bitmap
2. **`new Bitmap(Size2D size)`** — Create blank bitmap from `Size2D`
3. **`new Bitmap(Image<Rgba32> sourceImage)`** — Wrap existing ImageSharp image
4. **`Bitmap.Load(string fileName)`** — Load from file (static factory)
5. **`bitmap.Save(string fileName)`** — Save to file
6. **`bitmap.GetPixel(x, y)` / `bitmap[x, y]`** — Read pixel
7. **`bitmap.SetPixel(x, y, colour)` / `bitmap[x, y] = colour`** — Write pixel
8. **`bitmap.Dispose()`** — Release resources

## Execution

### Construction

| Constructor | ImageSharp Operation | Ownership |
|-------------|---------------------|-----------|
| `Bitmap(int, int)` | `new Image<Rgba32>(width, height)` | Bitmap owns the image |
| `Bitmap(Size2D)` | Delegates to `Bitmap(int, int)` | Bitmap owns the image |
| `Bitmap(Image<Rgba32>)` | None (wraps existing) | Caller owns, but `Bitmap.Dispose()` disposes it |
| `Bitmap.Load(string)` | `Image.Load<Rgba32>(fileName)` | Bitmap owns the image |

### Pixel Access

**GetPixel / indexer get:**
```csharp
Rgba32 pixel = sourceImage[x, y];
return Colour.FromArgb(pixel.A, pixel.R, pixel.G, pixel.B);
```

**SetPixel / indexer set:**
```csharp
sourceImage[x, y] = new Rgba32(colour.R, colour.G, colour.B, colour.A);
```

**Coordinate system:** Zero-based, origin at top-left. Delegated directly to ImageSharp.

**Bounds checking:** Delegated to ImageSharp; throws `IndexOutOfRangeException` for out-of-bounds coordinates.

### Point2D Overloads

All coordinate-based methods have `Point2D` overloads that delegate to the integer-coordinate versions:
- `GetPixel(Point2D)` → `GetPixel(point.X, point.Y)`
- `SetPixel(Point2D, Colour)` → `SetPixel(point.X, point.Y, colour)`
- `this[Point2D]` → `this[point.X, point.Y]`

### Save

```csharp
public void Save(string fileName) => sourceImage.Save(fileName);
```

Format is inferred by ImageSharp from the file extension.

### Dispose

```csharp
public void Dispose() => sourceImage.Dispose();
```

Disposes the wrapped `Image<Rgba32>`. Idempotent (safe to call multiple times).

## Data

| Data | Representation | Lifecycle |
|------|----------------|-----------|
| Pixel buffer | `Image<Rgba32>` (in-memory) | Allocated on construction, disposed on `Dispose()` |
| Dimensions | `Size2D` (immutable after construction) | Set from image dimensions |
| File paths | `string` (caller-provided) | Used for load/save, not retained |

## State

- `sourceImage` — `Image<Rgba32>`, mutable pixel buffer
- `Size` — `Size2D`, immutable after construction

## Dependencies

| Dependency | Direction | Purpose |
|------------|-----------|---------|
| `SixLabors.ImageSharp` | Outbound | Image decoding, encoding, buffer, pixel type |
| `NuciXNA.Primitives` | Outbound | `Colour`, `Point2D`, `Size2D` value types |

## Dependants

- Host applications using pixel-level image manipulation
- Tests verifying pixel round-trips, construction, disposal

## Configuration

No configuration. Behaviour is determined by:
- ImageSharp's codec detection (from file content on load, from extension on save)
- `NuciXNA.Primitives` type definitions

## Failures

| Failure Point | Behaviour |
|---------------|-----------|
| `Image.Load` fails | ImageSharp exceptions propagate (`FileNotFoundException`, `UnknownImageFormatException`, `IOException`, etc.) |
| `Image.Save` fails | ImageSharp exceptions propagate (`IOException`, `InvalidImageContentException`, etc.) |
| Out-of-bounds coordinates | `IndexOutOfRangeException` from ImageSharp |
| Use after `Dispose` | Undefined behaviour (ImageSharp throws `ObjectDisposedException`) |
| Invalid dimensions (negative, zero) | ImageSharp behaviour (typically throws) |

## Invariants

1. **Channel ordering:** `Colour.FromArgb(A, R, G, B)` ↔ `Rgba32(R, G, B, A)` round-trip preserves all four channels exactly
2. **Default pixel value:** New bitmaps have all pixels set to transparent black (`Colour.FromArgb(0, 0, 0, 0)`)
3. **Size immutability:** `Size` property never changes after construction
4. **Disposal boundary:** `Bitmap.Dispose()` disposes the wrapped `Image<Rgba32>`; caller must not use bitmap after disposal
5. **Wrapped image ownership:** `Bitmap(Image<Rgba32>)` does not clone; `Dispose()` disposes the caller's image

## Tests

| Test File | Coverage |
|-----------|----------|
| [`BitmapTests.cs`](../NuciXNA.DataAccess.UnitTests/IO/BitmapTests.cs) | Construction dimensions, pixel round-trips (coordinates and `Point2D`), indexer equivalence, disposal without exception |

**Coverage gaps:**
- No tests for `Bitmap.Load`, `Bitmap.Save`
- No tests for invalid dimensions, invalid coordinates
- No tests for codec failure, post-disposal access
- No tests for ownership behaviour of wrapped image after `Dispose`

## Integration

The bitmap subsystem is independent of the content loading subsystem. It integrates with:
- ImageSharp for all image mechanics
- `NuciXNA.Primitives` for public value types

## Modification Impact

Changes to this component may require updates to:
- `BitmapTests.cs` — if pixel access, construction, or disposal behaviour changes
- `ARCHITECTURE.md` — if architectural boundaries change
- `docs/flows/manipulate-bitmap.md` — if flow changes
- `docs/api-reference/bitmap.md` — if API changes