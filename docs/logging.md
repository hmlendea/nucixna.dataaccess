# Logging

## Overview

`NuciXNA.DataAccess` does **not implement any logging**. There is no logging framework, no log output, and no diagnostic tracing.

## Logging Framework

**None.** The library does not depend on or use any logging library (e.g., Serilog, NLog, log4net, Microsoft.Extensions.Logging).

## Log Levels

**Not applicable.** No logging is performed.

## Structured Fields

**Not applicable.** No logging is performed.

## Correlation

**Not applicable.** No logging is performed, so no correlation IDs, trace IDs, or span IDs are generated or propagated.

## Sinks/Destinations

**Not applicable.** No logging is performed, so there are no sinks or destinations (console, file, database, external service).

## Redaction/Masking

**Not applicable.** No logging is performed, so no redaction or masking of sensitive data occurs.

## Retention

**Not applicable.** No logging is performed, so there are no log retention policies, rotation, or archiving.

## Sampling

**Not applicable.** No logging is performed, so no log sampling is used.

## Performance Considerations

**Not applicable.** Since no logging is performed, there is no performance impact from logging.

## Error Logging

**Not applicable.** Exceptions are not logged. Callers observe exceptions through return values or propagated exceptions (see [Error Handling](./error-handling.md)).

## Audit Logging

**Not applicable.** No security-relevant events are logged separately.

## Configuration

**Not applicable.** There is no logging configuration to change at runtime or via configuration files.

## Related Documentation

- [Error Handling](./error-handling.md) — Describes how exceptions are handled (propagated or suppressed)
- [Configuration](./configuration.md) — Documents the minimal programmatic configuration available