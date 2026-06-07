# Backlog Item BI-RepoMigrator-013 - Create reusable CommandlineHelper infrastructure

## Status

Draft

## Parent

- Cross-cutting helper initiative for reusable developer tooling and application infrastructure

## Goal

Define and implement a reusable console-command helper so applications and tools can describe command-line options declaratively and reuse generated parsing, validation, usage, and help output.

## Description

Several console tools in this workspace repeat the same entry-point boilerplate: parse raw arguments, validate required values, print usage/help text, and then invoke the actual tool service. Current RepoMigrator tools such as `RepoMigrator.Tools.GitBranchSplitter`, `RepoMigrator.Tools.PipelinedMigration`, and `RepoMigrator.Tools.ArchiveSmokeTest` are the first identified adopters, but the infrastructure itself should be tracked as a standalone general-purpose helper.

The desired target state is a shared attribute-driven model based on dedicated options classes. Tool authors should declare command metadata and option metadata on properties while the helper infrastructure generates the repetitive command-line parsing and help-writing code. The first slice should keep existing `Program.cs` files as orchestration roots for dependency injection and runtime execution, while removing the hand-written parsing and usage boilerplate from the first adopters.

User-facing texts must respect localization requirements. Command descriptions, option descriptions, and longer help text should therefore be resolved indirectly through resource files rather than being embedded directly as user-visible string literals in generated or hand-written command metadata.

The design should prefer convention over configuration where possible. Requiredness and default values should normally be inferred from property shape such as `required`, nullable annotations, and property initializers. Optional attribute properties may override the inferred behavior explicitly when the CLI contract needs to differ from the basic property model.

## Scope

- Define a reusable command metadata model for console tools based on dedicated options classes
- Support resource-based command description and extended help text at command level
- Support resource-based option descriptions at property level
- Infer requiredness and default values from property shape where practical
- Allow optional attribute overrides that take precedence over inference
- Generate parsing, validation, and usage/help output code through source generation
- Retain existing `Program.cs` orchestration for service registration and tool execution in the first slice
- Adopt the helper in the existing three RepoMigrator console tools as initial consumers
- Add or update tests that cover the shared infrastructure and the initial consumers

## Acceptance Criteria

- A shared console command model exists for declarative option definitions on dedicated options classes
- Generated parsing can replace the current hand-written parsing in the three existing RepoMigrator console tools as initial adopters
- Command-level description and extended help can be resolved from resources
- Option-level descriptions can be resolved from resources
- Requiredness is inferred by default and can be overridden explicitly by attribute metadata
- Default values are inferred by default and can be overridden explicitly by attribute metadata
- Existing adopter tools keep their entry-point orchestration in `Program.cs` in the first implementation slice
- Relevant tests cover parsing, validation, help generation, and helper integration behavior

## Assumptions

- A shared runtime project plus a source-generator project is an acceptable implementation shape for this repository
- Dedicated options classes are preferred over static mutable fields on `Program`
- The first implementation slice does not need subcommands or nested command trees
- Initial default-value inference may reasonably be limited to simple constant or literal property initializers

## Risks

- Resource lookup and generator output may become tightly coupled if command metadata contracts are not kept small and explicit
- Over-aggressive inference rules may produce surprising CLI behavior when property semantics and CLI semantics diverge
- Generated help formatting may need further refinement once more tools adopt the model
- Migrating initial adopter tools may expose edge cases in current argument handling that were not previously documented

## Open Questions

- Should the generated API surface expose `Parse`, `TryParse`, and a richer parse-result type, or only a single parse entry point?
- Should examples also be modeled through resources in the first slice, or deferred until the command description and help blocks are stable?
- Should the helper eventually support non-console hosts such as WPF-triggered command composition, or stay console-specific?

## Next Refinement Steps

1. Define the concrete shared runtime and generator project boundaries
2. Specify the command and option attribute surface including resource references and inference rules
3. Define the generated API surface used by `Program.cs`
4. Split implementation work into infrastructure, migration, and test tasks

## Planned Implementation Tasks

- `T-RepoMigrator-046` - `Specify shared attribute-driven console command model and generated API`
- `T-RepoMigrator-047` - `Implement shared console command runtime and source generator`
- `T-RepoMigrator-048` - `Adopt generated command parsing in initial RepoMigrator console tools`
- `T-RepoMigrator-049` - `Add coverage for generated console command parsing and help output`
