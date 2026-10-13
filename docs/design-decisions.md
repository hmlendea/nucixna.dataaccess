# Design Decisions

## Decision Log

### 1. Loader Interface with Base Optional-Load Wrapper

**Decision:** Use `IContentLoader` interface with `ContentLoader` abstract base class providing `TryLoad*` wrappers.

**Rationale:** Host code and tests can substitute asset sources while common optional-load behaviour is centralised.

**Consequence:** Required and optional operations have materially different exception semantics. `TryLoad*` catches all exceptions (including `OutOfMemoryException`, `NullReferenceException`) and returns `null`.

**Status:** Accepted

**Verified by:** `ContentLoaderTests.cs` — tests for 5 exception types × 3 asset types

### 2. Pipeline-First with Plain-File Fallback

**Decision:** Pipeline loader is always attempted first; plain-file loader is fallback for `SoundEffect` and `Texture2D`.

**Rationale:** Compiled pipeline assets are preferred for performance and reliability; raw files provide a development-time fallback.

**Consequence:** Asset availability and failure behaviour depend on asset type and placeholder configuration.

**Status:** Accepted

**Verified by:** `NuciContentManagerTests.cs` — tests for all fallback branches

### 3. SpriteFont Pipeline-Only

**Decision:** `SpriteFont` is loaded exclusively from the pipeline; `PlainFileContentLoader.LoadSpriteFont` throws `NotImplementedException`.

**Rationale:** Raw font files require complex parsing; MonoGame's pipeline handles this.

**Consequence:** Fonts must be compiled into the content pipeline.

**Status:** Accepted

**Verified by:** `NuciContentManagerTests.cs` — tests verify plain-file loader never called for fonts

### 4. Global Content Facade (Singleton)

**Decision:** `NuciContentManager.Instance` is the sole construction path.

**Rationale:** Simplifies host integration — single access point for all content loading.

**Consequence:** Initialisation/reconfiguration is process-wide. No reset or disposal operation.

**Status:** Accepted

**Verified by:** Not directly tested (coverage gap)

### 5. Placeholder Substitution for Textures

**Decision:** When `MissingTexturePlaceholder` is set, texture fallback changes from required plain-file load to optional plain-file load + required placeholder pipeline load.

**Rationale:** Prevents crashes when textures are missing during development; provides visual feedback.

**Consequence:** Three-stage fallback for textures when placeholder is enabled; two-stage when disabled.

**Status:** Accepted

**Verified by:** `NuciContentManagerTests.cs` — tests for placeholder enabled, disabled, empty, whitespace

### 6. Delegate Image Mechanics to ImageSharp

**Decision:** `Bitmap` wraps `Image<Rgba32>` and forwards loading, saving, storage, and disposal.

**Rationale:** Avoids reimplementing image codec logic; leverages mature library.

**Consequence:** Codec, bounds, and resource behaviours inherit from ImageSharp.

**Status:** Accepted

**Verified by:** `BitmapTests.cs` — tests for construction, pixel access, disposal

### 7. Channel Ordering: Colour ARGB ↔ Rgba32 RGBA

**Decision:** `GetPixel` constructs `Colour.FromArgb(A, R, G, B)` from `Rgba32(R, G, B, A)`; `SetPixel` constructs `Rgba32(R, G, B, A)` from `Colour(A, R, G, B)`.

**Rationale:** Matches `NuciXNA.Primitives.Colour` API convention while using ImageSharp's native pixel format.

**Consequence:** Round-trip preserves all four channels exactly.

**Status:** Accepted

**Verified by:** `BitmapTests.cs` — round-trip tests for multiple colours

### 8. No Path Normalisation

**Decision:** Content paths are forwarded to loaders without trimming, case adjustment, or separator conversion.

**Rationale:** Preserves caller intent; avoids platform-specific assumptions.

**Consequence:** Host must provide correctly formatted paths.

**Status:** Accepted

**Verified by:** `NuciContentManagerTests.cs` — path forwarding tests

### 9. No Logging or Telemetry

**Decision:** No logging, metrics, or tracing is implemented.

**Rationale:** Keeps library lightweight; avoids dependency on logging frameworks.

**Consequence:** Optional failures collapse to `null` with no diagnostic information.

**Status:** Accepted

**Verified by:** `docs/logging.md` — documents absence of logging

### 10. No Thread Safety Beyond Singleton

**Decision:** Only singleton instance creation is thread-safe; all other operations are not.

**Rationale:** Simplifies implementation; MonoGame operations typically occur on main thread.

**Consequence:** Host must synchronise concurrent access.

**Status:** Accepted

**Verified by:** `docs/concurrency-and-scheduling.md` — documents thread safety

## Related Documentation

- [Architecture](./architecture.md)
- [Invariants](./invariants.md)
- [Error Handling](./error-handling.md)
- [Concurrency and Scheduling](./concurrency-and-scheduling.md)