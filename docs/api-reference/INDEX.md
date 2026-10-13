# API Reference

This section documents the public API of the `NuciXNA.DataAccess` library.

## Summary

| Class/Interface | Namespace | Description |
|-----------------|-----------|-------------|
| [`NuciContentManager`](./nuci-content-manager.md) | `NuciXNA.DataAccess.Content` | Singleton facade coordinating content loading via pipeline and plain-file loaders. |
| [`IContentLoader`](./icontent-loader.md) | `NuciXNA.DataAccess.Content` | Contract for loading MonoGame assets (`SoundEffect`, `SpriteFont`, `Texture2D`). |
| [`Bitmap`](./bitmap.md) | `NuciXNA.DataAccess.IO` | Mutable bitmap facade over `ImageSharp.Image<Rgba32>` with pixel access via `NuciXNA.Primitives`. |

## Related Documentation

- [Usage Examples](./api-usage-examples.md) — Practical examples of common API usage
- [Changelog](../CHANGELOG.md) — Version history (if present)
- [Architecture](./architecture.md) — High-level design and component relationships