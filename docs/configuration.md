# Configuration

## Overview

`NuciXNA.DataAccess` has minimal configuration — no configuration files, environment variables, or precedence model. Configuration is entirely programmatic via static properties and method arguments.

## Configuration Points

### 1. Missing Texture Placeholder

**Location:** `NuciContentManager.MissingTexturePlaceholder` (static property)

**Type:** `string`

**Default:** `null`

**Values:**
| Value | Effect |
|-------|--------|
| `null` | Placeholder disabled (default) |
| `""` (empty) | Placeholder disabled |
| `" "` (whitespace) | Placeholder disabled |
| `"path/to/texture"` | Placeholder enabled — uses this path for fallback |

**Scope:** Process-wide, mutable at any time

**Precedence:** None — last write wins

**Persistence:** In-memory only, not persisted

**Security:** No validation — host must ensure path is trusted

### 2. Content Loaders

**Location:** `NuciContentManager.LoadContent(...)` method arguments

**Overloads:**
| Overload | Parameters | Use Case |
|----------|------------|----------|
| Standard | `ContentManager`, `GraphicsDevice` | Production MonoGame applications |
| Injection | `IContentLoader`, `IContentLoader` | Testing, custom asset sources |

**Scope:** Process-wide, mutable — each call replaces previous loaders

**Precedence:** Last call wins

**Validation:** None — null arguments cause `NullReferenceException` on first load

### 3. Bitmap Construction

**Location:** `Bitmap` constructors and `Bitmap.Load`

**Parameters:** Caller-provided dimensions, `Size2D`, `Image<Rgba32>`, or file path

**No persistent configuration** — all parameters are per-instance

## Configuration Model

```
┌─────────────────────────────────────────────────────────────┐
│                    Configuration Model                       │
├─────────────────────────────────────────────────────────────┤
│  No config files, no env vars, no DI container              │
│                                                              │
│  ┌─────────────────────┐    ┌─────────────────────────────┐ │
│  │ Static Property     │    │ Method Arguments            │ │
│  │ MissingTexture      │    │ LoadContent(...)            │ │
│  │ Placeholder         │    │ Bitmap constructors         │ │
│  │                     │    │ Bitmap.Load(path)           │ │
│  │ - Process-wide      │    │ - Per-call / per-instance   │ │
│  │ - Mutable           │    │ - No persistence            │ │
│  │ - No validation     │    │ - No validation             │ │
│  └─────────────────────┘    └─────────────────────────────┘ │
└─────────────────────────────────────────────────────────────┘
```

## Override Policy

| Configuration | Override Mechanism | Constraints |
|---------------|-------------------|-------------|
| `MissingTexturePlaceholder` | Direct property assignment | None — any string accepted |
| Pipeline loader | `LoadContent(IContentLoader, IContentLoader)` | Must implement `IContentLoader` |
| Plain-file loader | `LoadContent(IContentLoader, IContentLoader)` | Must implement `IContentLoader` |
| Bitmap dimensions | Constructor arguments | Must be valid for ImageSharp |
| Bitmap source image | `Bitmap(Image<Rgba32>)` constructor | Caller retains ownership but `Dispose()` disposes it |

## Secret Handling

**No secrets are used or stored by this library.**

- No API keys, connection strings, or credentials
- No configuration encryption
- No secret scanning or redaction

## Runtime Modification

| Configuration | Can Change at Runtime? | Effect on In-Flight Operations |
|---------------|------------------------|--------------------------------|
| `MissingTexturePlaceholder` | Yes | Next texture load uses new value |
| Pipeline loader | Yes (via `LoadContent`) | Next load uses new loader |
| Plain-file loader | Yes (via `LoadContent`) | Next load uses new loader |
| Bitmap instance | N/A (per-instance) | N/A |

## Related Documentation

- [NuciContentManager API](../api-reference/nuci-content-manager.md)
- [Content Loading Component](../components/content-loading.md)
- [Load Asset Flow](../flows/load-asset.md)
- [Security](../security.md)