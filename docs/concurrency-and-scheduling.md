# Concurrency and Scheduling

## Overview

`NuciXNA.DataAccess` has minimal concurrency support. The singleton instance creation is thread-safe, but all other operations are **not thread-safe**.

## Thread Safety Summary

| Component | Thread-Safe? | Notes |
|-----------|--------------|-------|
| `NuciContentManager.Instance` | Yes | Volatile double-checked locking with `Lock` |
| `NuciContentManager.LoadContent` | No | Concurrent calls may lose updates |
| `NuciContentManager.Load*` | No | No synchronisation on loader fields |
| `NuciContentManager.MissingTexturePlaceholder` | No | Static mutable property, no synchronisation |
| `PipelineContentLoader` | No | Delegates to `ContentManager` (MonoGame) |
| `PlainFileContentLoader` | No | File I/O, `GraphicsDevice` usage |
| `Bitmap` | No | ImageSharp `Image<Rgba32>` is not thread-safe |

## Synchronisation Primitives

| Primitive | Location | Purpose |
|-----------|----------|---------|
| `Lock` (C# 13) | `NuciContentManager.syncRoot` | Singleton instance creation |
| `volatile` | `NuciContentManager.instance` | Memory visibility for singleton |

## Concurrency Model

```
Thread 1                    Thread 2
   │                           │
   ├── Instance ──────────────►│ (returns same instance)
   │                           │
   ├── LoadContent(A, B) ──────┤ (may race with Thread 2)
   │                           ├── LoadContent(C, D)
   │                           │
   ├── LoadTexture2D("x") ─────┤ (reads possibly stale loaders)
   │                           ├── LoadTexture2D("y")
   │                           │
   ├── MissingTexturePlaceholder = "p" ──┤ (race with Thread 2)
   │                           ├── MissingTexturePlaceholder = "q"
```

## Known Concurrency Issues

1. **Loader replacement race:** Concurrent `LoadContent` calls may result in one loader pair being lost
2. **Stale loader reads:** Load methods read loader fields without synchronisation; may see partially updated state
3. **Placeholder race:** Concurrent reads/writes of `MissingTexturePlaceholder` may see inconsistent values
4. **Bitmap sharing:** Sharing a `Bitmap` instance across threads without external locking causes undefined behaviour

## Recommendations

1. **Initialise once:** Call `LoadContent` exactly once during application startup, before any load operations
2. **Configure placeholder early:** Set `MissingTexturePlaceholder` before any texture loads, or treat as immutable after initialisation
3. **Don't share Bitmap:** Create separate `Bitmap` instances per thread, or use external locking
4. **Don't call LoadContent concurrently:** No built-in protection for reconfiguration

## Async/Await

**No async/await is used in the library.** All operations are synchronous.

- `ContentManager.Load<T>` is synchronous
- `SoundEffect.FromStream` is synchronous
- `Texture2D.FromStream` is synchronous
- `Image.Load` / `Image.Save` are synchronous

Host applications must perform content loading on appropriate threads (typically the main/game thread for MonoGame).

## Scheduling

**No schedulers, timers, or background tasks are used.**

## Related Documentation

- [NuciContentManager API](../api-reference/nuci-content-manager.md)
- [Bitmap API](../api-reference/bitmap.md)
- [Content Loading Component](../components/content-loading.md)
- [Bitmap Access Component](../components/bitmap-access.md)