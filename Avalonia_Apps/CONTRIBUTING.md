# Contributing Guide

Thank you for contributing. This document defines the project’s engineering standards and best practices.

## Quick checklist (before opening a PR)
- Follow MVVM using CommunityToolkit (partial property style).
- Use DI with Microsoft.Extensions.DependencyInjection.
- One class per file, meaningful names.
- Fully document public APIs with XML docs.
- Write/extend tests first (TDD) with MSTest v4.
- Parameterize edge/single cases with `[DataRow]`.
- Mock external dependencies with NSubstitute.
- Keep code analyzers clean; treat warnings as errors.
- Run `dotnet build` and `dotnet test` locally.

## Supported SDKs
- Multi-targeting across .NET (e.g., net6.0, net7.0, net8.0, net9.0, net10.0) and .NET Framework where applicable (e.g., net481, net48, net472, net462).
- Use the latest stable Visual Studio and .NET SDKs installed.

## Repository layout
- `src/<ProjectName>/` – Production code
- `tests/<ProjectName>.Tests/` – Test projects (MSTest v4)
- One class per file; file name = class name.

## Coding standards (C#)
- Enable nullable reference types in all projects.
- Treat warnings as errors in CI and locally.
- Keep methods small and cohesive.
- Prefer `async/await`; do not block on async.
- Avoid `static` state and singletons; prefer DI.
- Use `var` where type is obvious.
- Avoid magic strings/numbers; extract constants.
- Prefer expression-bodied members when clear.
- Always check arguments; throw `ArgumentException` variants for invalid input.

### Documentation
- XML-doc all public/protected types and members.
- Document intent, constraints, and side-effects.
- Use `<param>`, `<returns>`, `<exception>`, `<remarks>`.
- Example: