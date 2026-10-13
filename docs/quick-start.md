# Quick Start

## Prerequisites

- .NET 10 SDK
- MonoGame 3.8.4+ (DesktopGL)
- NuciXNA.Primitives 2.1.7+

## Installation

### NuGet Package

```bash
dotnet add package NuciXNA.DataAccess --version 4.0.4
```

### From Source

```bash
git clone https://github.com/horatiu/nucixna.dataaccess.git
cd nucixna.dataaccess
dotnet build NuciXNA.DataAccess/NuciXNA.DataAccess.csproj -c Release
```

## Minimal Setup

### 1. Add Content to MonoGame Pipeline

In your `.mgcb` file, add assets:
```
# Textures
/Textures/player.png
/Textures/missing.png

# Audio
/Audio/click.wav

# Fonts
/Fonts/UiFont.spritefont
```

Build the content pipeline.

### 2. Initialise in Game

```csharp
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NuciXNA.DataAccess.Content;

public class MyGame : Game
{
    protected override void Initialize()
    {
        // Standard initialisation
        NuciContentManager.Instance.LoadContent(Content, GraphicsDevice);

        // Optional: configure missing texture placeholder
        NuciContentManager.MissingTexturePlaceholder = "Textures/missing";

        base.Initialize();
    }
}
```

### 3. Load Assets

```csharp
protected override void LoadContent()
{
    var manager = NuciContentManager.Instance;

    // Texture2D — pipeline → plain-file → placeholder
    Texture2D playerTexture = manager.LoadTexture2D("Textures/player");

    // SoundEffect — pipeline → plain-file (.wav)
    SoundEffect clickSound = manager.LoadSoundEffect("Audio/click");

    // SpriteFont — pipeline only
    SpriteFont uiFont = manager.LoadSpriteFont("Fonts/UiFont");
}
```

### 4. Use Assets

```csharp
protected override void Draw(GameTime gameTime)
{
    GraphicsDevice.Clear(Color.CornflowerBlue);

    _spriteBatch.Begin();
    _spriteBatch.Draw(playerTexture, new Vector2(100, 100), Color.White);
    _spriteBatch.DrawString(uiFont, "Hello NuciXNA!", new Vector2(100, 200), Color.White);
    _spriteBatch.End();

    base.Draw(gameTime);
}
```

## Bitmap Quick Start

### Load, Modify, Save

```csharp
using NuciXNA.DataAccess.IO;
using NuciXNA.Primitives;

using Bitmap bitmap = Bitmap.Load("input.png");

// Read pixel
Colour pixel = bitmap[10, 20];

// Write pixel
bitmap[10, 20] = Colour.FromArgb(255, 255, 0, 0);  // Red

// Save
bitmap.Save("output.png");
```

### Create New Bitmap

```csharp
using NuciXNA.DataAccess.IO;
using NuciXNA.Primitives;

using Bitmap bitmap = new Bitmap(800, 600);

// Fill with gradient
for (int y = 0; y < bitmap.Size.Height; y++)
{
    for (int x = 0; x < bitmap.Size.Width; x++)
    {
        byte r = (byte)(x * 255 / bitmap.Size.Width);
        byte g = (byte)(y * 255 / bitmap.Size.Height);
        bitmap[x, y] = Colour.FromArgb(255, r, g, 0);
    }
}

bitmap.Save("gradient.png");
```

## Common Patterns

### Optional Loading

```csharp
Texture2D? texture = NuciContentManager.Instance.TryLoadTexture2D("Textures/optional");
if (texture != null)
{
    // Use texture
}
```

### Custom Content Root

```csharp
var pipeline = new PipelineContentLoader(Content);
var plain = new PlainFileContentLoader("MyCustomContent");
NuciContentManager.Instance.LoadContent(pipeline, plain);
```

### Error Handling

```csharp
try
{
    Texture2D texture = manager.LoadTexture2D("Textures/missing");
}
catch (ContentLoadException ex)
{
    // Pipeline load failed
}
catch (FileNotFoundException ex)
{
    // Plain-file load failed
}
```

## Next Steps

- [API Usage Examples](./api-usage-examples.md) — More detailed examples
- [Content Loading Component](../components/content-loading.md) — Architecture details
- [Bitmap Access Component](../components/bitmap-access.md) — Bitmap architecture
- [Troubleshooting](./troubleshooting.md) — Common issues
- [FAQs](./faqs.md) — Frequently asked questions

## Related Documentation

- [Root README.md](../README.md) — Full installation and usage guide
- [Repository Overview](./repository-overview.md) — Library purpose and scope