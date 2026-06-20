# Copilot Instructions

These instructions define the default engineering policy for this repository.

## Output Language Overrides
- Write repository-generated Git commit messages in English, even when the surrounding conversation is in German.
- Apply the same English-only rule to pull request titles, pull request descriptions, release note drafts, and changelog summaries unless the user explicitly asks for another language.
- If the user asks for a Git message, return the message itself in English and do not silently localize it to German.

## Target Style
- Work like a highly experienced C# developer.
- Produce clear, calm, well-structured solutions with high readability and explicit responsibilities.
- Preserve the order and structure associated with classic Pascal-style systems without rejecting modern C# and .NET idioms.
- Use new language and runtime features only when they make the code simpler, safer, or more robust.

## Core Principles
- Make small, focused changes with a clear cause-and-effect relationship.
- Fix root causes instead of symptoms.
- Keep files, types, and methods focused on a single responsibility.
- Avoid unnecessary complexity, hidden side effects, and hard-to-follow magic.
- Preserve existing architectural decisions unless there is a strong technical reason to change them.

## Structure and Readability
- Use expressive names, clear abstractions, and stable module boundaries.
- Keep methods short, linear, and easy to follow.
- Split complex logic into small private helper methods or dedicated types.
- Order members consistently, for example: constants, static members, fields, constructors, properties, public methods, internal methods, private methods.
- Avoid broad classes with mixed responsibilities.
- Use guard clauses and clear preconditions when they improve readability.

## Modern C# Style
- Use modern language features deliberately, not decoratively.
- Prefer `async` and `await` over blocking calls.
- Use `record`, `readonly`, pattern matching, switch expressions, collection expressions, or primary constructors only when they are appropriate for the target framework and improve readability.
- Prefer strong typing and explicit domain models over loose string-driven or dictionary-driven design.
- Use `var` only when the type is obvious from the right-hand side; otherwise use explicit types.

## Architecture
- Prefer loose coupling and clearly defined contracts.
- Keep domain logic, infrastructure, UI, and integration logic properly separated.
- Introduce dependencies through constructor injection or existing composition patterns.
- Avoid static global state when testable and replaceable alternatives are available.
- Prefer provider-neutral and platform-neutral abstractions at system boundaries.

## API and Domain Design
- Design APIs to be consistent, predictable, and minimal.
- Use `nameof(...)` when string literals map to member names, parameter names, or type names.
- Centralize reusable constants instead of scattering duplicates.
- Make domain concepts visible in the code instead of hiding them behind side effects.
- Prefer explicit error handling and meaningful return contracts.

## Error Handling and Robustness
- Catch exceptions only where they can be handled, enriched, or translated meaningfully.
- Avoid empty `catch` blocks and silently swallowing errors.
- Validate inputs at meaningful boundaries.
- Write fault-tolerant code, but not at the cost of silent data corruption.

## Repository Changes
- Match the existing style, naming conventions, and layering of the repository.
- Prefer existing libraries and helper types over new dependencies.
- Add new dependencies only with a clear technical justification.
- Change only the files that are actually required for the task.
- Avoid opportunistic side work.

## Tests and Validation
- Add or update tests when behavior changes or new logic is introduced.
- Prefer small, focused tests with a clear purpose.
- Cover edge cases, failure scenarios, and relevant regressions.
- Treat validation as part of the change; do not leave changes untested when suitable tests exist or can be added easily.

## Documentation
- Document architectural decisions when they are not obvious from the code itself.
- Write concise, technical documentation in English without marketing language.
- Write code comments and API documentation in XML style and in English.
- Avoid redundant comments; prefer self-explanatory code and use XML documentation deliberately for public or otherwise non-obvious APIs.
- Prefer the XML tags `summary`, `param`, `returns`, `remarks`, and `exception` when they are appropriate.
- For public API surface areas, also use `example` when a short commented usage example improves understanding.
- Add or update README, DevOps, or architecture documentation when behavior, usage, or structure changes noticeably.

## User-Facing Text and Localization
- Use English as the default language for user-facing text, UI text, messages, prompts, and documentation intended for end users.
- Treat German as the first localization through i18n rather than as the source language.
- Avoid hard-coded localized strings when an existing localization mechanism is available.

## Git Messages
- Everything must be written in English.
- The first line contains a short summary of the most important change, with a maximum of 50 characters.
- The second line remains empty.
- The third line and the following lines contain the full description of the change.
- Use Conventional Commits whenever possible, for example: `feat: add structured command parser` or `fix: handle null widget configuration`.
- Suitable types include `feat`, `fix`, `refactor`, `test`, `docs`, `build`, and `chore`.

## Performance and Maintainability
- Do not optimize speculatively.
- Prefer understandable and correct code first, then optimize deliberately when there is a clear need.
- Watch for allocations, unnecessary repeated work, and unsuitable data structures in hot paths.
- Keep extensibility high without introducing abstractions too early.

## Copilot Working Style
- Analyze the existing code first and then change it deliberately.
- If requirements are unclear and the answer affects the implementation, ask clarifying questions.
- Do not propose workarounds that merely hide technical debt.
- If multiple good solutions exist, prefer the simpler and more maintainable one.
- When doing architecture-related work, also provide suitable tests and concise documentation.

## Preferred Outcome
- The result should be technically correct, structured, testable, and calm in style.
- Good C# solutions in this repository should be clearer than clever.
