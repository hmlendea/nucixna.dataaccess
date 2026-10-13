# Security

## Overview

`NuciXNA.DataAccess` is a library with no network integration, no credential handling, no user data collection, and no telemetry. Security considerations focus on path handling and resource access.

## Threat Model

| Threat | Applicability | Mitigation |
|--------|---------------|------------|
| Path traversal | Host passes untrusted paths | **None** — library performs no path validation |
| Arbitrary file read | `PlainFileContentLoader` reads `.wav`/`.png` | **None** — library performs no access control |
| Arbitrary file write | `Bitmap.Save` writes to caller-provided path | **None** — library performs no access control |
| Resource exhaustion | Large image allocation via `Bitmap` | **None** — delegated to ImageSharp |
| Code execution | None | N/A — no deserialization, no dynamic code |
| Data exfiltration | None | N/A — no network, no telemetry |
| Credential exposure | None | N/A — no secrets handled |

## Security Boundaries

```
┌─────────────────────────────────────────────────────────────┐
│                    Host Application                          │
│  ┌─────────────────────────────────────────────────────┐    │
│  │ Trust Boundary: Host validates all paths            │    │
│  │ before passing to NuciXNA.DataAccess                │    │
│  └─────────────────────────────────────────────────────┘    │
│                           │                                  │
│                           ▼                                  │
│  ┌─────────────────────────────────────────────────────┐    │
│  │ NuciXNA.DataAccess Library                          │    │
│  │ - No path validation                                │    │
│  │ - No access control                                 │    │
│  │ - No sandboxing                                     │    │
│  │ - No secret handling                                │    │
│  └─────────────────────────────────────────────────────┘    │
│                           │                                  │
│           ┌───────────────┼───────────────┐                 │
│           ▼               ▼               ▼                 │
│    ┌─────────────┐ ┌─────────────┐ ┌─────────────┐         │
│    │ MonoGame    │ │ File System │ │ ImageSharp  │         │
│    │ ContentMgr  │ │ (raw files) │ │ (codecs)    │         │
│    └─────────────┘ └─────────────┘ └─────────────┘         │
└─────────────────────────────────────────────────────────────┘
```

## Path Handling

- **No normalisation:** Paths are passed through without trimming, case adjustment, or separator conversion
- **No validation:** No checks for path traversal (`..`), absolute paths, or restricted directories
- **No sandboxing:** File operations use the host process's permissions directly
- **Plain-file suffixes:** `.wav` and `.png` are appended by `PlainFileContentLoader` — host controls base path

## Resource Access

| Operation | Resource | Access Control |
|-----------|----------|----------------|
| `PipelineContentLoader` | MonoGame content pipeline | Delegated to `ContentManager` |
| `PlainFileContentLoader.LoadSoundEffect` | File system (`.wav`) | Host process permissions |
| `PlainFileContentLoader.LoadTexture2D` | File system (`.png`) | Host process permissions |
| `Bitmap.Load` | File system (image) | Host process permissions |
| `Bitmap.Save` | File system (output) | Host process permissions |

## Secrets and Credentials

**No secrets or credentials are used, stored, or processed by this library.**

- No API keys
- No connection strings
- No certificates
- No tokens
- No encryption keys

## Telemetry and Data Collection

**No telemetry, metrics, or user data collection is implemented.**

- No usage tracking
- No error reporting
- No performance monitoring
- No user identification

## Vulnerability Management

| Dependency | Current Version | Known Issues |
|------------|-----------------|--------------|
| `SixLabors.ImageSharp` | 3.1.6 | Restore reports known high- and moderate-severity vulnerabilities |
| `MonoGame.Framework.DesktopGL` | 3.8.4 | Check MonoGame advisories |
| `NuciXNA.Primitives` | 2.1.7 | Check upstream advisories |

**Recommendation:** Regularly update dependencies and monitor security advisories.

## Secure Usage Guidelines

1. **Validate all paths** in the host application before passing to `NuciXNA.DataAccess`
2. **Restrict file system permissions** for the host process to minimum required directories
3. **Monitor dependency updates** for security patches, especially ImageSharp
4. **Do not pass untrusted user input** directly as content paths
5. **Audit `Bitmap.Save` destinations** to prevent arbitrary file writes

## Related Documentation

- [Configuration](./configuration.md) — Documents the minimal programmatic configuration
- [Error Handling](./error-handling.md) — Describes exception behaviours
- [Root SECURITY.md](../SECURITY.md) — Repository-level security policy (if present)