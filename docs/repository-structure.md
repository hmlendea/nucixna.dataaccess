# Repository Structure

## Source Tree Layout

```
NuciXNA.DataAccess/
├── ARCHITECTURE.md                 # Verified current architecture
├── LICENSE                         # GPL-3.0-or-later
├── NuciXNA.DataAccess.slnx          # Solution file
├── README.md                       # Installation, usage, development
├── docs/                           # Generated documentation (this directory)
│   ├── INDEX.md
│   ├── architecture.md
│   ├── design-decisions.md
│   ├── repository-overview.md
│   ├── repository-structure.md
│   ├── components/
│   │   ├── content-loading.md
│   │   └── bitmap-access.md
│   ├── flows/
│   │   ├── initialise-content-loading.md
│   │   ├── load-asset.md
│   │   └── manipulate-bitmap.md
│   ├── api-reference/
│   │   ├── INDEX.md
│   │   ├── nuci-content-manager.md
│   │   ├── icontent-loader.md
│   │   └── bitmap.md
│   ├── api-usage-examples.md
│   ├── configuration.md
│   ├── error-handling.md
│   ├── logging.md
│   ├── security.md
│   ├── concurrency-and-scheduling.md
│   ├── dependencies.md
│   ├── invariants.md
│   ├── state-and-persistence.md
│   ├── testing.md
│   ├── build-and-deployment.md
│   ├── change-guide.md
│   ├── troubleshooting.md
│   ├── documentation-maintenance.md
│   ├── faqs.md
│   ├── ambiguities-and-open-questions.md
│   └── quick-start.md
├── NuciXNA.DataAccess/             # Production library
│   ├── NuciXNA.DataAccess.csproj   # Project file (net10.0)
│   ├── Content/                    # Content loading subsystem
│   │   ├── ContentLoader.cs        # Abstract base with TryLoad* wrappers
│   │   ├── IContentLoader.cs       # Public contract
│   │   ├── NuciContentManager.cs   # Singleton façade
│   │   ├── PipelineContentLoader.cs # MonoGame pipeline adapter
│   │   └── PlainFileContentLoader.cs # Raw-file adapter
│   └── IO/                         # Bitmap subsystem
│       └── Bitmap.cs               # ImageSharp façade
└── NuciXNA.DataAccess.UnitTests/   # Unit test project
    ├── NuciXNA.DataAccess.UnitTests.csproj
    ├── Content/
    │   ├── ContentLoaderTests.cs   # ContentLoader base class tests
    │   ├── NuciContentManagerTests.cs # Manager orchestration tests
    │   ├── ThrowingContentLoader.cs   # Throwing test double
    │   ├── NullReturningContentLoader.cs # Null-returning test double
    │   └── SpyContentLoader.cs        # Path-capturing test double
    └── IO/
        ├── BitmapTests.cs             # Bitmap construction, pixel access, disposal
        ├── TempFile.cs                # Temporary file helper
        └── TempDirectory.cs           # Temporary directory helper
└── NuciXNA.DataAccess.ArchitectureTests/ # Architecture test project
    ├── ArchitectureTestPaths.cs    # Repository path resolution
    ├── SourceFileIndex.cs           # Cached Roslyn parse of every source file
    ├── SourceText.cs                # Raw source lines for formatting rules
    ├── DependencyArchitectureTests.cs # Dependency and reference boundaries
    ├── ProjectConventionTests.cs   # Project and solution conventions
    ├── ProductionCodeConventionTests.cs # C# coding conventions
    ├── SourceFileConventionTests.cs # Source-file conventions
    └── TestSourceConventionTests.cs # Test-source conventions
```

## Module Organisation

### NuciXNA.DataAccess (Production)

| Directory | Namespace | Responsibility |
|-----------|-----------|----------------|
| `Content/` | `NuciXNA.DataAccess.Content` | Asset loading contract, adapters, orchestration |
| `IO/` | `NuciXNA.DataAccess.IO` | Bitmap façade over ImageSharp |

### NuciXNA.DataAccess.UnitTests (Tests)

| Directory | Namespace | Responsibility |
|-----------|-----------|----------------|
| `Content/` | `NuciXNA.DataAccess.UnitTests.Content` | Content loading tests and test doubles |
| `IO/` | `NuciXNA.DataAccess.UnitTests.IO` | Bitmap tests and file helpers |

### NuciXNA.DataAccess.ArchitectureTests (Tests)

| File | Namespace | Responsibility |
|------|-----------|----------------|
| `/` | `NuciXNA.DataAccess.ArchitectureTests` | Dependency, project, production-code, source-file, and test-source fitness tests |

## Project Files

### NuciXNA.DataAccess.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <RootNamespace>NuciXNA.DataAccess</RootNamespace>
    <ImplicitUsings>disable</ImplicitUsings>
    <LangVersion>latest</LangVersion>
    <Nullable>enable</Nullable>
    <Version>4.0.5</Version>
    <PackageLicenseExpression>GPL-3.0-or-later</PackageLicenseExpression>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="MonoGame.Framework.DesktopGL" Version="3.8.4" />
    <PackageReference Include="NuciXNA.Primitives" Version="2.1.8" />
    <PackageReference Include="SixLabors.ImageSharp" Version="4.1.2" />
  </ItemGroup>
</Project>
```

### NuciXNA.DataAccess.UnitTests.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <RootNamespace>NuciXNA.DataAccess.UnitTests</RootNamespace>
    <ImplicitUsings>disable</ImplicitUsings>
    <LangVersion>latest</LangVersion>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
    <ExcludeFromCodeCoverage>true</ExcludeFromCodeCoverage>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="18.10.1" />
    <PackageReference Include="MonoGame.Framework.DesktopGL" Version="3.8.4" />
    <PackageReference Include="Moq" Version="4.21.0" />
    <PackageReference Include="NUnit" Version="5.0.0" />
    <PackageReference Include="NUnit3TestAdapter" Version="6.3.1" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\NuciXNA.DataAccess\NuciXNA.DataAccess.csproj" />
  </ItemGroup>
</Project>
```

### NuciXNA.DataAccess.ArchitectureTests.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <RootNamespace>NuciXNA.DataAccess.ArchitectureTests</RootNamespace>
    <ImplicitUsings>disable</ImplicitUsings>
    <LangVersion>latest</LangVersion>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
    <ExcludeFromCodeCoverage>true</ExcludeFromCodeCoverage>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.CodeAnalysis.Common" Version="4.14.0" />
    <PackageReference Include="Microsoft.CodeAnalysis.CSharp" Version="4.14.0" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="18.10.1" />
    <PackageReference Include="NUnit" Version="5.0.0" />
    <PackageReference Include="NUnit3TestAdapter" Version="6.3.1" />
    <PackageReference Include="TngTech.ArchUnitNET" Version="0.13.5" />
    <PackageReference Include="TngTech.ArchUnitNET.NUnit" Version="0.13.5" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\NuciXNA.DataAccess\NuciXNA.DataAccess.csproj" />
  </ItemGroup>
</Project>
```

## Key Files by Concern

### Content Loading

| File | Role |
|------|------|
| `Content/IContentLoader.cs` | Public contract (6 methods) |
| `Content/ContentLoader.cs` | Abstract base with `TryLoad<T>` |
| `Content/PipelineContentLoader.cs` | Pipeline adapter |
| `Content/PlainFileContentLoader.cs` | Raw-file adapter |
| `Content/NuciContentManager.cs` | Singleton façade and policy |

### Bitmap Access

| File | Role |
|------|------|
| `IO/Bitmap.cs` | Complete implementation |

### Tests

| File | Role |
|------|------|
| `UnitTests/Content/ContentLoaderTests.cs` | `TryLoad*` exception suppression |
| `UnitTests/Content/NuciContentManagerTests.cs` | Manager policy and fallback |
| `UnitTests/IO/BitmapTests.cs` | Construction, pixel access, disposal |

## Build Output

```
bin/
├── Debug/
│   └── net10.0/
│       ├── NuciXNA.DataAccess.dll
│       ├── NuciXNA.DataAccess.pdb
│       └── (dependencies)
└── Release/
    └── net10.0/
        ├── NuciXNA.DataAccess.dll
        ├── NuciXNA.DataAccess.pdb
        └── (dependencies)
```

## Documentation Files (Generated)

All files under `docs/` are generated documentation complementing the root `ARCHITECTURE.md` and `README.md`.

## Related Documentation

- [Repository Overview](./repository-overview.md)
- [Architecture](./architecture.md)
- [Build and Deployment](./build-and-deployment.md)