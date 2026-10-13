[![Donate](https://img.shields.io/badge/-%E2%99%A5%20Donate-%23ff69b4)](https://hmlendea.go.ro/funding)
[![Latest Release](https://img.shields.io/github/v/release/hmlendea/nucixna.dataaccess)](https://github.com/hmlendea/nucixna.dataaccess/releases/latest)
[![Build Status](https://github.com/hmlendea/nucixna.dataaccess/actions/workflows/dotnet.yml/badge.svg)](https://github.com/hmlendea/nucixna.dataaccess/actions/workflows/dotnet.yml)
[![NuGet](https://img.shields.io/nuget/v/NuciXNA.DataAccess)](https://nuget.org/packages/NuciXNA.DataAccess)
[![License](https://img.shields.io/github/license/hmlendea/nucixna.dataaccess)](https://github.com/hmlendea/nucixna.dataaccess/blob/master/LICENSE)

# NuciXNA.DataAccess

Data access utilities for the NuciXNA ecosystem, built on top of MonoGame/XNA.

## 📑 Table of Contents

- [Capabilities](#capabilities)
- [Usage](#usage)
- [Known Limitations](#known-limitations)
- [Installation](#installation)
- [Development](#development)
- [Dependencies](#dependencies)
- [Project Structure](#project-structure)
- [Architecture](#architecture)
- [Contributing](#contributing)
- [Code of Conduct](#code-of-conduct)
- [Security](#security)
- [Project Engagement](#project-engagement)
- [License](#license)

## ✨ Capabilities

- **Content loading** — loads `SoundEffect`, `SpriteFont`, and `Texture2D` assets via the MonoGame content pipeline with automatic plain-file fallback
- **Missing-texture placeholder** — configurable pipeline fallback attempted when an asset cannot be located by either loader
- **Custom loader injection** — accepts any `IContentLoader` implementation, enabling easy testing and custom asset sources
- **Bitmap utility** — pixel-level image manipulation powered by ImageSharp, integrated with `NuciXNA.Primitives` types (`Colour`, `Point2D`, `Size2D`)

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

[![Obtain it from NuGet](https://raw.githubusercontent.com/hmlendea/readme-assets/master/badges/stores/nuget.png)](https://nuget.org/packages/NuciXNA.DataAccess)

### Package Manager Installation

```bash
dotnet add package NuciXNA.DataAccess
```

Or, via the `Package Manager Console`:
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

| Package | Version | Scope | Purpose |
|---------|---------|-------|---------|
| `MonoGame.Framework.DesktopGL` | 3.8.4 | Runtime | XNA/MonoGame types (`Texture2D`, `SoundEffect`, `SpriteFont`, `GraphicsDevice`) |
| `NuciXNA.Primitives` | 2.1.7 | Runtime | `Colour`, `Point2D`, and `Size2D` used by the bitmap utility |
| `SixLabors.ImageSharp` | 3.1.6 | Runtime | Pixel-level image decoding and encoding for `Bitmap` |

## 🗂️ Project Structure

The solution contains the following projects:

| Project | Type | Purpose |
|---------|------|---------|
| `NuciXNA.DataAccess` | Library | The main library with content loading and bitmap utilities |
| `NuciXNA.DataAccess.UnitTests` | Test | Unit tests for the main library |

### Directories

| Directory | Purpose |
|-----------|---------|
| `NuciXNA.DataAccess/Content/` | Content loading infrastructure (`IContentLoader`, `ContentLoader`, `NuciContentManager`, `PipelineContentLoader`, `PlainFileContentLoader`) |
| `NuciXNA.DataAccess/IO/` | File I/O utilities (`Bitmap`) |

## 🏗️ Architecture

NuciXNA.DataAccess is a .NET 10 library with an interface-and-adapter content boundary, a singleton orchestration facade, and an independent bitmap adapter.

See the [architecture documentation](./ARCHITECTURE.md) for the system context, principal components, runtime flows, ownership boundaries, dependencies, constraints, and extension points.

## 🤝 Contributing

You are welcome to submit any suggestion, feedback, or modification to this project.

When doing so, please:
- Maintain cross-platform compatibility
- Preserve the existing public contract unless a breaking change is intentional
- Submit focused pull requests that conform to the existing code style
- Maintain your branch synchronised with `master`
- Revise the documentation when functionality changes
- Properly test all modifications, including edge cases and error conditions
- Add tests for additional or modified functionality
- Raise a new [issue](https://github.com/hmlendea/nucixna.dataaccess/issues) for problems or suggestions

See the [contributing guidelines](./CONTRIBUTING.md) for details on how to report issues, suggest enhancements, and submit changes.

## 🔗 Related Projects

- [NuciXNA.Graphics](https://github.com/hmlendea/nucixna.graphics): Rendering and graphics utilities for the NuciXNA ecosystem
- [NuciXNA.GUI](https://github.com/hmlendea/nucixna.gui): GUI controls and screen management for NuciXNA games
- [NuciXNA.Input](https://github.com/hmlendea/nucixna.input): Input handling (keyboard, mouse, gamepad) for NuciXNA games
- [NuciXNA.Primitives](https://github.com/hmlendea/nucixna.primitives): Primitive types (`Colour`, `Point2D`, `Size2D`) shared across the NuciXNA ecosystem

## 🔒 Security

NuciXNA.DataAccess follows coordinated disclosure for security vulnerabilities.

For information on reporting security vulnerabilities, see [SECURITY.md](./SECURITY.md).

## 💝 Project Engagement

Discovered a problem or have a suggestion? [Open an issue](https://github.com/hmlendea/nucixna.dataaccess/issues)!

If you find this project useful, consider [funding it](https://hmlendea.go.ro/funding) or starring ⭐️ it on GitHub!

[![Donate](https://raw.githubusercontent.com/hmlendea/readme-assets/master/donate_generic.png)](https://hmlendea.go.ro/funding)

## 📄 License

This project is being distributed under the `GNU General Public License v3.0` or later.
See [LICENSE](./LICENSE) for further information.
