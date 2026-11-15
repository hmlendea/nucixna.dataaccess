# NuciXNA.DataAccess Traceability

This document is an evidence-based map of current public behaviour to implementation and NUnit verification. It complements the system model in [`ARCHITECTURE.md`](../ARCHITECTURE.md); it does not define a proposed API or imply coverage where no test exists.

## Table Of Contents

- [Repository Inventory](#repository-inventory)
- [Feature To Code And Test Map](#feature-to-code-and-test-map)
- [Content Contract Matrix](#content-contract-matrix)
- [Content Manager Decision Matrix](#content-manager-decision-matrix)
- [Bitmap Contract Matrix](#bitmap-contract-matrix)
- [Test Suite Inventory](#test-suite-inventory)
- [Uncovered Or Partially Covered Behaviour](#uncovered-or-partially-covered-behaviour)
- [Reverse Coverage Audit](#reverse-coverage-audit)
- [Change Impact Guide](#change-impact-guide)

## Repository Inventory

| Category | Evidence | Responsibility |
|----------|----------|----------------|
| Production project | [`NuciXNA.DataAccess.csproj`](../NuciXNA.DataAccess/NuciXNA.DataAccess.csproj) | `net10.0` library package and runtime dependencies. |
| Test project | [`NuciXNA.DataAccess.UnitTests.csproj`](../NuciXNA.DataAccess.UnitTests/NuciXNA.DataAccess.UnitTests.csproj) | NUnit test assembly referencing the production project. |
| Architecture test project | [`NuciXNA.DataAccess.ArchitectureTests.csproj`](../NuciXNA.DataAccess.ArchitectureTests/NuciXNA.DataAccess.ArchitectureTests.csproj) | NUnit fitness-test assembly enforcing dependency, project, and source-file conventions. |
| Content implementation | [`Content`](../NuciXNA.DataAccess/Content) | Loader contract, shared optional-load wrapper, concrete adapters, and global policy. |
| Bitmap implementation | [`Bitmap.cs`](../NuciXNA.DataAccess/IO/Bitmap.cs) | ImageSharp-backed mutable bitmap façade. |
| Content tests | [`UnitTests/Content`](../NuciXNA.DataAccess.UnitTests/Content) | Exception semantics, path forwarding, manager branch selection. |
| Bitmap tests | [`UnitTests/IO/BitmapTests.cs`](../NuciXNA.DataAccess.UnitTests/IO/BitmapTests.cs) | Dimensions, pixel mutation/access, indexers, disposal. |
| Architecture tests | [`ArchitectureTests`](../NuciXNA.DataAccess.ArchitectureTests) | Dependency boundaries, subsystem cycle freedom, project references, and project and source-file conventions. |
| Automation | [`dotnet.yml`](../.github/workflows/dotnet.yml) | Restore, build, and test on `master` push and pull request. |
| Security automation | [`security.yml`](../.github/workflows/security.yml) | CodeQL C# analysis and vulnerable-package check on `master` push, pull request, and a weekly schedule. |

## Feature To Code And Test Map

| Capability | Public Entry Point | Controlling Implementation | Verified By | Material Gap |
|------------|--------------------|----------------------------|-------------|--------------|
| Required sound loading | [`LoadSoundEffect`](../NuciXNA.DataAccess/Content/NuciContentManager.cs) | Pipeline optional attempt, then required plain-file attempt. | [`NuciContentManagerTests`](../NuciXNA.DataAccess.UnitTests/Content/NuciContentManagerTests.cs): `GivenPipelineTryLoadReturnsNull_WhenLoadSoundEffectIsCalled_ThenPlainFileLoaderIsCalled`; propagation case. | No real MonoGame pipeline or `.wav` fixture. |
| Required font loading | [`LoadSpriteFont`](../NuciXNA.DataAccess/Content/NuciContentManager.cs) | Required pipeline load only. | [`NuciContentManagerTests`](../NuciXNA.DataAccess.UnitTests/Content/NuciContentManagerTests.cs): pipeline invocation, plain loader never called, exception propagation. | No actual compiled font asset. |
| Texture loading without a placeholder | [`LoadTexture2D`](../NuciXNA.DataAccess/Content/NuciContentManager.cs) | Pipeline optional attempt, then required plain-file attempt. | [`NuciContentManagerTests`](../NuciXNA.DataAccess.UnitTests/Content/NuciContentManagerTests.cs): no-placeholder branch, path forwarding, null and exception cases. | No real `.png` fixture or graphics device. |
| Texture loading with a placeholder | [`LoadTexture2D`](../NuciXNA.DataAccess/Content/NuciContentManager.cs) and `MissingTexturePlaceholder` | Pipeline optional attempt, plain-file optional attempt, required pipeline placeholder load. | [`NuciContentManagerTests`](../NuciXNA.DataAccess.UnitTests/Content/NuciContentManagerTests.cs): placeholder call, path, null, and exception cases. | No test that an existing pipeline texture avoids plain-file and placeholder calls. |
| Loader extension contract | [`IContentLoader`](../NuciXNA.DataAccess/Content/IContentLoader.cs) | Abstract required loads plus optional wrapper in [`ContentLoader`](../NuciXNA.DataAccess/Content/ContentLoader.cs). | [`ContentLoaderTests`](../NuciXNA.DataAccess.UnitTests/Content/ContentLoaderTests.cs): every `TryLoad*` path and result category. | Concrete external implementation interoperability is not tested. |
| Pipeline adapter | [`PipelineContentLoader`](../NuciXNA.DataAccess/Content/PipelineContentLoader.cs) | Direct `ContentManager.Load<T>` delegation. | Indirectly exercised through manager mocks only; no direct adapter test. | Delegation type/path behaviour is untested against a `ContentManager`. |
| Plain-file adapter | [`PlainFileContentLoader`](../NuciXNA.DataAccess/Content/PlainFileContentLoader.cs) | Appends `.wav`/`.png`; uses MonoGame stream factories; fonts unsupported. | No direct test. | File suffixes, stream lifetime, image/audio decode, and `NotImplementedException` are untested. |
| Bitmap dimensions | [`Bitmap` constructors](../NuciXNA.DataAccess/IO/Bitmap.cs) | Constructs or wraps `Image<Rgba32>` and exposes `Size2D`. | [`BitmapTests`](../NuciXNA.DataAccess.UnitTests/IO/BitmapTests.cs): integer, `Size2D`, and image constructors. | Invalid dimensions and null wrapped image are untested. |
| Bitmap pixel access | [`GetPixel`](../NuciXNA.DataAccess/IO/Bitmap.cs), [`SetPixel`](../NuciXNA.DataAccess/IO/Bitmap.cs), indexers | Direct `Rgba32` and `Colour` channel conversion. | [`BitmapTests`](../NuciXNA.DataAccess.UnitTests/IO/BitmapTests.cs): default pixels, colour round trips, coordinate and `Point2D` forms, indexer equivalence. | Invalid/out-of-range coordinates are untested. |
| Bitmap file I/O | [`Bitmap.Load`](../NuciXNA.DataAccess/IO/Bitmap.cs), [`Bitmap.Save`](../NuciXNA.DataAccess/IO/Bitmap.cs) | Delegates decoding/encoding to ImageSharp. | No direct test. | Format inference, successful round trip, malformed input, I/O error, and overwrite behaviour are untested. |
| Bitmap ownership | [`Bitmap.Dispose`](../NuciXNA.DataAccess/IO/Bitmap.cs) | Disposes the backing `Image<Rgba32>`. | [`BitmapTests`](../NuciXNA.DataAccess.UnitTests/IO/BitmapTests.cs): disposal does not throw. | Backing-image disposal, repeated disposal, and access after disposal are untested. |

## Content Contract Matrix

[`IContentLoader`](../NuciXNA.DataAccess/Content/IContentLoader.cs) presents paired methods for each asset type. The concrete library base class implements the optional member as a wrapper around the required member.

| Asset | Required Method Contract | Optional Method Contract In `ContentLoader` | Concrete Pipeline Support | Concrete Plain-File Support |
|-------|--------------------------|---------------------------------------------|---------------------------|-----------------------------|
| `SoundEffect` | Return an asset or propagate loader failure. | Return the asset, or `null` after any exception or `null` result. | `ContentManager.Load<SoundEffect>(path)`. | `SoundEffect.FromStream(File.OpenRead(path + ".wav"))`. |
| `SpriteFont` | Return an asset or propagate loader failure. | Return the asset, or `null` after any exception or `null` result. | `ContentManager.Load<SpriteFont>(path)`. | `NotImplementedException`. |
| `Texture2D` | Return an asset or propagate loader failure. | Return the asset, or `null` after any exception or `null` result. | `ContentManager.Load<Texture2D>(path)`. | `Texture2D.FromStream(graphicsDevice, File.OpenRead(path + ".png"))`. |

### Optional-Load Invariant

For a class derived from [`ContentLoader`](../NuciXNA.DataAccess/Content/ContentLoader.cs), each `TryLoad*` method forwards the supplied path to its matching `Load*` member exactly once. It catches an unrestricted exception and returns `null`. [`ContentLoaderTests`](../NuciXNA.DataAccess.UnitTests/Content/ContentLoaderTests.cs) establishes this for `IOException`, `ArgumentException`, `InvalidOperationException`, `OutOfMemoryException`, `NullReferenceException`, returned `null`, and several paths for all three asset kinds.

This invariant does not constrain a direct `IContentLoader` implementation. The manager trusts the optional member supplied by that implementation.

## Content Manager Decision Matrix

The decision owner is [`NuciContentManager`](../NuciXNA.DataAccess/Content/NuciContentManager.cs). `LoadContent(IContentLoader, IContentLoader)` enables injected implementations; `LoadContent(ContentManager, GraphicsDevice)` constructs the built-in pair.

| Request | Predicate | Next Call | Result Or Failure |
|---------|-----------|-----------|-------------------|
| Sound | Pipeline optional load returns asset. | None. | Return pipeline asset. |
| Sound | Pipeline optional load returns `null`. | Plain-file required sound load with original path. | Return asset, returned `null`, or propagate the plain-file exception. |
| Font | Any request. | Pipeline required font load with original path. | Return asset, returned `null`, or propagate exception. Plain-file loader is never selected. |
| Texture | Pipeline optional load returns asset. | None. | Return pipeline asset. |
| Texture | Pipeline optional load returns `null`; placeholder is `null`, empty, or whitespace. | Plain-file required texture load with original path. | Return asset, returned `null`, or propagate exception. |
| Texture | Pipeline optional load returns `null`; placeholder has non-whitespace content; plain optional load returns asset. | None. | Return plain-file asset. |
| Texture | Both optional loads return `null`; placeholder has non-whitespace content. | Pipeline required texture load using placeholder path. | Return placeholder asset, returned `null`, or propagate exception. |

The test fixture resets the static placeholder and replaces singleton loader fields during every `SetUp`. This keeps its mocked scenarios independent but demonstrates that tests share process-wide singleton state.

## Bitmap Contract Matrix

| Surface | Verified Implementation Detail | Tested Scenario | Known Boundary |
|---------|-------------------------------|-----------------|----------------|
| `Bitmap(int, int)` | Allocates `Image<Rgba32>` and constructs `Size2D` from the same dimensions. | Multiple wide, tall, square, and 1x1 cases. | ImageSharp validates unsupported dimensions. |
| `Bitmap(Size2D)` | Delegates to integer constructor. | Reported size matches `Size2D`. | No invalid `Size2D` test. |
| `Bitmap(Image<Rgba32>)` | Retains provided reference and derives `Size`. | Reported size matches image. | Documentation states no ownership transfer, but `Dispose` invokes the wrapped image's `Dispose`; callers must not use it afterwards. |
| `GetPixel` | Converts ImageSharp `(R,G,B,A)` to `Colour.FromArgb(A,R,G,B)`. | Transparent default and several opaque/semi-transparent colours. | No bounds or disposal tests. |
| `SetPixel` | Converts `Colour` to `Rgba32(R,G,B,A)`. | Read-after-write, overwrite, location isolation. | No concurrent mutation guarantee. |
| Indexers and `Point2D` overloads | Delegate to integer-coordinate accessors. | Cross-form equivalence tests. | No additional validation. |
| `Load` / `Save` | Delegates codec work to ImageSharp. | None. | Caller receives all decoding, encoding, path, and I/O failures. |

## Test Suite Inventory

| Fixture | Test Strategy | Behaviour Established |
|---------|---------------|-----------------------|
| [`ContentLoaderTests`](../NuciXNA.DataAccess.UnitTests/Content/ContentLoaderTests.cs) | Internal throwing, null-returning, and spy subclasses. | Catch-all optional loads return `null`, never throw for chosen exception types, and preserve paths. |
| [`NuciContentManagerTests`](../NuciXNA.DataAccess.UnitTests/Content/NuciContentManagerTests.cs) | Moq `IContentLoader` doubles injected into the singleton. | Per-asset ordering, selected method, path forwarding, disallowed font fallback, placeholder switch, and propagated required-load exceptions. |
| [`BitmapTests`](../NuciXNA.DataAccess.UnitTests/IO/BitmapTests.cs) | Real in-memory ImageSharp images. | Constructor dimensions, transparent default, colour/channel round trips, overload/indexer consistency, and non-throwing disposal. |
| [`DependencyArchitectureTests`](../NuciXNA.DataAccess.ArchitectureTests/DependencyArchitectureTests.cs) | ArchUnitNET rules over the loaded production assembly and parsed project files. | Content and IO type-dependency isolation, subsystem cycle freedom, `InternalsVisibleTo` absence, production-to-test reference absence, and approved package references. |
| [`ProjectConventionTests`](../NuciXNA.DataAccess.ArchitectureTests/ProjectConventionTests.cs) | XML parsing of every project file and the solution file. | Target framework, implicit usings, language version, nullable references, packability and test flags, alphabetical package ordering, and solution structure and ordering. |
| [`SourceFileConventionTests`](../NuciXNA.DataAccess.ArchitectureTests/SourceFileConventionTests.cs) | Roslyn syntax analysis of every handwritten production and test source file. | One top-level type per file, exact filename-to-type matching, block namespaces, namespace spacing, alphabetised using groups, absence of inline fully qualified types, and absence of `#nullable` directives. |
| [`ProductionCodeConventionTests`](../NuciXNA.DataAccess.ArchitectureTests/ProductionCodeConventionTests.cs) | Roslyn syntax analysis and raw source inspection of production sources. | Prohibited constructs (regions, global usings, `var`, ternaries, `const`, `init`, `ref`/`out`, optional parameters, tuples, `== null`, `??`, empty catches, exception rethrows, partials, custom exceptions), member accessibility, sealed classes, XML documentation, braced control flow, expression-bodied methods, and formatting. |
| [`TestSourceConventionTests`](../NuciXNA.DataAccess.ArchitectureTests/TestSourceConventionTests.cs) | Roslyn syntax analysis of the test sources. | Top-level test helpers, sealed `Tests`-suffixed fixtures, `Given_When_Then` naming, minimum two `TestCase` values, constraint-model assertions, and no reflection. |

Observed validation command:

```bash
dotnet test NuciXNA.DataAccess.slnx
```

On the investigated repository state, the command discovered and passed 364 unit cases and 78 architecture cases. `dotnet list package --vulnerable --include-transitive` reported no vulnerable packages for any project.

## Uncovered Or Partially Covered Behaviour

- Built-in adapter integration: no sound, texture, content-pipeline, `GraphicsDevice`, or `ContentManager` fixture validates the actual MonoGame adapters.
- Initialisation contract: no test validates behaviour prior to `LoadContent`, successive reconfiguration, `LoadContent` argument validation, or lazy singleton identity.
- Concurrency: singleton construction is guarded, but configuration mutation and calls are untested under concurrent access.
- Successful file I/O: no test calls `Bitmap.Load` or `Bitmap.Save`, including round-trip pixel equality.
- Failure I/O: no malformed image, inaccessible path, invalid output extension, invalid bitmap dimension, invalid coordinate, or disposed-object case is asserted.
- Resource ownership: tests only establish that `Dispose` does not throw; they do not establish what happens to a wrapped ImageSharp image after disposal.
- Documentation mismatch risk: the README says the missing-texture placeholder is returned when neither loader locates an asset, but the implementation can instead propagate a pipeline placeholder-load exception or return `null` from a custom loader.

## Reverse Coverage Audit

Every production file and material public member is represented in [`ARCHITECTURE.md`](../ARCHITECTURE.md) and in this map:

| Production File | Represented Behaviours | Test Representation |
|-----------------|------------------------|---------------------|
| [`IContentLoader.cs`](../NuciXNA.DataAccess/Content/IContentLoader.cs) | Six-method public asset contract. | Mocked in manager tests; inherited by `ContentLoader` test doubles. |
| [`ContentLoader.cs`](../NuciXNA.DataAccess/Content/ContentLoader.cs) | Generic catch-all optional load wrapper. | Directly exercised by all three method groups in `ContentLoaderTests`. |
| [`PipelineContentLoader.cs`](../NuciXNA.DataAccess/Content/PipelineContentLoader.cs) | Typed `ContentManager.Load<T>` forwarding. | No direct implementation test. |
| [`PlainFileContentLoader.cs`](../NuciXNA.DataAccess/Content/PlainFileContentLoader.cs) | Fixed suffix file reads and unsupported font operation. | No direct implementation test. |
| [`NuciContentManager.cs`](../NuciXNA.DataAccess/Content/NuciContentManager.cs) | Singleton composition and load-selection matrix. | Mock-driven branch tests; concurrency/composition gaps remain. |
| [`Bitmap.cs`](../NuciXNA.DataAccess/IO/Bitmap.cs) | Construction, RGBA/ARGB conversion, pixel access, I/O, disposal. | In-memory construction/pixel/disposal only; I/O and error paths remain gaps. |

## Change Impact Guide

| Change | Review Required | Regression Tests Required |
|--------|-----------------|---------------------------|
| Change `TryLoad*` exception filtering | [`ContentLoader.cs`](../NuciXNA.DataAccess/Content/ContentLoader.cs), manager fallback paths, public interface docs. | All three `ContentLoaderTests` method groups and manager fallback tests. |
| Change asset precedence or placeholder semantics | [`NuciContentManager.cs`](../NuciXNA.DataAccess/Content/NuciContentManager.cs), README, architecture decision matrix. | `NuciContentManagerTests`, including positive pipeline-success non-invocation assertions that are presently absent. |
| Add a content type | [`IContentLoader.cs`](../NuciXNA.DataAccess/Content/IContentLoader.cs), every loader, manager, README, architecture, tests. | Required/optional exception matrix and manager decision matrix for the new type. |
| Change raw-file formats or suffixes | [`PlainFileContentLoader.cs`](../NuciXNA.DataAccess/Content/PlainFileContentLoader.cs), README limitations, compatibility contracts. | New direct adapter fixtures using actual files and MonoGame resources. |
| Change `Colour` mapping or bitmap backing format | [`Bitmap.cs`](../NuciXNA.DataAccess/IO/Bitmap.cs), primitive compatibility. | Alpha and channel-specific round trips plus saved-file/read-back coverage. |
| Change bitmap ownership or disposal | [`Bitmap.cs`](../NuciXNA.DataAccess/IO/Bitmap.cs), public XML documentation, architecture. | Wrapped-image and repeated-disposal behavioural tests. |
| Upgrade ImageSharp | [production project file](../NuciXNA.DataAccess/NuciXNA.DataAccess.csproj), lock-free restore artefacts, release verification. | Full test suite plus image file I/O regression tests, currently absent. |
