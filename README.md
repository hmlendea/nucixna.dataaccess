[![Donate](https://img.shields.io/badge/-%E2%99%A5%20Donate-%23ff69b4)](https://hmlendea.go.ro/funding)
[![Latest Release](https://img.shields.io/github/v/release/hmlendea/nucixna.dataaccess)](https://github.com/hmlendea/nucixna.dataaccess/releases/latest)
[![Build Status](https://github.com/hmlendea/nucixna.dataaccess/actions/workflows/dotnet.yml/badge.svg)](https://github.com/hmlendea/nucixna.dataaccess/actions/workflows/dotnet.yml)
[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://gnu.org/licenses/gpl-3.0)

# NuciXNA.DataAccess

Data access utilities for the NuciXNA ecosystem, built on top of MonoGame/XNA.

## ✨ Features

- **Content loading** - loads `SoundEffect`, `SpriteFont`, and `Texture2D` assets via the MonoGame content pipeline with automatic plain-file fallback
- **Missing-texture placeholder** - configurable fallback texture returned when an asset cannot be located by either loader
- **Custom loader injection** - accepts any `IContentLoader` implementation, enabling easy testing and custom asset sources
- **Bitmap utility** - pixel-level image manipulation powered by ImageSharp, integrated with `NuciXNA.Primitives` types (`Colour`, `Point2D`, `Size2D`)

## 🚀 Usage

### Initialise content loading

```csharp
using NuciXNA.DataAccess.Content;

// Call once during game initialisation (e.g. in LoadContent)
NuciContentManager.Instance.LoadContent(Content, GraphicsDevice);

Texture2D playerTexture = NuciContentManager.Instance.LoadTexture2D("Textures/player");
SoundEffect clickSound = NuciContentManager.Instance.LoadSoundEffect("Audio/click");
SpriteFont uiFont = NuciContentManager.Instance.LoadSpriteFont("Fonts/UiFont");
```

### Configure a missing-texture placeholder

```csharp
using NuciXNA.DataAccess.Content;

NuciContentManager.MissingTexturePlaceholder = "Textures/missing";

// Returns the placeholder texture if "Textures/unknown" cannot be found
Texture2D texture = NuciContentManager.Instance.LoadTexture2D("Textures/unknown");
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

## ⚠️ Known Limitations

- `PlainFileContentLoader` supports `.wav` for `SoundEffect` and `.png` for `Texture2D` only; other formats require a custom `IContentLoader` implementation.
- `SpriteFont` loading is pipeline-only; `PlainFileContentLoader.LoadSpriteFont` throws `NotImplementedException`.

## 📦 Installation

[![Get it from NuGet](https://raw.githubusercontent.com/hmlendea/readme-assets/master/badges/stores/nuget.png)](https://nuget.org/packages/NuciXNA.DataAccess)

### .NET CLI

```bash
dotnet add package NuciXNA.DataAccess
```

### Package Manager Console

```powershell
Install-Package NuciXNA.DataAccess
```

## 🛠️ Development

### Requirements

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)

All NuGet dependencies are restored automatically by `dotnet restore`.

### Build

```bash
dotnet build NuciXNA.DataAccess.sln
```

### Test

```bash
dotnet test NuciXNA.DataAccess.sln
```

### Release

```bash
dotnet pack NuciXNA.DataAccess -c Release
```

### Dependencies

| Package | Purpose |
|---------|---------|
| `MonoGame.Framework.DesktopGL` | XNA/MonoGame types (`Texture2D`, `SoundEffect`, `SpriteFont`, `GraphicsDevice`) |
| `NuciXNA.Primitives` | `Colour`, `Point2D`, and `Size2D` used by the bitmap utility |
| `SixLabors.ImageSharp` | Pixel-level image decoding and encoding for `Bitmap` |

## 🗂️ Project Structure

The solution contains the following projects:

- `NuciXNA.DataAccess`: The main library with content loading and bitmap utilities.
- `NuciXNA.DataAccess.UnitTests`: Unit tests for the main library.

The key directories inside `NuciXNA.DataAccess/` are:

| Directory | Purpose |
|-----------|---------|
| `Content/` | Content loading infrastructure (`IContentLoader`, `ContentLoader`, `NuciContentManager`, `PipelineContentLoader`, `PlainFileContentLoader`) |
| `IO/` | File I/O utilities (`Bitmap`) |

## 🤝 Contributing

Contributions are welcome. Please:
- Keep the changes cross-platform
- Keep the existing public contract intact unless a breaking change is intentional
- Keep the pull requests focused and consistent with the existing code style
- Update the documentation when behaviour changes
- Properly test all changes, including edge cases and error conditions
- Add unit tests for any new or changed functionality

## 🔗 Related Projects

- [NuciXNA.Graphics](https://github.com/hmlendea/nucixna.graphics): Rendering and graphics utilities for the NuciXNA ecosystem
- [NuciXNA.GUI](https://github.com/hmlendea/nucixna.gui): GUI controls and screen management for NuciXNA games
- [NuciXNA.Input](https://github.com/hmlendea/nucixna.input): Input handling (keyboard, mouse, gamepad) for NuciXNA games
- [NuciXNA.Primitives](https://github.com/hmlendea/nucixna.primitives): Primitive types (`Colour`, `Point2D`, `Size2D`) shared across the NuciXNA ecosystem

## 💝 Support

Found a bug or have a suggestion? [Open an issue](https://github.com/hmlendea/nucixna.dataaccess/issues)!

If you find this project useful, consider [funding it](https://hmlendea.go.ro/funding) or giving a ⭐️ on GitHub!

## 📄 License

Licensed under the `GNU General Public License v3.0` or later.
See [LICENSE](./LICENSE) for details.

