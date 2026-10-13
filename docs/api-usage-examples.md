# API Usage Examples

## NuciContentManager

### Standard Initialisation

```csharp
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NuciXNA.DataAccess.Content;

// In Game.Initialize or equivalent
NuciContentManager.Instance.LoadContent(Content, GraphicsDevice);
```

### Custom Loaders

```csharp
using NuciXNA.DataAccess.Content;

var pipeline = new PipelineContentLoader(Content);
var plain = new PlainFileContentLoader("Content");
NuciContentManager.Instance.LoadContent(pipeline, plain);
```

### Configure Missing Texture Placeholder

```csharp
using NuciXNA.DataAccess.Content;

// Before any LoadTexture2D calls
NuciContentManager.MissingTexturePlaceholder = "Textures/missing";
```

### Load Assets

```csharp
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using NuciXNA.DataAccess.Content;

var manager = NuciContentManager.Instance;

// Texture2D — pipeline → plain-file → placeholder (if configured)
Texture2D playerTexture = manager.LoadTexture2D("Textures/player");

// SoundEffect — pipeline → plain-file (.wav appended)
SoundEffect clickSound = manager.LoadSoundEffect("Audio/click");

// SpriteFont — pipeline only
SpriteFont uiFont = manager.LoadSpriteFont("Fonts/UiFont");
```

### Optional Loads (Null on Failure)

```csharp
Texture2D? optionalTexture = manager.TryLoadTexture2D("Textures/maybe-missing");
SoundEffect? optionalSound = manager.TryLoadSoundEffect("Audio/maybe-missing");
SpriteFont? optionalFont = manager.TryLoadSpriteFont("Fonts/maybe-missing");
```

## IContentLoader Implementations

### PipelineContentLoader

```csharp
using Microsoft.Xna.Framework.Content;
using NuciXNA.DataAccess.Content;

var loader = new PipelineContentLoader(Content);
Texture2D texture = loader.LoadTexture2D("Textures/player");
SoundEffect sound = loader.LoadSoundEffect("Audio/click");
SpriteFont font = loader.LoadSpriteFont("Fonts/UiFont");
```

### PlainFileContentLoader

```csharp
using NuciXNA.DataAccess.Content;

var loader = new PlainFileContentLoader("Content");
Texture2D texture = loader.LoadTexture2D("Textures/player");  // loads Textures/player.png
SoundEffect sound = loader.LoadSoundEffect("Audio/click");    // loads Audio/click.wav
// loader.LoadSpriteFont("Fonts/UiFont"); // throws NotImplementedException
```

## Bitmap

### Create New Bitmap

```csharp
using NuciXNA.DataAccess.IO;
using NuciXNA.Primitives;

// From dimensions
using Bitmap bitmap = new Bitmap(800, 600);

// From Size2D
using Bitmap bitmap = new Bitmap(new Size2D(800, 600));

// From existing ImageSharp image
using SixLabors.ImageSharp.Image<Rgba32> image = ...;
using Bitmap bitmap = new Bitmap(image);
```

### Load and Save

```csharp
using NuciXNA.DataAccess.IO;

// Load from file
using Bitmap bitmap = Bitmap.Load("input.png");

// Save to file (format inferred from extension)
bitmap.Save("output.png");
bitmap.Save("output.jpg");
bitmap.Save("output.bmp");
```

### Pixel Access

```csharp
using NuciXNA.DataAccess.IO;
using NuciXNA.Primitives;

using Bitmap bitmap = Bitmap.Load("input.png");

// Read pixel (returns Colour with ARGB)
Colour pixel = bitmap[100, 200];
Colour pixel2 = bitmap.GetPixel(100, 200);

// Write pixel (accepts Colour with ARGB)
bitmap[100, 200] = Colour.FromArgb(255, 255, 0, 0);  // Red
bitmap.SetPixel(100, 200, Colour.FromArgb(128, 0, 255, 0));  // Semi-transparent green

// Bounds
int width = bitmap.Size.Width;
int height = bitmap.Size.Height;
Point2D size = bitmap.Size;
```

### Disposal

```csharp
using NuciXNA.DataAccess.IO;

using Bitmap bitmap = Bitmap.Load("input.png");
// ... use bitmap
// Automatically disposed at end of using block

// Or manual disposal
Bitmap bitmap = Bitmap.Load("input.png");
try
{
    // ... use bitmap
}
finally
{
    bitmap.Dispose();
}
```

### Complete Example: Texture Processing Pipeline

```csharp
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NuciXNA.DataAccess.Content;
using NuciXNA.DataAccess.IO;
using NuciXNA.Primitives;

public class TextureProcessor
{
    public void Initialise(ContentManager content, GraphicsDevice graphics)
    {
        NuciContentManager.Instance.LoadContent(content, graphics);
        NuciContentManager.MissingTexturePlaceholder = "Textures/missing";
    }

    public void ProcessAndSave(string assetName, string outputPath)
    {
        // Load via content manager
        Texture2D texture = NuciContentManager.Instance.LoadTexture2D(assetName);

        // Convert to Bitmap for processing
        using Bitmap bitmap = new Bitmap(texture.Width, texture.Height);

        // Copy pixels from Texture2D to Bitmap
        Color[] xnaColors = new Color[texture.Width * texture.Height];
        texture.GetData(xnaColors);

        for (int y = 0; y < texture.Height; y++)
        {
            for (int x = 0; x < texture.Width; x++)
            {
                Color xnaColor = xnaColors[y * texture.Width + x];
                bitmap[x, y] = Colour.FromArgb(xnaColor.A, xnaColor.R, xnaColor.G, xnaColor.B);
            }
        }

        // Process: invert colours
        for (int y = 0; y < bitmap.Size.Height; y++)
        {
            for (int x = 0; x < bitmap.Size.Width; x++)
            {
                Colour c = bitmap[x, y];
                bitmap[x, y] = Colour.FromArgb(
                    c.A,
                    (byte)(255 - c.R),
                    (byte)(255 - c.G),
                    (byte)(255 - c.B)
                );
            }
        }

        // Save result
        bitmap.Save(outputPath);
    }
}
```

## Related Documentation

- [NuciContentManager API](../api-reference/nuci-content-manager.md)
- [IContentLoader API](../api-reference/icontent-loader.md)
- [Bitmap API](../api-reference/bitmap.md)
- [Content Loading Component](../components/content-loading.md)
- [Bitmap Access Component](../components/bitmap-access.md)