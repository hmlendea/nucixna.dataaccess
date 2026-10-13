# Privacy and Personal Data

This document describes the personal data handling behaviour of the `NuciXNA.DataAccess` .NET library. The library is a reusable component for MonoGame content loading and bitmap manipulation; it has no network capabilities, telemetry, user accounts, or persistent data collection.

**Information reviewed:** 2026-10-13

## 📑 Table of Contents

- [What This Document Covers](#what-this-document-covers)
- [Data We Handle](#data-we-handle)
- [Processing and Use](#processing-and-use)
- [Storage, Retention, and Deletion](#storage-retention-and-deletion)
- [External Processing and Integrations](#external-processing-and-integrations)
- [Document Changes](#document-changes)
- [Contact](#contact)

## 🔎 What This Document Covers

This document describes how `NuciXNA.DataAccess` handles personal data. It covers the library behaviour and verified integrations described below. The library is distributed as a NuGet package and runs entirely within the host application process; it has no self-hosted deployment model and no project-maintainer-operated service.

## 📥 Data We Handle

### Data Provided to the Application

The library accepts the following data from the host application:
- Content asset paths (strings) passed to `LoadSoundEffect`, `LoadSpriteFont`, `LoadTexture2D`, and their `TryLoad*` counterparts
- Image file paths (strings) passed to `Bitmap.Load` and `Bitmap.Save`
- An existing `Image<Rgba32>` instance passed to the `Bitmap(Image<Rgba32>)` constructor
- `ContentManager` and `GraphicsDevice` instances passed to `NuciContentManager.LoadContent`
- Custom `IContentLoader` implementations supplied by the host

No personal data is requested by the library. Asset paths and image paths are under full host control.

### Data Generated or Collected by the Application

The library generates no personal data automatically. It produces no telemetry, logs, crash reports, audit trails, update checks, or usage analytics. The only data generated are the MonoGame assets (`SoundEffect`, `SpriteFont`, `Texture2D`) and `Bitmap` instances returned to the caller, which contain only the pixel or audio data loaded from the provided paths.

### Data Received from Integrations

The library receives no personal data from third parties or external integrations. It interacts only with:
- MonoGame `ContentManager` (for compiled pipeline assets)
- Host file system (for raw `.wav` and `.png` files via `PlainFileContentLoader`)
- ImageSharp (for image decoding, encoding, and pixel buffer management)

None of these integrations transmit data to external recipients.

## 🧭 Processing and Use

The library processes the data described above for these verified functions:
- **Content loading** — Resolves asset paths through MonoGame content pipeline and optional raw-file fallback, returning `SoundEffect`, `SpriteFont`, or `Texture2D` instances to the caller
- **Bitmap manipulation** — Decodes image files into an in-memory `Image<Rgba32>` buffer, exposes mutable pixel access via `Colour` values, and encodes the buffer to caller-selected output paths

No profiling, analytics, personalisation, advertising, or automated decision-making occurs.

## 🗄️ Storage, Retention, and Deletion

| Data Category | Storage Location | Retention | Deletion |
|---------------|------------------|-----------|----------|
| Asset paths (strings) | Host application memory (call stack) | Duration of the load call | Discarded when call returns |
| Image file paths (strings) | Host application memory (call stack) | Duration of the load/save call | Discarded when call returns |
| `Image<Rgba32>` pixel buffer | Process heap (managed by ImageSharp) | Until `Bitmap.Dispose()` is called | Disposed by `Bitmap.Dispose()` |
| MonoGame assets (`SoundEffect`, `SpriteFont`, `Texture2D`) | Process heap (managed by MonoGame) | Until host disposes them | Host responsibility |
| `ContentManager`, `GraphicsDevice` references | `NuciContentManager` static fields | Until process exit or `LoadContent` called again | Overwritten by subsequent `LoadContent` call |

The library has no persistent storage, database, cache, or backup mechanism. All in-memory data is released when the host disposes the returned objects or when the process terminates. The host application controls all retention and deletion.

## 🔗 External Processing and Integrations

The library has no built-in external data transfer. It does not connect to any network service, telemetry endpoint, authentication provider, email service, object storage, or monitoring integration.

| Service or integration | Purpose | Data involved | Configuration or documentation |
|-----------------------|---------|---------------|--------------------------------|
| MonoGame `ContentManager` | Load compiled `.xnb` assets | Asset paths provided by host | [MonoGame documentation](https://docs.monogame.net/) |
| Host file system (`File.OpenRead`) | Read raw `.wav` and `.png` files | Asset paths with appended suffixes | Standard .NET `System.IO` |
| ImageSharp (`SixLabors.ImageSharp`) | Decode, encode, pixel buffer | Image file paths, pixel data | [ImageSharp documentation](https://github.com/SixLabors/ImageSharp) |

All data flows are strictly local to the host process. The library does not transmit data to project maintainers or any external party.

## 🛡️ Data Protection and Security

The library performs no path validation, access control, sandboxing, or secret handling. Callers determine trust boundaries and must not pass untrusted paths without appropriate host-side policy. The library contains no cryptographic operations, credential processing, or network integration.

For security vulnerability reporting, see [`SECURITY.md`](SECURITY.md).

## 🔄 Document Changes

Update this document when library data flows, storage, integrations, or deployment responsibilities change. The current version is published at the repository root as `PRIVACY.md`.

## 📬 Contact

For questions about library data handling, contact the project maintainers via the GitHub repository issues. Include the library version and relevant code paths; do not send passwords, access tokens, or other secrets.