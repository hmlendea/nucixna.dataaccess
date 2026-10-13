# Integrations

## External System Integrations

### MonoGame Framework

| Aspect | Details |
|--------|---------|
| **Package** | `MonoGame.Framework.DesktopGL` 3.8.4 |
| **Direction** | Outbound |
| **Usage** | Content pipeline (`ContentManager.Load<T>`), `GraphicsDevice`, `Texture2D`, `SoundEffect`, `SpriteFont` |
| **Coupling** | Hard dependency — library cannot function without MonoGame |
| **Version Sensitivity** | API stable across 3.x; tested with 3.8.4 |

**Integration Points:**
- `PipelineContentLoader` wraps `ContentManager`
- `NuciContentManager.LoadContent(ContentManager, GraphicsDevice)` accepts MonoGame instances
- `PlainFileContentLoader` uses `Texture2D.FromStream` and `SoundEffect.FromStream`

### ImageSharp

| Aspect | Details |
|--------|---------|
| **Package** | `SixLabors.ImageSharp` 3.1.6 |
| **Direction** | Outbound |
| **Usage** | Image decoding, encoding, pixel buffer (`Image<Rgba32>`), format detection |
| **Coupling** | Hard dependency — `Bitmap` wraps `Image<Rgba32>` |
| **Version Sensitivity** | Major versions may change API; 3.x tested |

**Integration Points:**
- `Bitmap` constructors wrap `Image<Rgba32>`
- `Bitmap.Load` uses `Image.Load<Rgba32>`
- `Bitmap.Save` uses `Image.Save` with format inference
- Pixel access maps `Rgba32` ↔ `Colour`

### NuciXNA.Primitives

| Aspect | Details |
|--------|---------|
| **Package** | `NuciXNA.Primitives` 2.1.7 |
| **Direction** | Outbound |
| **Usage** | `Colour` (ARGB), `Point2D`, `Size2D` value types |
| **Coupling** | Hard dependency — public API uses these types |
| **Version Sensitivity** | Stable value types; minimal breaking change risk |

**Integration Points:**
- `Bitmap.GetPixel`/`SetPixel` use `Colour`
- `Bitmap.Size` returns `Size2D`
- `Bitmap` indexers use `Point2D` implicitly

### Host File System

| Aspect | Details |
|--------|---------|
| **Direction** | Outbound |
| **Usage** | Raw `.wav` and `.png` file access via `PlainFileContentLoader` |
| **Coupling** | Implicit — paths passed through without abstraction |
| **Platform Sensitivity** | Path separators, case sensitivity, permissions vary by OS |

**Integration Points:**
- `PlainFileContentLoader` constructor accepts content root path
- `File.OpenRead` for `.wav` and `.png` files
- No path normalisation — host must provide correct paths

## Internal Integrations

### Content Loading Subsystem

```
NuciContentManager (façade)
    ├── IContentLoader (contract)
    │       ├── PipelineContentLoader → ContentManager
    │       └── PlainFileContentLoader → FileSystem
    └── MissingTexturePlaceholder (policy)
```

### Bitmap Subsystem

```
Bitmap (façade)
    └── Image<Rgba32> (ImageSharp)
            ├── Pixel buffer (Rgba32)
            ├── Codecs (PNG, JPEG, BMP, etc.)
            └── Disposal
```

## Integration Patterns

### Adapter Pattern

Both subsystems use adapter pattern:
- `IContentLoader` adapts MonoGame `ContentManager` and raw file access
- `Bitmap` adapts ImageSharp `Image<Rgba32>`

### Façade Pattern

- `NuciContentManager` façades dual-loader orchestration
- `Bitmap` façades ImageSharp operations

### Dependency Injection (Manual)

Host injects dependencies via:
```csharp
NuciContentManager.Instance.LoadContent(pipelineLoader, plainFileLoader);
```

No DI container — manual composition.

## Version Compatibility Matrix

| NuciXNA.DataAccess | MonoGame | ImageSharp | NuciXNA.Primitives | .NET |
|-------------------|----------|------------|-------------------|------|
| 4.0.4 | 3.8.4 | 3.1.6 | 2.1.7 | 10.0 |

## Breaking Change Risks

| Dependency | Risk | Mitigation |
|------------|------|------------|
| MonoGame | Low (stable 3.x API) | Pin version in csproj |
| ImageSharp | Medium (major versions break) | Pin major version; test on upgrade |
| NuciXNA.Primitives | Low (value types) | Pin version |
| .NET 10 | Low (LTS) | Target framework fixed |

## Testing Integrations

Unit tests mock external dependencies:
- `Moq` for `IContentLoader`, `ContentManager`, `GraphicsDevice`
- Real `Image<Rgba32>` for `Bitmap` tests (no mock — tests actual behaviour)
- No file system access in tests (uses `MemoryStream`)

## Related Documentation

- [Dependencies](./dependencies.md)
- [Architecture](./architecture.md)
- [Components: Content Loading](../components/content-loading.md)
- [Components: Bitmap Access](../components/bitmap-access.md)
- [Build and Deployment](./build-and-deployment.md)