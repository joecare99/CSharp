# Contributing

Thank you for contributing to this C# framework. This document describes conventions and best practices to keep the codebase consistent, maintainable and well-tested.

## Table of contents
- Purpose
- Getting started
- Repository layout
- Branching & workflow
- Coding conventions
- Architecture & project organization
- Dependency injection & configuration
- Asynchronous code & cancellation
- Error handling & logging
- Testing strategy
- Continuous integration
- Pull request checklist
- Releases & versioning
- Tools & commands
- Contacts

## Purpose
This repository targets a structured, test-first C# framework. Follow these guidelines to ensure code quality, predictable behavior and easy review.

## Getting started
- Prerequisites:
  - `dotnet` SDK (match repository `global.json`).
  - IDE with C# support (Visual Studio, Rider, VS Code).
  - Install recommended analyzers and formatter extensions.
- Local setup:
  - Clone repository and run `dotnet restore`.
  - Run `dotnet build` and `dotnet test` to verify baseline.

## Repository layout (recommended)
- `src/` - production projects
  - `Core` - domain, interfaces, DTOs
  - `Application` - use-cases, services
  - `Infrastructure` - external integrations, persistence
  - `Api` or `Host` - web/api entrypoints
- `tests/` - test projects
  - `Unit` - unit tests
  - `Integration` - integration tests
  - `E2E` - end-to-end tests
- `build/` or `ci/` - CI scripts
- `docs/` - design and API docs

Use clear project names and keep projects small and focused.

## Branching & workflow
- Use a branching model like `main` (or `master`) + feature branches:
  - Branches: `feature/<name>`, `bugfix/<issue>`, `hotfix/<issue>`.
- Pull requests:
  - Target `main` (or develop -> main if using GitFlow).
  - Include issue number and short description.
  - Require CI passing and approvals (see PR checklist).

## Coding conventions
- Target .NET versions as defined in the repository.
- Enable nullable reference types (`<Nullable>enable</Nullable>`).
- Follow .NET naming conventions (`PascalCase` for types/methods, `camelCase` for parameters and private fields prefixed with `_` if used).
- Keep methods small; prefer single responsibility.
- Use `async`/`await` for I/O-bound work. Avoid `async void`.
- Prefer interfaces for public-facing components.
- Prefer expression-bodied members for short members.
- Use `using` declarations and `IAsyncDisposable` where appropriate.
- Prefer immutable DTOs and avoid mutable shared state.

## Style & analyzers
- Add and respect `.editorconfig`.
- Run `dotnet format` and enable analyzers:
  - `Microsoft.CodeAnalysis.FxCopAnalyzers` / Roslyn analyzers
  - `StyleCop.Analyzers` (optional)
- Treat warnings as errors in CI where feasible.
- Use `IDE` and `analyzer` rules to enforce rules automatically.

## Architecture & project organization
- Separate concerns: `Core` (domain), `Application` (business logic), `Infrastructure` (persistence, external services), `Api` (host).
- Keep `Core` free of framework-specific dependencies.
- Use DTOs for boundaries and map with minimal mapping logic.
- Use `Options` pattern for configuration (`IOptions<T>` / `IOptionsSnapshot<T>`).
- Register services via `IServiceCollection` extension methods in each project: provide `AddXxxServices(this IServiceCollection services, IConfiguration config)`.

## Dependency injection & configuration
- Constructor inject dependencies; avoid service locator pattern.
- Prefer small interfaces (interface segregation).
- Use `Func<T>` or `IServiceProvider` only when necessary.
- For optional dependencies, use `TryAdd` semantics or `IOptions<T>` defaults.

## Asynchronous code & cancellation
- Accept `CancellationToken` on APIs that may be long-running.
- Propagate `CancellationToken` through call chain.
- Avoid blocking calls (`.Result`, `.GetAwaiter().GetResult()`).

## Error handling & logging
- Throw meaningful exceptions or return `Result`/`OneOf`/`Either`-like types for expected failures.
- Log structured events with `ILogger<T>`.
- Do not swallow exceptions; wrap with contextual information if rethrowing.
- Use `ProblemDetails` in HTTP APIs for consistent error shapes.

## Logging
- Use `Microsoft.Extensions.Logging` and structured logging.
- Log at appropriate levels (`Debug`, `Information`, `Warning`, `Error`, `Critical`).
- Avoid logging sensitive data.

## Testing strategy
- Unit tests:
  - Use `MSTest` v4 (or chosen test framework).
  - Use `FluentAssertions` for assertions.
  - Use `NSubstitute` for mocking; prefer `NSubstitute` for readability if chosen.
  - Naming convention: `MethodName_StateUnderTest_ExpectedBehavior`.
  - Arrange / Act / Assert structure.
  - Keep tests fast and isolated; avoid touching external resources.
- Integration tests:
  - Use testcontainers or in-memory providers where possible.
  - Run longer tests in `tests/integration` and isolated CI stages.
- E2E tests:
  - Run against controlled environments.
- Test fixtures:
  - Use shared fixtures sparingly; reset state between tests.
- Coverage:
  - Aim for meaningful coverage; protect critical paths with tests, not artificial coverage.
- Avoid flakiness: ensure tests are deterministic.

## Continuous Integration (CI)
- CI must run on PRs and main merges.
- Required checks:
  - `dotnet build --configuration Release`
  - `dotnet test --no-build --configuration Release`
  - `dotnet format --verify-no-changes`
  - Static analysis and analyzers
  - Optional: code coverage report and threshold
- Fail PR if any required check fails.

## Pull Request checklist
Before requesting review:
- [ ] CI passes (build, tests, analyzers).
- [ ] Code formatted (`dotnet format`).
- [ ] New behavior covered by tests.
- [ ] No sensitive data or credentials.
- [ ] Update changelog or docs if public API changed.
- [ ] Add migration notes for breaking changes.

Reviewers should check:
- Correctness & tests
- Design & architecture fit
- Performance implications
- Security considerations
- Clear commit messages

## Releases & versioning
- Follow semantic versioning (MAJOR.MINOR.PATCH).
- Document breaking changes in `CHANGELOG.md`.
- For breaking changes, increment major and document migration steps.

## Tools & common commands
- Build: `dotnet build`
- Restore: `dotnet restore`
- Test: `dotnet test`
- Run formatter: `dotnet format`
- Run analyzers: configured in CI / IDE
- Run a single test project: `dotnet test ./tests/Project.Tests/Project.Tests.csproj`
- Add migration / update DB: follow infra README

## Security & dependencies
- Keep dependencies updated; prefer minimal dependencies.
- Run automated dependency checks (Dependabot, Renovate).
- Scan for secrets and vulnerabilities before merge.

## Performance
- Profile before optimizing.
- Prefer asynchronous non-blocking patterns.
- Cache responsibly; invalidate correctly.

## Documentation
- Document public APIs with XML comments.
- Keep high-level architecture docs in `docs/`.
- Update docs as part of PR when behavior or configuration changes.

## Troubleshooting
- If CI fails, reproduce locally with the same commands used by the pipeline.
- For flaky tests, add logs and isolate to a local run.

## Contacts
- For contribution questions, open an issue or contact the maintainers listed in `README.md`.

Thank you for helping keep this framework consistent and reliable.