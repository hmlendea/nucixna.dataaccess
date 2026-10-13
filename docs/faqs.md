# Frequently Asked Questions

## General

### What is NuciXNA.DataAccess?

A .NET 10 library providing content loading and bitmap manipulation utilities for MonoGame/XNA applications. It wraps MonoGame's content pipeline and ImageSharp with a unified API.

### What .NET versions are supported?

Target framework is `net10.0`. The library uses .NET 10 features and dependencies.

### What MonoGame version is required?

`MonoGame.Framework.DesktopGL` 3.8.4 or compatible.

### Is this a game engine?

No. It's a data access library for asset loading and pixel manipulation. It has no game loop, scene graph, or entity system.

## Content Loading

### Why does LoadTexture2D have three fallback stages?

When `MissingTexturePlaceholder` is configured:
1. Pipeline loader (compiled `.xnb`)
2. Plain-file loader (raw `.png`)
3. Placeholder pipeline load (configured path)

Without placeholder: stages 1 and 2 only. This prevents crashes during development when assets are missing.

### Can I load SpriteFont from a raw file?

No. `PlainFileContentLoader.LoadSpriteFont` throws `NotImplementedException`. Fonts must be compiled through the MonoGame content pipeline.

### What audio formats are supported?

Pipeline: whatever MonoGame's content pipeline supports (typically `.xnb` compiled from `.wav`, `.mp3`, `.ogg`).
Plain-file: `.wav` only (appended automatically).

### What image formats are supported for textures?

Pipeline: whatever MonoGame's content pipeline supports.
Plain-file: `.png` only (appended automatically).
Bitmap.Load/Save: whatever ImageSharp supports (PNG, JPEG, BMP, GIF, TIFF, etc.).

### Why does TryLoad catch ALL exceptions?

Design decision to make optional loads truly optional — any failure (including `OutOfMemoryException`, `NullReferenceException`) returns `null`. See [Design Decisions](./design-decisions.md#1-loader-interface-with-base-optional-load-wrapper).

### Can I use my own IContentLoader implementation?

Yes. Pass custom loaders to `NuciContentManager.Instance.LoadContent(pipelineLoader, plainFileLoader)`.

### Is NuciContentManager thread-safe?

Only singleton instance creation is thread-safe. All load operations and configuration are not thread-safe. See [Concurrency and Scheduling](./concurrency-and-scheduling.md).

### How do I reset or reconfigure the manager?

You cannot. `NuciContentManager.Instance` is a singleton with no reset or disposal. Reinitialisation requires process restart.

## Bitmap

### What pixel format does Bitmap use?

Internally: ImageSharp `Rgba32` (RGBA byte order).
API: `NuciXNA.Primitives.Colour` (ARGB component order).
Conversion is automatic in `GetPixel`/`SetPixel`.

### Can I use Bitmap across threads?

No. `Bitmap` is not thread-safe. Don't share instances across threads.

### What happens if I use a Bitmap after Dispose?

`ObjectDisposedException`. The underlying `Image<Rgba32>` is disposed.

### Can I create a Bitmap from a Texture2D?

Not directly. Copy pixel data manually:
```csharp
Color[] data = new Color[texture.Width * texture.Height];
texture.GetData(data);
// Convert to Bitmap pixel by pixel
```

### Does Bitmap support image processing (filters, transforms)?

No. Only pixel-level read/write, load, and save. Use ImageSharp directly for processing.

## Testing

### What test framework is used?

NUnit 4.6.1 with NUnit3TestAdapter.

### How do I run tests?

```bash
dotnet test NuciXNA.DataAccess.UnitTests/NuciXNA.DataAccess.UnitTests.csproj
```

### Are there integration tests?

No. Only unit tests with mocked dependencies.

## Deployment

### How do I package this as a NuGet package?

```bash
dotnet pack NuciXNA.DataAccess/NuciXNA.DataAccess.csproj -c Release
```

### What is the license?

GPL-3.0-or-later (see `LICENSE` file).

## Related Documentation

- [Troubleshooting](./troubleshooting.md)
- [Design Decisions](./design-decisions.md)
- [Concurrency and Scheduling](./concurrency-and-scheduling.md)
- [Error Handling](./error-handling.md)