# NuciContentManager

**Namespace:** `NuciXNA.DataAccess.Content`
**File:** [`NuciContentManager.cs`](../NuciXNA.DataAccess/Content/NuciContentManager.cs)

## Overview

`NuciContentManager` is a thread-safe singleton facade that coordinates content loading across two `IContentLoader` instances: a pipeline loader (primary) and a plain-file loader (fallback). It applies per-asset-type fallback and placeholder policies.

## Singleton Access

### `Instance`

```csharp
public static NuciContentManager Instance { get; }
```

**Description:** Gets the singleton instance using volatile double-checked locking.

**Thread safety:** Instance creation is thread-safe via `volatile` field and `Lock` double-checked initialisation. However, `LoadContent`, `MissingTexturePlaceholder`, and load methods are **not** thread-safe.

**Returns:** The process-wide `NuciContentManager` instance.

## Initialisation

### `LoadContent(IContentLoader, IContentLoader)`

```csharp
public void LoadContent(IContentLoader pipelineContentLoader, IContentLoader plainFileContentLoader)
```

**Description:** Initialises the manager with caller-supplied loader instances. Use this overload for unit tests or custom loader implementations.

**Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `pipelineContentLoader` | `IContentLoader` | The primary pipeline-based content loader. |
| `plainFileContentLoader` | `IContentLoader` | The fallback plain-file content loader. |

**Side effects:** Replaces the retained loader references. No validation is performed.

**Throws:** Nothing directly. Calling load methods before initialisation dereferences unset fields and fails.

### `LoadContent(ContentManager, GraphicsDevice)`

```csharp
public void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
```

**Description:** Initialises the manager using standard MonoGame dependencies. Creates a `PipelineContentLoader` and `PlainFileContentLoader` internally.

**Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `content` | `ContentManager` | The MonoGame content manager for pipeline-compiled assets. |
| `graphicsDevice` | `GraphicsDevice` | The graphics device for loading raw texture files. |

**Side effects:** Creates new `PipelineContentLoader` and `PlainFileContentLoader` instances and assigns them to the internal loader fields.

## Static Configuration

### `MissingTexturePlaceholder`

```csharp
public static string MissingTexturePlaceholder { get; set; }
```

**Description:** Gets or sets the content path of the texture to use as a placeholder when a requested texture cannot be found.

**Default:** `null`

**Policy:** Set to `null`, empty string, or whitespace to disable placeholder substitution. When set to a non-empty value, the texture fallback policy changes (see [LoadTexture2D](#loadtexture2d) below).

## Asset Loading Methods

### `LoadSoundEffect`

```csharp
public SoundEffect LoadSoundEffect(string contentPath)
```

**Description:** Loads a `SoundEffect`. Attempts the pipeline loader first via `TryLoadSoundEffect`; falls back to the plain-file loader via `LoadSoundEffect` if the pipeline returns `null`.

**Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `contentPath` | `string` | The path to the sound effect asset, relative to the content root. |

**Returns:** The loaded `SoundEffect`.

**Behaviour:**
1. Calls `pipelineContentLoader.TryLoadSoundEffect(contentPath)`
2. If result is `null`, calls `plainFileContentLoader.LoadSoundEffect(contentPath)` (required load)
3. Returns the first non-null result

**Throws:** Propagates any exception from the plain-file required load if the pipeline returns `null` and the plain-file load fails.

**Flow reference:** [Load an asset](./flows/load-asset.md)

### `LoadSpriteFont`

```csharp
public SpriteFont LoadSpriteFont(string contentPath)
```

**Description:** Loads a `SpriteFont` exclusively from the pipeline loader via `LoadSpriteFont` (required load).

**Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `contentPath` | `string` | The path to the sprite font asset, relative to the content root. |

**Returns:** The loaded `SpriteFont`.

**Throws:** Propagates any exception from the pipeline loader. No fallback is attempted.

**Flow reference:** [Load an asset](./flows/load-asset.md)

### `LoadTexture2D`

```csharp
public Texture2D LoadTexture2D(string contentPath)
```

**Description:** Loads a `Texture2D`. Attempts the pipeline loader first via `TryLoadTexture2D`; falls back to the plain-file loader. When `MissingTexturePlaceholder` is set and neither loader finds the asset, the placeholder texture is loaded from the pipeline.

**Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `contentPath` | `string` | The path to the texture asset, relative to the content root. |

**Returns:** The loaded `Texture2D`, or `null` if no asset could be found.

**Behaviour:**
1. Calls `pipelineContentLoader.TryLoadTexture2D(contentPath)`
2. If result is `null`:
   - If `MissingTexturePlaceholder` is null/empty/whitespace: calls `plainFileContentLoader.LoadTexture2D(contentPath)` (required load)
   - If `MissingTexturePlaceholder` is set: calls `plainFileContentLoader.TryLoadTexture2D(contentPath)` (optional load)
3. If result is still `null` and `MissingTexturePlaceholder` is set: calls `pipelineContentLoader.LoadTexture2D(MissingTexturePlaceholder)` (required load)
4. Returns the result

**Throws:**
- Propagates exceptions from required loads (plain-file load without placeholder, or placeholder pipeline load)
- Does not throw for optional `TryLoad` failures (returns `null`)

**Flow reference:** [Load an asset](./flows/load-asset.md)

## Error Scenarios

| Scenario | Behaviour |
|----------|-----------|
| Pipeline `TryLoad` throws | Exception suppressed, returns `null` |
| Plain-file required `Load` throws | Exception propagates to caller |
| Placeholder pipeline `Load` throws | Exception propagates to caller |
| `MissingTexturePlaceholder` is null/empty/whitespace | Placeholder substitution disabled; plain-file fallback is required |
| Loaders not initialised | `NullReferenceException` from dereferencing unset fields |

## Related Documentation

- [Content Loading Component](./components/content-loading.md)
- [Load Asset Flow](./flows/load-asset.md)
- [Error Handling](../error-handling.md)
- [Configuration](../configuration.md)