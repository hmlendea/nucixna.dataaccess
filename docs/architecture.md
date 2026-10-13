# Architecture

This document complements the root [ARCHITECTURE.md](../ARCHITECTURE.md) by elaborating on architectural concepts with implementation evidence.

## Architectural Style

The library follows an **interface-and-adapter** pattern for content loading and a **façade** pattern for bitmap access.

```
┌─────────────────────────────────────────────────────────────────┐
│                      NuciContentManager                         │
│  (singleton façade coordinating two IContentLoader instances)  │
└──────────────────────────┬──────────────────────────────────────┘
                           │
           ┌───────────────┴───────────────┐
           ▼                               ▼
    ┌───────────────┐               ┌───────────────┐
    │PipelineContent│               │PlainFileContent│
    │    Loader     │               │    Loader     │
    └───────┬───────┘               └───────┬───────┘
            │                               │
            ▼                               ▼
    ┌───────────────┐               ┌───────────────┐
    │ContentManager │               │GraphicsDevice │
    │  (MonoGame)   │               │  + FileSystem │
    └───────────────┘               └───────────────┘
```

## Component Relationships

| Component | Pattern | Dependencies | Consumers |
|-----------|---------|--------------|-----------|
| `IContentLoader` | Interface (contract) | MonoGame asset types | `ContentLoader`, `PipelineContentLoader`, `PlainFileContentLoader`, host implementations |
| `ContentLoader` | Abstract base (template method) | `System.Func<T>` | `PipelineContentLoader`, `PlainFileContentLoader` |
| `PipelineContentLoader` | Adapter | `ContentManager` | `NuciContentManager` |
| `PlainFileContentLoader` | Adapter | `GraphicsDevice`, `FileStream` | `NuciContentManager` |
| `NuciContentManager` | Singleton façade | Two `IContentLoader` | Host application |
| `Bitmap` | Façade | `ImageSharp.Image<Rgba32>`, `NuciXNA.Primitives` | Host application |

## Dependency Direction

```
NuciXNA.DataAccess.UnitTests
         │
         ▼
NuciXNA.DataAccess (production)
         │
         ├──► MonoGame.Framework.DesktopGL
         ├──► NuciXNA.Primitives
         └──► SixLabors.ImageSharp
```

Production code depends **outward** on external libraries. The test project depends **inward** on production code.

## Runtime Topology

The library has no executable entry point. A consuming MonoGame application provides the runtime sequence:

1. Host calls `NuciContentManager.Instance.LoadContent(ContentManager, GraphicsDevice)` during its initialisation phase
2. Manager constructs `PipelineContentLoader` and `PlainFileContentLoader`
3. Host subsequently calls `LoadSoundEffect`, `LoadSpriteFont`, `LoadTexture2D`
4. Manager applies per-asset fallback policy (see [flows/load-asset.md](./flows/load-asset.md))

## Major State

| State | Owner | Mutability | Scope |
|-------|-------|------------|-------|
| `pipelineContentLoader` | `NuciContentManager` | Mutable (replaced by `LoadContent`) | Process-wide singleton |
| `plainFileContentLoader` | `NuciContentManager` | Mutable (replaced by `LoadContent`) | Process-wide singleton |
| `MissingTexturePlaceholder` | `NuciContentManager` | Mutable (static property) | Process-wide |
| `sourceImage` | `Bitmap` | Mutable (pixel buffer) | Per-instance |
| `Size` | `Bitmap` | Immutable after construction | Per-instance |

## System-Wide Invariants

1. **Loader ordering invariant**: Pipeline loader is always attempted before plain-file loader for `SoundEffect` and `Texture2D`
2. **Font exclusivity invariant**: `SpriteFont` is loaded exclusively from pipeline; `PlainFileContentLoader.LoadSpriteFont` throws `NotImplementedException`
3. **Placeholder invariant**: When `MissingTexturePlaceholder` is set (non-null, non-empty, non-whitespace), texture fallback uses `TryLoad` on plain-file loader, then required load of placeholder from pipeline
4. **Bitmap disposal invariant**: `Bitmap.Dispose()` disposes the wrapped `Image<Rgba32>`; callers must not use the bitmap after disposal
5. **Channel ordering invariant**: `Colour.FromArgb(A, R, G, B)` ↔ `Rgba32(R, G, B, A)` round-trip preserves values

## Extension Points

1. **Custom content loaders**: Implement `IContentLoader` or derive from `ContentLoader`, supply via `LoadContent(IContentLoader, IContentLoader)`
2. **Bitmap construction**: From dimensions, `Size2D`, or existing `Image<Rgba32>` (wrapped, not cloned)