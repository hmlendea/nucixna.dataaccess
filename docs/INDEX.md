# NuciXNA.DataAccess Documentation

## Repository Summary

`NuciXNA.DataAccess` is a .NET 10 library that provides two related capabilities for MonoGame applications: a content-loading facade with pipeline and raw-file fallback for `SoundEffect`, `SpriteFont`, and `Texture2D` assets, and a mutable bitmap utility backed by ImageSharp that exposes pixel-level access through `NuciXNA.Primitives` value types (`Colour`, `Point2D`, `Size2D`).

## Root Document Map

- [ARCHITECTURE.md](../ARCHITECTURE.md) — Verified current architecture of the library
- [README.md](../README.md) — Installation, usage, and development guide
- [LICENSE](../LICENSE) — GPL-3.0-or-later licence

## Documentation Catalogue

### Architecture & Design
- [architecture.md](./architecture.md) — High-level architecture complementing root ARCHITECTURE.md
- [design-decisions.md](./design-decisions.md) — Key architectural choices and rationale
- [repository-overview.md](./repository-overview.md) — Repository purpose, scope, and entry points
- [repository-structure.md](./repository-structure.md) — Source tree layout and module organisation

### Components
- [components/content-loading.md](./components/content-loading.md) — Content loading subsystem deep dive
- [components/bitmap-access.md](./components/bitmap-access.md) — Bitmap utility deep dive

### Flows
- [flows/initialise-content-loading.md](./flows/initialise-content-loading.md) — Content manager initialisation sequence
- [flows/load-asset.md](./flows/load-asset.md) — Asset loading with fallback and placeholder policy
- [flows/manipulate-bitmap.md](./flows/manipulate-bitmap.md) — Bitmap creation, pixel access, and save

### API Reference
- [api-reference/INDEX.md](./api-reference/INDEX.md) — API reference index and endpoint summary
- [api-reference/nuci-content-manager.md](./api-reference/nuci-content-manager.md) — NuciContentManager API
- [api-reference/icontent-loader.md](./api-reference/icontent-loader.md) — IContentLoader contract
- [api-reference/bitmap.md](./api-reference/bitmap.md) — Bitmap API

### Cross-Cutting Concerns
- [configuration.md](./configuration.md) — Configuration schema, sources, precedence
- [error-handling.md](./error-handling.md) — Error taxonomy, handling patterns, recovery
- [logging.md](./logging.md) — Logging framework, levels, structured fields, correlation, sinks
- [security.md](./security.md) — Security model, threats, mitigations
- [concurrency-and-scheduling.md](./concurrency-and-scheduling.md) — Threading, async, schedulers, locks
- [dependencies.md](./dependencies.md) — External and internal dependencies
- [invariants.md](./invariants.md) — System-wide invariants and contracts
- [state-and-persistence.md](./state-and-persistence.md) — State management, stores, caches, migrations

### Quality & Operations
- [testing.md](./testing.md) — Test strategy, organisation, coverage
- [build-and-deployment.md](./build-and-deployment.md) — Build pipeline, deployment, environments
- [change-guide.md](./change-guide.md) — How to modify common areas safely
- [troubleshooting.md](./troubleshooting.md) — Common issues and solutions
- [documentation-maintenance.md](./documentation-maintenance.md) — How to keep docs current, ownership

### Reference
- [api-usage-examples.md](./api-usage-examples.md) — Example requests/responses for API usage
- [faqs.md](./faqs.md) — Frequently asked questions
- [ambiguities-and-open-questions.md](./ambiguities-and-open-questions.md) — Unresolved items, TODOs, known gaps

## Navigation Aids

- **Start here** (newcomers): [quick-start.md](./quick-start.md) → [repository-overview.md](./repository-overview.md) → [architecture.md](./architecture.md)
- **Deep dive** (component owners): [components/](./components/) → [api-reference/](./api-reference/)
- **Flows** (debuggers): [flows/](./flows/) → [error-handling.md](./error-handling.md)

## Maintenance Metadata

- Last reviewed: 2026-10-13
- Owner: Horațiu Mlendea
- Coverage status: Comprehensive — all production components documented with implementation evidence