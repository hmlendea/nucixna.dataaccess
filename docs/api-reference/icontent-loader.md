# IContentLoader

**Namespace:** `NuciXNA.DataAccess.Content`
**File:** [`IContentLoader.cs`](../NuciXNA.DataAccess/Content/IContentLoader.cs)

## Overview

`IContentLoader` defines the contract for loading MonoGame content assets. It provides both required-load methods (which may throw) and optional-load methods (which return `null` on failure).

## Methods

### `LoadSoundEffect`

```csharp
SoundEffect LoadSoundEffect(string contentPath)
```

**Description:** Loads a `SoundEffect` from the specified content path. This is a required-load operation; failures propagate as exceptions.

**Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `contentPath` | `string` | The path to the sound effect asset, relative to the content root. |

**Returns:** The loaded `SoundEffect`.

**Throws:** Implementation-dependent. Common exceptions include `ContentLoadException`, `FileNotFoundException`, `IOException`.

### `TryLoadSoundEffect`

```csharp
SoundEffect TryLoadSoundEffect(string contentPath)
```

**Description:** Attempts to load a `SoundEffect` from the specified content path. Returns `null` instead of throwing if the asset cannot be loaded.

**Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `contentPath` | `string` | The path to the sound effect asset, relative to the content root. |

**Returns:** The loaded `SoundEffect`, or `null` if loading fails.

**Default behaviour (via `ContentLoader` base class):** Catches **all** exceptions (including `OutOfMemoryException`, `NullReferenceException`) and returns `null`. Custom implementations control their own behaviour.

### `LoadSpriteFont`

```csharp
SpriteFont LoadSpriteFont(string contentPath)
```

**Description:** Loads a `SpriteFont` from the specified content path. This is a required-load operation; failures propagate as exceptions.

**Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `contentPath` | `string` | The path to the sprite font asset, relative to the content root. |

**Returns:** The loaded `SpriteFont`.

**Throws:** Implementation-dependent. Common exceptions include `ContentLoadException`, `FileNotFoundException`, `IOException`.

### `TryLoadSpriteFont`

```csharp
SpriteFont TryLoadSpriteFont(string contentPath)
```

**Description:** Attempts to load a `SpriteFont` from the specified content path. Returns `null` instead of throwing if the asset cannot be loaded.

**Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `contentPath` | `string` | The path to the sprite font asset, relative to the content root. |

**Returns:** The loaded `SpriteFont`, or `null` if loading fails.

**Default behaviour (via `ContentLoader` base class):** Catches **all** exceptions and returns `null`.

### `LoadTexture2D`

```csharp
Texture2D LoadTexture2D(string contentPath)
```

**Description:** Loads a `Texture2D` from the specified content path. This is a required-load operation; failures propagate as exceptions.

**Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `contentPath` | `string` | The path to the texture asset, relative to the content root. |

**Returns:** The loaded `Texture2D`.

**Throws:** Implementation-dependent. Common exceptions include `ContentLoadException`, `FileNotFoundException`, `IOException`.

### `TryLoadTexture2D`

```csharp
Texture2D TryLoadTexture2D(string contentPath)
```

**Description:** Attempts to load a `Texture2D` from the specified content path. Returns `null` instead of throwing if the asset cannot be loaded.

**Parameters:**
| Parameter | Type | Description |
|-----------|------|-------------|
| `contentPath` | `string` | The path to the texture asset, relative to the content root. |

**Returns:** The loaded `Texture2D`, or `null` if loading fails.

**Default behaviour (via `ContentLoader` base class):** Catches **all** exceptions and returns `null`.

## Implementations

| Implementation | File | Description |
|----------------|------|-------------|
| `ContentLoader` | [`ContentLoader.cs`](../NuciXNA.DataAccess/Content/ContentLoader.cs) | Abstract base class providing `TryLoad*` wrappers that catch all exceptions. |
| `PipelineContentLoader` | [`PipelineContentLoader.cs`](../NuciXNA.DataAccess/Content/PipelineContentLoader.cs) | Loads compiled assets via `ContentManager.Load<T>`. |
| `PlainFileContentLoader` | [`PlainFileContentLoader.cs`](../NuciXNA.DataAccess/Content/PlainFileContentLoader.cs) | Loads raw `.wav` (SoundEffect) and `.png` (Texture2D) files. `LoadSpriteFont` throws `NotImplementedException`. |

## Usage in NuciContentManager

`NuciContentManager` uses two `IContentLoader` instances:
- **Pipeline loader** (primary): Attempted first for all asset types
- **Plain-file loader** (fallback): Used for `SoundEffect` and `Texture2D` when pipeline returns `null`

The manager calls `TryLoad*` on the pipeline loader and either `Load*` or `TryLoad*` on the plain-file loader depending on asset type and `MissingTexturePlaceholder` configuration.

## Path Conventions

- Paths are passed through **without normalisation** — no trimming, case adjustment, or separator conversion
- `PlainFileContentLoader` appends `.wav` for `SoundEffect` and `.png` for `Texture2D`
- `PipelineContentLoader` passes paths directly to `ContentManager.Load<T>`

## Error Semantics

| Method | Exception Behaviour |
|--------|---------------------|
| `Load*` | Required-load boundary — exceptions propagate |
| `TryLoad*` (via `ContentLoader`) | Catches **all** exceptions, returns `null` |
| `TryLoad*` (custom implementation) | Implementation-defined |

## Related Documentation

- [ContentLoader Base Class](../NuciXNA.DataAccess/Content/ContentLoader.cs)
- [PipelineContentLoader](../NuciXNA.DataAccess/Content/PipelineContentLoader.cs)
- [PlainFileContentLoader](../NuciXNA.DataAccess/Content/PlainFileContentLoader.cs)
- [NuciContentManager](./nuci-content-manager.md)
- [Content Loading Component](./components/content-loading.md)
- [Error Handling](../error-handling.md)