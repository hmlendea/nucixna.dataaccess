# State and Persistence

## Overview

`NuciXNA.DataAccess` owns **no durable application state**. All state is in-memory and process-scoped.

## State Inventory

| State | Owner | Type | Mutability | Lifetime | Persistence |
|-------|-------|------|------------|----------|-------------|
| `pipelineContentLoader` | `NuciContentManager` | `IContentLoader` | Mutable (replaced by `LoadContent`) | Process | None |
| `plainFileContentLoader` | `NuciContentManager` | `IContentLoader` | Mutable (replaced by `LoadContent`) | Process | None |
| `MissingTexturePlaceholder` | `NuciContentManager` | `string` | Mutable (static property) | Process | None |
| `sourceImage` | `Bitmap` | `Image<Rgba32>` | Mutable (pixel buffer) | Per-instance | None (caller controls `Save`) |
| `Size` | `Bitmap` | `Size2D` | Immutable after construction | Per-instance | None |

## State Transitions

### NuciContentManager

```
Uninitialised → LoadContent() → Initialised
                    │
                    ├── LoadContent() again → Reinitialised (replaces loaders)
                    │
                    └── MissingTexturePlaceholder = "x" → Placeholder configured
```

### Bitmap

```
Created (dimensions) → Pixel mutations → Save() → Dispose() → Disposed
Created (Load)       → Pixel mutations → Save() → Dispose() → Disposed
Created (wrap)       → Pixel mutations → Save() → Dispose() → Disposed (wrapped image also disposed)
```

## Persistence Mechanisms

| Mechanism | Used? | Details |
|-----------|-------|---------|
| Database | No | — |
| File system (config) | No | — |
| File system (assets) | Yes | `Bitmap.Save` writes to caller-provided path; `PlainFileContentLoader` reads from caller-provided paths |
| Cache | No | Delegated to MonoGame or host |
| Serialization | No | — |

## Asset Loading State

The library does not cache loaded assets. Each `Load*` call delegates to the underlying loader:
- `PipelineContentLoader` → `ContentManager.Load<T>` (MonoGame may cache)
- `PlainFileContentLoader` → `File.OpenRead` + MonoGame factory (no caching)

## Bitmap State

- `Bitmap` owns or wraps an `Image<Rgba32>` buffer
- Pixel mutations are immediate and in-memory
- `Save` serialises current buffer state to disk via ImageSharp
- `Dispose` releases the buffer

## No Migration, No Retention

- No schema migrations (no persistent schema)
- No data retention policies
- No backup/restore mechanisms

## Related Documentation

- [Architecture](./architecture.md)
- [Bitmap Access Component](./components/bitmap-access.md)
- [Content Loading Component](./components/content-loading.md)
- [Configuration](./configuration.md)