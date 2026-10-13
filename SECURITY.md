# Security Policy

This document describes the security maintenance scope, vulnerability reporting process, and disclosure expectations for the `NuciXNA.DataAccess` .NET library.

## 📑 Table of Contents

- [Supported Versions](#supported-versions)
- [Reporting a Vulnerability](#reporting-a-vulnerability)
- [Scope](#scope)
- [Disclosure Policy](#disclosure-policy)

## 🛡️ Supported Versions

Use this table to indicate which project versions currently receive security maintenance.

| Version | Distribution Channel | Supported |
|---------|--------------------|-----------|
| Latest version | NuGet.org | ✅ |
| Preceding versions | NuGet.org | ❌ |

## 🚨 Reporting a Vulnerability

Please do not disclose suspected vulnerabilities publicly before maintainers have had an opportunity to validate and remediate them.

To report a vulnerability:
- [GitHub Security Advisories](https://github.com/hmlendea/nucixna.dataaccess/security/advisories)
- Contact the maintainers directly via GitHub issues or the repository contact information

## 📌 Scope

The subsequent report categories are in scope for this repository:
- Vulnerabilities in the `NuciXNA.DataAccess` library code (`NuciXNA.DataAccess/` project)
- Vulnerabilities in the build, test, or CI pipeline configuration (`.github/workflows/`, `.csproj`, `.sln`)
- Supply-chain vulnerabilities in declared dependencies (`MonoGame.Framework.DesktopGL`, `NuciXNA.Primitives`, `SixLabors.ImageSharp`)

The subsequent categories are out of scope unless explicitly stated to the contrary:
- Vulnerabilities in the MonoGame framework itself (report to MonoGame maintainers)
- Vulnerabilities in ImageSharp itself (report to SixLabors maintainers)
- Vulnerabilities in NuciXNA.Primitives itself (report to that project's maintainers)
- Vulnerabilities in the .NET runtime or SDK (report to Microsoft)
- Issues in applications that consume this library (report to the consuming application maintainers)
- Social engineering, phishing, or physical security attacks

## 📢 Disclosure Policy

This project follows coordinated disclosure:
1. Vulnerabilities are investigated privately.
2. A remediation plan is prepared and validated.
3. Public disclosure is published after a fix, mitigation, or agreed risk decision is available.
4. Credit is attributed in accordance with reporter preference and project policy.

## 🧾 Safe Harbour

If your research is conducted in good faith, confined to authorised scope, and disclosed responsibly, the maintainers will not pursue action for policy-compliant activity.

## 🙏 Recognition

We appreciate responsible disclosure. Reporters who desire public attribution may be acknowledged in release notes, advisories, or a dedicated acknowledgements section.