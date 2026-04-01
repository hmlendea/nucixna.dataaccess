[![Donate](https://img.shields.io/badge/-%E2%99%A5%20Donate-%23ff69b4)](https://hmlendea.go.ro/fund.html) [![Latest Release](https://img.shields.io/github/v/release/hmlendea/nucixna.dataaccess)](https://github.com/hmlendea/nucixna.dataaccess/releases/latest) [![Build Status](https://github.com/hmlendea/nucixna.dataaccess/actions/workflows/dotnet.yml/badge.svg)](https://github.com/hmlendea/nucixna.dataaccess/actions/workflows/dotnet.yml)

# NuciXNA.DataAccess

Data access utilities for the NuciXNA ecosystem, built on top of MonoGame/XNA.

This package provides:

- Flexible content loading through pipeline assets and plain files
- A singleton content manager that can fallback between loaders
- A lightweight bitmap utility powered by ImageSharp

## Features

### Content loading

`NuciContentManager` can load:

- `SoundEffect` from Content Pipeline first, then fallback to disk (`.wav`)
- `SpriteFont` from Content Pipeline
- `Texture2D` from Content Pipeline first, then fallback to disk (`.png`)

It supports a configurable missing-texture placeholder:

- Set `NuciContentManager.MissingTexturePlaceholder` to a pipeline texture path
- If a texture cannot be found, the placeholder texture is returned instead

### Bitmap helper

`NuciXNA.DataAccess.IO.Bitmap` wraps ImageSharp for pixel-level manipulation:

- Load and save images
- Get and set pixels using coordinates or points
- Work with `NuciXNA.Primitives` types (`Colour`, `Point2D`, `Size2D`)

## Installation

[![Get it from NuGet](https://raw.githubusercontent.com/hmlendea/readme-assets/master/badges/stores/nuget.png)](https://nuget.org/packages/NuciXNA.DataAccess)

### .NET CLI

```bash
dotnet add package NuciXNA.DataAccess
```

### Package Manager Console

```powershell
Install-Package NuciXNA.DataAccess
```

## Target Framework

The project currently targets `net10.0`, and MonoGame.

## Quick Start

### Initialize content loading

```csharp
using NuciXNA.DataAccess.Content;

// Usually called during game initialization
NuciContentManager.Instance.LoadContent(contentManager, graphicsDevice);

var texture = NuciContentManager.Instance.LoadTexture2D("Content/Textures/player");
var clickSfx = NuciContentManager.Instance.LoadSoundEffect("Content/Audio/click");
var uiFont = NuciContentManager.Instance.LoadSpriteFont("Fonts/UiFont");
```

### Configure missing-texture placeholder

```csharp
using NuciXNA.DataAccess.Content;

NuciContentManager.MissingTexturePlaceholder = "Textures/missing";
var texture = NuciContentManager.Instance.LoadTexture2D("Textures/unknown");
```

### Use the bitmap utility

```csharp
using NuciXNA.DataAccess.IO;
using NuciXNA.Primitives;

using Bitmap image = Bitmap.Load("input.png");
Colour pixel = image[10, 20];
image[10, 20] = Colour.FromArgb(255, 255, 0, 0);
image.Save("output.png");
```

## Development

### Build

```bash
dotnet build NuciXNA.DataAccess.sln
```

### Test

```bash
dotnet test NuciXNA.DataAccess.sln
```

## Notes

- `PlainFileContentLoader` currently supports:
	- `.wav` for `SoundEffect`
	- `.png` for `Texture2D`
- `SpriteFont` loading is pipeline-based.

## Related Projects

- [NuciXNA.DataAccess](https://github.com/hmlendea/nucixna.dataaccess)
- [NuciXNA.Graphics](https://github.com/hmlendea/nucixna.graphics)
- [NuciXNA.GUI](https://github.com/hmlendea/nucixna.gui)
- [NuciXNA.Input](https://github.com/hmlendea/nucixna.input)
- [NuciXNA.Primitives](https://github.com/hmlendea/nucixna.Primitives)

## License

Licensed under the GNU General Public License v3.0 or later.
See [LICENSE](./LICENSE) for details.
