# Bitmap

**Namespace:** `NuciXNA.DataAccess.IO`
**File:** [`Bitmap.cs`](../NuciXNA.DataAccess/IO/Bitmap.cs)

## Overview

`Bitmap` is a mutable, pixel-addressable façade over an ImageSharp `Image<Rgba32>`. It provides pixel-level read/write access using `NuciXNA.Primitives` value types (`Colour`, `Point2D`, `Size2D`), and delegates image loading, encoding, and resource management to ImageSharp.

## Constructors

### `Bitmap(Image<Rgba32>)`

```csharp
public Bitmap(Image<Rgba32> sourceImage)
```

**Description:** Wraps an existing ImageSharp image. The bitmap does **not** take ownership of the image's lifetime — the caller remains responsible for the image, but `Bitmap.Dispose()` will dispose the wrapped image.

**Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `sourceImage` | `Image<Rgba32>` | The source image to wrap. |

**Side effects:** Stores reference to `sourceImage`. `Size` is set from image dimensions.

**Warning:** After `Bitmap.Dispose()`, the wrapped `Image<Rgba32>` is disposed and must not be used.

### `Bitmap(int, int)`

```csharp
public Bitmap(int width, int height)
```

**Description:** Creates a new blank bitmap with all pixels set to transparent black (`Colour.FromArgb(0, 0, 0, 0)`).

**Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `width` | `int` | The width of the bitmap in pixels. |
| `height` | `int` | The height of the bitmap in pixels. |

**Side effects:** Allocates a new `Image<Rgba32>` internally. `Size` is set to `(width, height)`.

### `Bitmap(Size2D)`

```csharp
public Bitmap(Size2D size) : this(size.Width, size.Height)
```

**Description:** Creates a new blank bitmap with the specified dimensions.

**Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `size` | `Size2D` | The dimensions of the bitmap in pixels. |

### `Bitmap(string)` (private, used by `Load`)

```csharp
private Bitmap(string fileName)
```

**Description:** Loads an image from disk using ImageSharp. Used internally by `Bitmap.Load`.

**Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `fileName` | `string` | The path to the image file to load. |

**Side effects:** Allocates a new `Image<Rgba32>` by decoding the file. `Size` is set from image dimensions.

## Properties

### `Size`

```csharp
public Size2D Size { get; }
```

**Description:** Gets the dimensions of the bitmap. Immutable after construction.

**Returns:** A `Size2D` with `Width` and `Height` matching the underlying image.

## Indexers

### `this[int x, int y]`

```csharp
public Colour this[int x, int y] { get; set; }
```

**Description:** Gets or sets the pixel colour at the specified zero-based coordinates.

**Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `x` | `int` | The zero-based horizontal coordinate. |
| `y` | `int` | The zero-based vertical coordinate. |

**Get:** Returns `Colour.FromArgb(pixel.A, pixel.R, pixel.G, pixel.B)` from the underlying `Rgba32`.

**Set:** Assigns `new Rgba32(colour.R, colour.G, colour.B, colour.A)` to the underlying image.

**Bounds:** Delegates directly to ImageSharp; out-of-bounds access throws `IndexOutOfRangeException`.

### `this[Point2D point]`

```csharp
public Colour this[Point2D point] { get; set; }
```

**Description:** Gets or sets the pixel colour at the specified `Point2D` location.

**Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `point` | `Point2D` | The zero-based pixel location. |

**Behaviour:** Delegates to `this[int, int]` with `point.X` and `point.Y`.

## Methods

### `GetPixel(int x, int y)`

```csharp
public Colour GetPixel(int x, int y)
```

**Description:** Returns the colour of the pixel at the specified coordinates.

**Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `x` | `int` | The zero-based horizontal coordinate. |
| `y` | `int` | The zero-based vertical coordinate. |

**Returns:** `Colour.FromArgb(pixel.A, pixel.R, pixel.G, pixel.B)`.

**Throws:** `IndexOutOfRangeException` if coordinates are outside image bounds (delegated to ImageSharp).

### `GetPixel(Point2D location)`

```csharp
public Colour GetPixel(Point2D location)
```

**Description:** Returns the colour of the pixel at the specified point.

**Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `location` | `Point2D` | The zero-based pixel location. |

**Returns:** `GetPixel(location.X, location.Y)`.

### `SetPixel(int x, int y, Colour colour)`

```csharp
public void SetPixel(int x, int y, Colour colour)
```

**Description:** Sets the colour of the pixel at the specified coordinates.

**Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `x` | `int` | The zero-based horizontal coordinate. |
| `y` | `int` | The zero-based vertical coordinate. |
| `colour` | `Colour` | The colour to assign to the pixel. |

**Behaviour:** Assigns `sourceImage[x, y] = new Rgba32(colour.R, colour.G, colour.B, colour.A)`.

**Throws:** `IndexOutOfRangeException` if coordinates are outside image bounds.

### `SetPixel(Point2D location, Colour colour)`

```csharp
public void SetPixel(Point2D location, Colour colour)
```

**Description:** Sets the colour of the pixel at the specified point.

**Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `location` | `Point2D` | The zero-based pixel location. |
| `colour` | `Colour` | The colour to assign to the pixel. |

**Behaviour:** Delegates to `SetPixel(location.X, location.Y, colour)`.

### `Load(string fileName)`

```csharp
public static Bitmap Load(string fileName)
```

**Description:** Loads a `Bitmap` from the specified image file on disk.

**Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `fileName` | `string` | The path to the image file to load. |

**Returns:** A new `Bitmap` containing the loaded image data.

**Behaviour:** Calls `new Bitmap(fileName)` which uses `Image.Load<Rgba32>(fileName)`. Format is inferred by ImageSharp from file content.

**Throws:** `FileNotFoundException`, `UnknownImageFormatException`, `IOException`, etc. (ImageSharp exceptions).

### `Save(string fileName)`

```csharp
public void Save(string fileName)
```

**Description:** Saves the bitmap to the specified file path on disk.

**Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `fileName` | `string` | The destination file path. The format is inferred from the file extension. |

**Behaviour:** Calls `sourceImage.Save(fileName)`. Encoding is delegated to ImageSharp based on extension.

**Throws:** `IOException`, `InvalidImageContentException`, etc. (ImageSharp exceptions).

### `Dispose()`

```csharp
public void Dispose()
```

**Description:** Releases the resources held by the underlying image.

**Behaviour:** Calls `sourceImage.Dispose()`.

**Post-condition:** The bitmap must not be used after disposal. The wrapped `Image<Rgba32>` is disposed.

**Idempotency:** Safe to call multiple times (ImageSharp's `Dispose` is idempotent).

## Channel Ordering

| Direction | Transformation |
|-----------|----------------|
| `GetPixel` / indexer get | `Rgba32(R, G, B, A)` → `Colour.FromArgb(A, R, G, B)` |
| `SetPixel` / indexer set | `Colour(A, R, G, B)` → `Rgba32(R, G, B, A)` |

**Invariant:** Round-trip `GetPixel` → `SetPixel` preserves all four channels exactly.

## Default Pixel Value

New bitmaps (constructed from dimensions or `Size2D`) have all pixels initialised to **transparent black**: `Colour.FromArgb(0, 0, 0, 0)`.

## Thread Safety

`Bitmap` is **not thread-safe**. Concurrent pixel access from multiple threads without external synchronisation results in undefined behaviour.

## Related Documentation

- [Bitmap Access Component](./components/bitmap-access.md)
- [Manipulate Bitmap Flow](./flows/manipulate-bitmap.md)
- [NuciXNA.Primitives Types](https://github.com/hmlendea/nucixna.primitives) — `Colour`, `Point2D`, `Size2D`
- [ImageSharp Documentation](https://docs.sixlabors.com/articles/imagesharp.html)