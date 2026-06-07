# Task T-RepoMigrator-048 - Adopt generated command parsing in initial RepoMigrator console tools

## Status

Draft

## Parent

- Backlog Item `BI-RepoMigrator-013` - `Create reusable CommandlineHelper infrastructure`
- Task `T-RepoMigrator-047` - `Implement reusable command-line runtime and source generator`

## Goal

Adopt the shared generated command parsing helper in the existing RepoMigrator console tools while retaining `Program.cs` as the orchestration root in the first slice.

## Scope

- Migrate `RepoMigrator.Tools.GitBranchSplitter` to the generated options and help model
- Migrate `RepoMigrator.Tools.PipelinedMigration` to the generated options and help model
- Migrate `RepoMigrator.Tools.ArchiveSmokeTest` to the generated options and help model
- Add required resource files and localized metadata keys for user-facing command text
- Remove superseded hand-written parsing and usage boilerplate from the adopter tools

## Acceptance Criteria

- All three current RepoMigrator console tools use the shared generated command parsing helper as initial adopters
- Existing entry-point orchestration and execution behavior remain intact in `Program.cs`
- User-facing help text is resolved via resources
- The migrated tools build successfully within the workspace

## Dependencies

- `T-RepoMigrator-047` - `Implement reusable command-line runtime and source generator`

## Adoption Design

### 1. Adoption goal

This task adopts the reusable `CommandlineHelper` in the first three consumer applications:

- `RepoMigrator.Tools.GitBranchSplitter`
- `RepoMigrator.Tools.PipelinedMigration`
- `RepoMigrator.Tools.ArchiveSmokeTest`

The purpose of this task is not to redesign the runtime behavior of these tools. The goal is to replace repeated command-line parsing and help boilerplate with generated parsing and help infrastructure while preserving the existing execution flow in `Program.cs`.

### 2. First-slice migration boundary

This adoption task should only move command-line declaration and command-line handling concerns into the new helper model.

Must move:

- command metadata
- option metadata
- positional argument metadata where applicable
- usage text generation
- help text generation
- raw argument parsing
- required-value validation currently implemented manually

Must remain in the consumer application:

- dependency injection wiring
- service instantiation and service resolution
- runtime orchestration
- exception handling
- exit-code mapping
- business validation that goes beyond generic command-line binding

### 3. Consumer project changes

Each adopter project should:

1. reference `CommandlineHelper`
2. reference `CommandlineHelper.Generators` as analyzer
3. update the options class to the new attribute-based declaration model
4. add resource files for user-facing command and option text
5. update `Program.cs` to consume the generated parser companion type
6. remove superseded manual parse and usage code once the generated path is in place

The migration should be performed tool by tool so regressions stay easy to diagnose.

### 4. Tool-specific adoption notes

#### 4.1 GitBranchSplitter

Current behavior to preserve:

- `--help` and `-h` return success with help output
- invalid arguments return exit code `1`
- runtime failures return exit code `2`
- existing service resolution and `SplitAsync` invocation remain unchanged

Expected migration shape:

- keep `GitBranchSplitOptions` as the options type
- decorate the type with `CommandDescriptorAttribute`
- decorate the bound properties with `CommandOptionAttribute` and `CommandFlagAttribute`
- move user-facing descriptions and help text to a resource file
- replace `GitBranchSplitOptions.Parse(args)` and local `WriteUsage()` with generated helper usage

#### 4.2 PipelinedMigration

Current behavior to preserve:

- `--help` and `-h` return success with help output
- invalid arguments return exit code `1`
- runtime failures return exit code `2`
- provider registration and migration execution flow remain unchanged

Expected migration shape:

- keep `PipelinedMigrationOptions` as the options type
- represent all current named arguments with command option and flag attributes
- keep semantic validation that belongs to the options object or runtime logic, while moving generic command-line parsing into generated code
- move help and description text to resources
- replace the current handwritten dictionary-driven parsing entry point with generated parsing

#### 4.3 ArchiveSmokeTest

Current behavior to preserve:

- help request returns success with help output
- invalid arguments return exit code `1`
- runtime failures return exit code `2`
- service registration and archive smoke-test execution remain unchanged

Expected migration shape:

- keep `ArchiveSmokeTestOptions` as the options type
- map `--source`, `--workspace`, `--recursive`, and `--extension` to the first-slice helper model where possible
- if repeated `--extension` values are still outside the first implementation slice, either defer full adoption for that single feature or temporarily narrow the first migration scope and record the gap explicitly
- move help and description text to resources
- replace the current manual parsing and local usage writer with generated parsing/help

### 5. Handling first-slice feature gaps

This task depends on the actual first-slice capabilities implemented in `T-RepoMigrator-047`. If an adopter currently uses a command-line feature not yet supported by the helper, the task should handle it explicitly instead of silently widening the helper scope.

Preferred order of response to a feature gap:

1. determine whether the existing consumer behavior can be expressed within the approved first slice without semantic loss
2. if not, decide whether the consumer can temporarily keep a small local compatibility shim around the unsupported edge case
3. if that still produces an awkward contract, record the gap and split follow-up helper work rather than overloading this adoption task

The currently visible risk is repeated option values such as multiple `--extension` occurrences in `ArchiveSmokeTest`.

### 6. Resource migration rules

All user-facing command text added or moved in this task must be resource-based.

Per adopter, add or update resource entries for:

- command description
- optional long help text
- option descriptions
- positional argument description if a first positional token is introduced later

Consumer code should use `nameof(...)` for resource member references where applicable.

### 7. Program.cs consumption pattern

Each migrated consumer `Program.cs` should adopt the same control-flow pattern:

1. call generated `Parse(args)`
2. if help requested, call generated `WriteHelp(...)` and return `0`
3. if parse failed, write the parse error, call generated `WriteUsage(...)`, and return `1`
4. otherwise continue with existing orchestration using the parsed options object
5. preserve current runtime exception handling and runtime failure exit code behavior

This keeps consumer entry points consistent across the three adopters.

### 8. Recommended implementation order

1. Migrate `GitBranchSplitter` because its current command surface is the smallest
2. Migrate `PipelinedMigration` next because it has a larger but still regular named-option surface
3. Migrate `ArchiveSmokeTest` last because repeated `--extension` values may expose first-slice helper limitations
4. Review all three tools for consistent exit-code and help-output behavior

### 9. Definition of done for adoption

This task should be considered complete when:

- each of the three adopter tools references the reusable helper infrastructure
- command declarations live on dedicated options classes through attributes
- user-facing command/help text is resource-based
- `Program.cs` uses generated parse and help APIs instead of handwritten parsing and usage code
- redundant manual parser code has been removed or reduced to a clearly documented temporary compatibility shim
- the tools are ready for coverage expansion in `T-RepoMigrator-049`

## Deliverables

- Updated `GitBranchSplitOptions` and `Program.cs` consumer integration
- Updated `PipelinedMigrationOptions` and `Program.cs` consumer integration
- Updated `ArchiveSmokeTestOptions` and `Program.cs` consumer integration
- Resource files or updated resource entries for the migrated command metadata
- Removal or reduction of superseded manual parser/help code in the adopter tools

## Validation Evidence

- The adoption task now defines clear migration boundaries between generic command-line handling and application-specific orchestration
- The three initial consumer tools are each covered with tool-specific adoption notes and preserved runtime behavior expectations
- First-slice capability gaps such as repeated option values are identified explicitly so they can be handled deliberately during migration
