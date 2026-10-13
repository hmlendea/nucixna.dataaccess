# Troubleshooting

## Common Issues

### 1. NullReferenceException on Load Calls

**Symptom:** `NullReferenceException` when calling `LoadSoundEffect`, `LoadSpriteFont`, or `LoadTexture2D`

**Cause:** `LoadContent` was not called before load methods, or was called with null arguments.

**Solution:**
```csharp
// Call once during initialisation
NuciContentManager.Instance.LoadContent(Content, GraphicsDevice);

// Or with custom loaders
NuciContentManager.Instance.LoadContent(pipelineLoader, plainFileLoader);
```

### 2. Texture Not Found — No Placeholder

**Symptom:** `LoadTexture2D` throws exception or returns `null` for missing texture

**Cause:** Texture not in pipeline, no `.png` file at path, and `MissingTexturePlaceholder` not configured.

**Solution:**
```csharp
// Option 1: Add .png file at content path
// Option 2: Configure placeholder
NuciContentManager.MissingTexturePlaceholder = "Textures/missing";
```

### 3. Texture Not Found — With Placeholder

**Symptom:** `LoadTexture2D` throws exception even with `MissingTexturePlaceholder` set

**Cause:** Placeholder texture not in pipeline.

**Solution:** Ensure the placeholder texture exists in the compiled content pipeline at the configured path.

### 4. SpriteFont Load Fails

**Symptom:** `LoadSpriteFont` throws exception

**Cause:** Font not in pipeline. `PlainFileContentLoader` does not support fonts.

**Solution:** Add the `.spritefont` file to the MonoGame content pipeline and rebuild.

### 5. SoundEffect Load Fails

**Symptom:** `LoadSoundEffect` throws exception

**Cause:** Sound not in pipeline and no `.wav` file at path.

**Solution:**
- Add `.xnb` sound to pipeline, OR
- Add `.wav` file at content path (appended automatically)

### 6. Bitmap.Load Fails

**Symptom:** `FileNotFoundException` or `UnknownImageFormatException`

**Cause:** File doesn't exist, path is wrong, or format not supported by ImageSharp.

**Solution:**
- Verify file path
- Check ImageSharp supported formats (PNG, JPEG, BMP, GIF, TIFF, etc.)

### 7. Bitmap.Save Fails

**Symptom:** `IOException` or `UnknownImageFormatException`

**Cause:** Output directory doesn't exist, no write permission, or extension not recognised.

**Solution:**
- Ensure output directory exists
- Check write permissions
- Use supported extension (.png, .jpg, .bmp, etc.)

### 8. OutOfMemoryException on Large Bitmap

**Symptom:** `OutOfMemoryException` when creating large `Bitmap`

**Cause:** ImageSharp allocates `width * height * 4` bytes. Large dimensions exceed available memory.

**Solution:**
- Reduce dimensions
- Process in tiles
- Ensure 64-bit process

### 9. IndexOutOfRangeException on Pixel Access

**Symptom:** `IndexOutOfRangeException` on `GetPixel`/`SetPixel`

**Cause:** Coordinates outside bitmap bounds (negative or >= width/height).

**Solution:**
```csharp
if (x >= 0 && x < bitmap.Size.Width && y >= 0 && y < bitmap.Size.Height)
{
    var colour = bitmap.GetPixel(x, y);
}
```

### 10. ObjectDisposedException After Dispose

**Symptom:** `ObjectDisposedException` when using `Bitmap` after `Dispose()`

**Cause:** `Bitmap.Dispose()` disposes the underlying `Image<Rgba32>`.

**Solution:** Don't use bitmap after disposal. Create new instance if needed.

### 11. Placeholder Not Working

**Symptom:** `MissingTexturePlaceholder` set but fallback doesn't use it

**Cause:** Placeholder value is empty string or whitespace.

**Solution:**
```csharp
// These disable placeholder:
NuciContentManager.MissingTexturePlaceholder = null;
NuciContentManager.MissingTexturePlaceholder = "";
NuciContentManager.MissingTexturePlaceholder = "   ";

// This enables placeholder:
NuciContentManager.MissingTexturePlaceholder = "Textures/missing";
```

### 12. Concurrent Access Issues

**Symptom:** Inconsistent behaviour, crashes, or corrupted images under load

**Cause:** `NuciContentManager` and `Bitmap` are not thread-safe.

**Solution:**
- Initialise `LoadContent` once at startup
- Set `MissingTexturePlaceholder` before any loads
- Don't share `Bitmap` instances across threads
- Use external locking if concurrent access required

## Debugging Tips

1. **Enable MonoGame content logging** to see pipeline load attempts
2. **Check file paths** — paths are passed through without normalisation
3. **Verify content pipeline build** — `.xnb` files must be built
4. **Test with custom loaders** — inject mock loaders to isolate issues
5. **Check ImageSharp version** — codec support varies by version

## Related Documentation

- [Error Handling](./error-handling.md)
- [Configuration](./configuration.md)
- [Concurrency and Scheduling](./concurrency-and-scheduling.md)
- [NuciContentManager API](../api-reference/nuci-content-manager.md)
- [Bitmap API](../api-reference/bitmap.md)