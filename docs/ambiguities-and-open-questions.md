# Ambiguities and Open Questions

## Known Gaps from Architecture Review

### 1. NuciContentManager Initialisation Coverage

**Gap:** No tests verify `LoadContent` initialisation behaviour, reinitialisation, or null argument handling.

**Impact:** Unknown behaviour if `LoadContent` called multiple times, with null arguments, or not called before loads.

**Resolution Needed:** Add tests for:
- Multiple `LoadContent` calls
- Null `ContentManager` or `GraphicsDevice`
- Load calls before initialisation

### 2. PlainFileContentLoader Path Handling

**Gap:** Path concatenation uses simple string interpolation (`$"{_contentRoot}/{assetName}.png"`).

**Impact:** Platform-specific separator issues, double separators, relative path traversal.

**Resolution Needed:** Document expected path format or add `Path.Combine` normalisation.

### 3. PipelineContentLoader Exception Surface

**Gap:** `Load*` methods delegate directly to `ContentManager.Load<T>` without wrapping.

**Impact:** Exceptions from MonoGame pipeline (missing asset, corrupt `.xnb`, type mismatch) propagate unmodified.

**Resolution Needed:** Document exception types or add wrapping.

### 4. MissingTexturePlaceholder Validation

**Gap:** Placeholder value is not validated at set time.

**Impact:** Empty/whitespace placeholder silently disables feature; invalid path causes exception at load time.

**Resolution Needed:** Validate on set or document behaviour explicitly.

### 5. Bitmap Constructor from ImageSharp Image

**Gap:** `Bitmap(Image<Rgba32>)` constructor does not clone — wraps directly.

**Impact:** External mutation of source image affects bitmap; disposal ownership unclear.

**Resolution Needed:** Document ownership semantics or add clone option.

### 6. Bitmap.Load Format Detection

**Gap:** `Bitmap.Load(string)` relies on ImageSharp format detection.

**Impact:** Unsupported formats throw `UnknownImageFormatException` with no fallback.

**Resolution Needed:** Document supported formats or add format parameter.

### 7. Colour Channel Mapping Edge Cases

**Gap:** `Colour.FromArgb` and `Rgba32` constructor handle values outside 0-255 differently.

**Impact:** Potential silent truncation or exception on invalid values.

**Resolution Needed:** Test and document clamping/validation behaviour.

### 8. Content Root Discovery

**Gap:** No mechanism to discover content root automatically.

**Impact:** Host must know and provide correct content root path.

**Resolution Needed:** Document requirement or add discovery helper.

### 9. Asset Cache Invalidation

**Gap:** No cache invalidation or reload mechanism.

**Impact:** Changed assets require process restart.

**Resolution Needed:** Document limitation or add `Reload` method.

### 10. SoundEffect Instance Management

**Gap:** `LoadSoundEffect` returns `SoundEffect`; caller must create `SoundEffectInstance` for playback.

**Impact:** Not obvious from API; multiple instances share same buffer.

**Resolution Needed:** Document in API reference.

## Open Questions

### Q1: Should TryLoad* catch all exceptions?

**Current:** Yes — bare `catch { return null; }`

**Alternative:** Catch only expected exceptions (`FileNotFoundException`, `ContentLoadException`, etc.)

**Trade-off:** Current approach hides bugs; alternative requires maintaining exception list.

### Q2: Should NuciContentManager support multiple content roots?

**Current:** Single content root via `PlainFileContentLoader` constructor.

**Alternative:** Multiple roots with search order.

**Trade-off:** Simplicity vs flexibility.

### Q3: Should Bitmap expose ImageSharp directly?

**Current:** No — fully wrapped.

**Alternative:** Expose `Image<Rgba32>` property for advanced operations.

**Trade-off:** Encapsulation vs power.

### Q4: Should there be a non-singleton NuciContentManager?

**Current:** Singleton only.

**Alternative:** Allow instantiation for isolated contexts (testing, multiple games).

**Trade-off:** Global state simplicity vs testability.

### Q5: Should placeholder support per-asset override?

**Current:** Global `MissingTexturePlaceholder` string.

**Alternative:** `LoadTexture2D(string assetName, string? placeholderOverride)`

**Trade-off:** Global config simplicity vs per-asset control.

### Q6: Should Bitmap support other pixel formats?

**Current:** `Rgba32` only.

**Alternative:** Generic `Bitmap<TPixel>` or format parameter.

**Trade-off:** Simplicity vs flexibility.

### Q7: Should content loading be async?

**Current:** Synchronous only.

**Alternative:** `LoadTexture2DAsync` etc.

**Trade-off:** MonoGame's `ContentManager.Load` is synchronous; async would require thread pool.

### Q8: Should there be a content manifest or index?

**Current:** No — paths are strings.

**Alternative:** Manifest file listing available assets.

**Trade-off:** No metadata vs discoverability.

## TODOs from Code Comments

None found in source code.

## Related Documentation

- [Architecture](./architecture.md)
- [Design Decisions](./design-decisions.md)
- [Testing](./testing.md)
- [Change Guide](./change-guide.md)