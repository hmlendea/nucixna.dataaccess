# Repository Overview

## Purpose

`NuciXNA.DataAccess` is a .NET 10 library providing data access utilities for the NuciXNA ecosystem, built on top of MonoGame/XNA. It serves two primary concerns:

1. **Content loading** — Resolves MonoGame assets through compiled content-pipeline and raw-file loaders with automatic fallback
2. **Bitmap manipulation** — Exposes mutable pixel access over ImageSharp images using `NuciXNA.Primitives` value types

## Scope

### In Scope

- `SoundEffect`, `SpriteFont`, and `Texture2D` asset loading via MonoGame content pipeline
- Raw-file fallback for `.wav` sounds and `.png` textures
- Configurable missing-texture placeholder substitution
- Custom `IContentLoader` injection for testing and custom asset sources
- Pixel-level image manipulation (read, write, load, save) via ImageSharp

### Out of Scope

- Application loop or game state management
- Content root or asset cache management
- Dependency injection container
- Process lifecycle
- Network operations or remote asset loading
- Image processing beyond pixel access (no filters, transforms, compositing)
- Font loading from raw files (pipeline-only)

## Entry Points

### Public API

| Class | Namespace | Purpose |
|-------|-----------|---------|
| `NuciContentManager` | `NuciXNA.DataAccess.Content` | Singleton façade for content loading |
| `IContentLoader` | `NuciXNA.DataAccess.Content` | Contract for asset loading |
| `ContentLoader` | `NuciXNA.DataAccess.Content` | Abstract base with `TryLoad*` wrappers |
| `PipelineContentLoader` | `NuciXNA.DataAccess.Content` | MonoGame pipeline adapter |
| `PlainFileContentLoader` | `NuciXNA.DataAccess.Content` | Raw-file adapter |
| `Bitmap` | `NuciXNA.DataAccess.IO` | ImageSharp façade for pixel access |

### Initialisation

```csharp
// Standard initialisation
NuciContentManager.Instance.LoadContent(Content, GraphicsDevice);

// Custom loaders
NuciContentManager.Instance.LoadContent(pipelineLoader, plainFileLoader);
```

### Usage

```csharp
// Load assets
Texture2D texture = NuciContentManager.Instance.LoadTexture2D("Textures/player");
SoundEffect sound = NuciContentManager.Instance.LoadSoundEffect("Audio/click");
SpriteFont font = NuciContentManager.Instance.LoadSpriteFont("Fonts/UiFont");

// Manipulate bitmaps
using Bitmap image = Bitmap.Load("input.png");
Colour pixel = image[10, 20];
image[10, 20] = Colour.FromArgb(255, 255, 0, 0);
image.Save("output.png");
```

## Principal Capabilities

1. **Dual-source asset loading** — Pipeline first, plain-file fallback
2. **Placeholder substitution** — Configurable missing-texture fallback
3. **Loader injection** — Custom `IContentLoader` implementations
4. **Pixel-level bitmap access** — Read/write individual pixels via `Colour` type
5. **Image load/save** — Decode and encode images via ImageSharp

## Component Relationships

```
Host Application
    │
    ├── NuciContentManager (singleton)
    │       ├── IContentLoader (pipeline) → PipelineContentLoader → ContentManager
    │       └── IContentLoader (plain-file) → PlainFileContentLoader → FileSystem
    │
    └── Bitmap → ImageSharp.Image<Rgba32> ↔ NuciXNA.Primitives
```

## Major Integrations

| Integration | Direction | Purpose |
|-------------|-----------|---------|
| MonoGame | Outbound | Content pipeline, graphics, audio |
| ImageSharp | Outbound | Image decoding, encoding, pixel buffer |
| NuciXNA.Primitives | Outbound | Colour, Point2D, Size2D value types |
| Host file system | Outbound | Raw `.wav` and `.png` file access |

## System-Wide Invariants

1. Pipeline loader always attempted before plain-file loader
2. `SpriteFont` is pipeline-only (no fallback)
3. `TryLoad*` catches all exceptions and returns `null`
4. `Colour` ARGB ↔ `Rgba32` RGBA channel ordering preserved
5. `Bitmap.Dispose()` disposes wrapped `Image<Rgba32>`
6. Paths forwarded without normalisation

## Related Documentation

- [Root ARCHITECTURE.md](../ARCHITECTURE.md) — Verified current architecture
- [Root README.md](../README.md) — Installation and usage
- [Architecture](./architecture.md) — High-level design
- [Repository Structure](./repository-structure.md) — Source tree layout
- [Quick Start](./quick-start.md) — Getting started guide