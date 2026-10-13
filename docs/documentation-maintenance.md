# Documentation Maintenance

## Ownership

- **Primary maintainer:** Horațiu Mlendea
- **Review cadence:** Per release or when architecture changes

## Update Triggers

| Trigger | Action |
|---------|--------|
| New feature added | Update relevant component, flow, API reference docs |
| Behaviour changed | Update affected docs + changelog |
| Bug fixed | Update troubleshooting if user-facing |
| Dependency updated | Update dependencies.md, security.md |
| Test coverage changed | Update testing.md |
| Configuration added | Update configuration.md |

## Documentation Structure

```
docs/
├── INDEX.md                          # Master index (update on new files)
├── architecture.md                   # High-level architecture
├── design-decisions.md               # Key decisions and rationale
├── repository-overview.md            # Purpose, scope, entry points
├── repository-structure.md           # Source tree layout
├── components/
│   ├── content-loading.md            # Content loading subsystem
│   └── bitmap-access.md              # Bitmap subsystem
├── flows/
│   ├── initialise-content-loading.md # Initialisation sequence
│   ├── load-asset.md                 # Asset loading with fallback
│   └── manipulate-bitmap.md          # Bitmap lifecycle
├── api-reference/
│   ├── INDEX.md                      # API index
│   ├── nuci-content-manager.md       # NuciContentManager API
│   ├── icontent-loader.md            # IContentLoader contract
│   └── bitmap.md                     # Bitmap API
├── api-usage-examples.md             # Usage examples
├── configuration.md                  # Configuration schema
├── error-handling.md                 # Error taxonomy and patterns
├── logging.md                        # Logging (none implemented)
├── security.md                       # Security model
├── concurrency-and-scheduling.md     # Threading model
├── dependencies.md                   # External/internal dependencies
├── invariants.md                     # System-wide invariants
├── state-and-persistence.md          # State management
├── testing.md                        # Test strategy and coverage
├── build-and-deployment.md           # Build pipeline and deployment
├── change-guide.md                   # How to modify common areas
├── troubleshooting.md                # Common issues and solutions
├── documentation-maintenance.md      # This file
├── faqs.md                           # Frequently asked questions
├── ambiguities-and-open-questions.md # Known gaps
└── quick-start.md                    # Getting started guide
```

## Update Process

1. **Make code changes**
2. **Update affected documentation** in same PR/commit
3. **Verify links** — check relative links resolve
4. **Run tests** — ensure behaviour matches docs
5. **Update INDEX.md** if new files added

## Link Validation

All cross-references use relative paths from `docs/` root:
- `[Architecture](./architecture.md)`
- `[NuciContentManager API](../api-reference/nuci-content-manager.md)`
- `[Root ARCHITECTURE.md](../ARCHITECTURE.md)`

## Style Guidelines

- **Headings:** Sentence case (`## Repository purpose`)
- **Files:** kebab-case (`content-loading.md`)
- **Code symbols:** Backticks with full namespace (``NuciXNA.DataAccess.Content.NuciContentManager``)
- **Code blocks:** Language specified (`csharp`, `bash`, `mermaid`)

## Root Document Integration

- Reference root `ARCHITECTURE.md`, `SECURITY.md`, `ROADMAP.md`, `PRIVACY.md`, `LICENSE` from `INDEX.md`
- Do not duplicate root content under `docs/`
- Complement, don't replicate

## Versioning

Documentation version tracks library version (from `NuciXNA.DataAccess.csproj` `<Version>`).

## Related Documentation

- [Change Guide](./change-guide.md)
- [Root ARCHITECTURE.md](../ARCHITECTURE.md)
- [Root README.md](../README.md)