# Content Loading Component

## Purpose

The content loading subsystem provides a unified API for loading MonoGame assets (`SoundEffect`, `SpriteFont`, `Texture2D`) with automatic fallback from compiled pipeline assets to raw files, and optional placeholder substitution for textures.

## Scope

**Owns:**
- Asset loading orchestration and fallback policy
- Placeholder texture selection
- The `IContentLoader` contract and `ContentLoader` base class
- `PipelineContentLoader` and `PlainFileContentLoader` adapters

**Does not own:**
- MonoGame's `ContentManager` or `GraphicsDevice` lifecycle
- Asset caching (delegated to MonoGame or host)
- File system access control or path validation
- Process lifecycle or dependency injection

## Location

| File | Role |
|------|------|
| [`IContentLoader.cs`](../NuciXNA.DataAccess/Content/IContentLoader.cs) | Public contract for asset loading |
| [`ContentLoader.cs`](../NuciXNA.DataAccess/Content/ContentLoader.cs) | Abstract base with `TryLoad*` wrappers |
| [`PipelineContentLoader.cs`](../NuciXNA.DataAccess/Content/PipelineContentLoader.cs) | Pipeline adapter using `ContentManager.Load<T>` |
| [`PlainFileContentLoader.cs`](../NuciXNA.DataAccess/Content/PlainFileContentLoader.cs) | Raw-file adapter for `.wav` and `.png` |
| [`NuciContentManager.cs`](../NuciXNA.DataAccess/Content/NuciContentManager.cs) | Singleton façade and policy coordinator |

## Entry Points

1. **`NuciContentManager.Instance.LoadContent(ContentManager, GraphicsDevice)`** — Standard initialisation with MonoGame dependencies
2. **`NuciContentManager.Instance.LoadContent(IContentLoader, IContentLoader)`** — Injection initialisation with custom loaders
3. **`NuciContentManager.Instance.LoadSoundEffect(path)`** — Load a sound effect
4. **`NuciContentManager.Instance.LoadSpriteFont(path)`** — Load a sprite font
5. **`NuciContentManager.Instance.LoadTexture2D(path)`** — Load a texture

## Execution

### ContentLoader Base Class

`ContentLoader` is an abstract base class that implements `IContentLoader`. It defines three abstract `Load*` methods and provides concrete `TryLoad*` wrappers:

```csharp
private static T TryLoad<T>(Func<T> load) where T : class
{
    try
    {
        return load();
    }
    catch
    {
        return null;
    }
}
```

**Key behaviour:** The `TryLoad*` methods catch **all** exceptions (including `OutOfMemoryException`, `NullReferenceException`) and return `null`. This is intentional, tested behaviour — not filtered missing-file handling.

### PipelineContentLoader

Delegates directly to `ContentManager.Load<T>`:

| Method | Implementation |
|--------|----------------|
| `LoadSoundEffect` | `content.Load<SoundEffect>(contentPath)` |
| `LoadSpriteFont` | `content.Load<SpriteFont>(contentPath)` |
| `LoadTexture2D` | `content.Load<Texture2D>(contentPath)` |

No path transformation is applied.

### PlainFileContentLoader

Loads raw files from the file system:

| Method | Implementation |
|--------|----------------|
| `LoadSoundEffect` | `SoundEffect.FromStream(File.OpenRead($"{contentPath}.wav"))` |
| `LoadSpriteFont` | `throw new NotImplementedException()` |
| `LoadTexture2D` | `Texture2D.FromStream(graphicsDevice, File.OpenRead($"{contentPath}.png"))` |

**Key behaviours:**
- Appends `.wav` for sounds, `.png` for textures
- `LoadSpriteFont` throws `NotImplementedException` by design
- `File.OpenRead` returns a `FileStream` passed directly to MonoGame factory methods without a local `using` — stream ownership is determined by those external APIs

### NuciContentManager

The singleton façade coordinates two `IContentLoader` instances and applies per-asset-type policy.

#### LoadSoundEffect

```
pipeline.TryLoadSoundEffect(path)
  → if null: plainFile.LoadSoundEffect(path)  [required]
  → return result
```

#### LoadSpriteFont

```
pipeline.LoadSpriteFont(path)  [required, no fallback]
  → return result
```

#### LoadTexture2D

```
pipeline.TryLoadTexture2D(path)
  → if null:
      if placeholder disabled: plainFile.LoadTexture2D(path)  [required]
      if placeholder enabled:  plainFile.TryLoadTexture2D(path)  [optional]
  → if still null AND placeholder enabled:
      pipeline.LoadTexture2D(MissingTexturePlaceholder)  [required]
  → return result
```

## Data

| Data | Representation | Lifecycle |
|------|----------------|-----------|
| Loader references | Two `IContentLoader` fields | Replaced by each `LoadContent` call |
| Placeholder path | Static `string` | Read per texture request |
| Asset paths | `string` | Passed through without normalisation |

## State

- `pipelineContentLoader` — mutable, replaced by `LoadContent`
- `plainFileContentLoader` — mutable, replaced by `LoadContent`
- `MissingTexturePlaceholder` — static mutable `string`

## Dependencies

| Dependency | Direction | Purpose |
|------------|-----------|---------|
| `IContentLoader` | Inbound | Contract for asset loading |
| `ContentManager` | Outbound | Pipeline asset loading |
| `GraphicsDevice` | Outbound | Raw texture decoding |
| `FileStream` | Outbound | Raw file access |

## Dependants

- Host MonoGame applications call `NuciContentManager.Instance.Load*` methods
- Tests mock `IContentLoader` to verify manager policy

## Configuration

| Setting | Source | Effect |
|---------|--------|--------|
| `MissingTexturePlaceholder` | Static property | When set (non-null, non-empty, non-whitespace), changes texture fallback from required plain-file load to optional plain-file load + required placeholder pipeline load |
| Loader instances | `LoadContent` arguments | Selects standard MonoGame adapters or host-defined implementations |

## Failures

| Failure Point | Behaviour |
|---------------|-----------|
| Pipeline `TryLoad*` throws | Exception suppressed, returns `null` |
| Plain-file `Load*` throws | Exception propagates to caller |
| Placeholder pipeline `Load*` throws | Exception propagates to caller |
| Loaders not initialised | `NullReferenceException` from dereferencing unset fields |
| `PlainFileContentLoader.LoadSpriteFont` | Always throws `NotImplementedException` |

## Invariants

1. Pipeline loader is always attempted before plain-file loader for `SoundEffect` and `Texture2D`
2. `SpriteFont` is loaded exclusively from pipeline; no fallback
3. When `MissingTexturePlaceholder` is set, texture fallback uses `TryLoad` on plain-file loader, then required load of placeholder from pipeline
4. Paths are forwarded without normalisation
5. `TryLoad*` catches all exceptions and returns `null`

## Tests

| Test File | Coverage |
|-----------|----------|
| [`ContentLoaderTests.cs`](../NuciXNA.DataAccess.UnitTests/Content/ContentLoaderTests.cs) | `TryLoad*` exception suppression (IOException, ArgumentException, InvalidOperationException, OutOfMemoryException, NullReferenceException), null return handling, path forwarding |
| [`NuciContentManagerTests.cs`](../NuciXNA.DataAccess.UnitTests/Content/NuciContentManagerTests.cs) | Loader selection, propagation, placeholder branches, path forwarding, empty/whitespace placeholder handling |

## Integration

The content loading subsystem is the primary entry point for host applications. It integrates with:
- MonoGame's `ContentManager` for compiled assets
- The host file system for raw `.wav` and `.png` files
- `NuciXNA.Primitives` (indirectly, via `Bitmap`)

## Modification Impact

Changes to this component may require updates to:
- `NuciContentManagerTests.cs` — if fallback policy changes
- `ContentLoaderTests.cs` — if `TryLoad*` exception handling changes
- `ARCHITECTURE.md` — if architectural boundaries change
- `docs/flows/load-asset.md` — if loading flow changes
- `docs/api-reference/nuci-content-manager.md` — if API changes