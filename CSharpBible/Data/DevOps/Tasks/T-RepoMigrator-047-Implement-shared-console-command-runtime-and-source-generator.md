# Task T-RepoMigrator-047 - Implement reusable command-line runtime and source generator

## Status

Draft

## Parent

- Backlog Item `BI-RepoMigrator-013` - `Create reusable CommandlineHelper infrastructure`
- Task `T-RepoMigrator-046` - `Specify reusable attribute-driven command-line model and generated API`

## Goal

Implement the shared runtime types and source generator that transform attribute-decorated options classes into generated parsing, validation, and usage/help infrastructure for a reusable command-line helper.

## Scope

- Create the shared runtime project for command and option metadata attributes plus parse-result support types
- Create the source-generator project that discovers eligible options classes and emits parsing/help code
- Implement metadata validation and developer-facing diagnostics for unsupported or invalid declarations
- Implement resource-based lookup hooks for command and option user texts
- Keep the first slice limited to the scenarios needed by the initial adopter tools while avoiding RepoMigrator-specific helper contracts

## Acceptance Criteria

- The workspace contains a reusable runtime project and a reusable source-generator project for command-line handling
- Supported attribute-decorated options classes receive generated parsing and help APIs during build
- Invalid metadata produces clear diagnostics
- User-facing command descriptions and help text are sourced indirectly through resources

## Dependencies

- `T-RepoMigrator-046` - `Specify reusable attribute-driven command-line model and generated API`

## Implementation Design

### 1. First-slice implementation goal

This task implements the reusable `CommandlineHelper` described in `T-RepoMigrator-046`. The implementation must stay aligned with the first-slice boundaries defined there:

- dedicated options classes as the binding model
- command metadata through attributes
- named options, repeatable named options for supported collection-typed properties, boolean flags, and a first positional argument at position `0`
- inferred `Required` and `DefaultValue` semantics with explicit attribute override precedence
- resource-based user-facing text
- generated parse and help APIs
- no RepoMigrator-specific contracts in the helper projects

The task does not yet include migration of existing consumer applications. That belongs to `T-RepoMigrator-048`.

### 2. Project layout

The implementation should introduce two new projects under a neutral location, for example a top-level `Libraries` or `Tools` area that is not product-specific.

Recommended projects:

1. `CommandlineHelper\CommandlineHelper.csproj`
2. `CommandlineHelper\CommandlineHelper.Generators.csproj`

Recommended target frameworks:

- `CommandlineHelper`: target a broadly reusable framework such as `netstandard2.0` if the required language and runtime APIs allow it, otherwise the lowest practical modern target used across the workspace
- `CommandlineHelper.Generators`: target the normal Roslyn source-generator-compatible framework used for analyzers and generators in the current SDK toolchain

The runtime library should expose only the public contracts needed by consuming applications and generated code. The generator project should reference Roslyn packages and the runtime project, and should be referenced by consumers as an analyzer.

### 3. Runtime project contents

The runtime project should contain the stable public API and minimal shared implementation support.

Recommended files:

- `CommandDescriptorAttribute.cs`
- `CommandOptionAttribute.cs`
- `CommandFlagAttribute.cs`
- `CommandArgumentAttribute.cs`
- `CommandParseResult.cs`
- `CommandTextResourceResolver.cs` or similar helper for generated resource lookup support
- optional small internal helper files for generated code support

Responsibilities:

- define the attribute contracts exactly as specified in `T-RepoMigrator-046`
- define the generic parse-result model
- provide any small helper methods that generated code can call without duplicating fragile logic everywhere
- avoid runtime reflection-driven parsing infrastructure; ordinary parsing logic belongs in generated code

The runtime project should keep the public surface small, explicit, and stable.

### 4. Generator project contents

The generator project should implement source discovery, semantic validation, and code emission.

Recommended files:

- `CommandlineHelperGenerator.cs`
- `CommandModelBuilder.cs`
- `CommandModel.cs`
- `CommandPropertyModel.cs`
- `CommandEmitter.cs`
- `HelpTextEmitter.cs`
- `DiagnosticDescriptors.cs`
- `ResourceLookupInspector.cs`
- `TypeSupportRules.cs`

Responsibilities:

- discover candidate options classes with `CommandDescriptorAttribute`
- inspect bound properties for supported attributes
- build an internal semantic command model
- validate supported property shapes, duplicate names, duplicate positions, invalid defaults, and unsupported combinations
- detect supported collection-typed option properties and model them as repeatable single-value option occurrences
- emit one generated companion type per options class
- emit deterministic diagnostics with stable IDs

The generator should use incremental generation so repeated builds only reprocess changed inputs.

### 5. Internal generator model

The generator should normalize declarations into an internal model before emitting source.

Recommended command model fields:

- command CLR namespace and type name
- generated companion type name
- command display name
- resource type symbol
- description resource key
- help resource key
- collection of bound property models

Recommended property model fields:

- CLR property symbol and property type
- binding kind: option, flag, argument
- collection behavior: scalar or repeatable
- long name or position
- optional short name
- resolved effective requiredness metadata
- resolved effective default metadata
- effective resource lookup source
- conversion strategy

This separation keeps semantic validation and code generation easier to test and evolve.

### 6. Generated code responsibilities

For each command options class, the generator should emit a companion type that:

- recognizes `--help` and `-h`
- recognizes the supported named options and short aliases
- recognizes repeatable named options for supported collection-typed properties
- binds the first positional argument when declared
- converts raw string tokens to the target property types
- applies inferred or explicit defaults
- validates missing required values
- accumulates one value per occurrence for collection-typed options
- returns a populated `CommandParseResult<TOptions>`
- writes usage output
- writes help output including description, options, default values, and extended help text where present

The generator should avoid emitting unnecessary reflection, dynamic dispatch, or per-invocation allocation-heavy helper structures when simpler code generation is possible.

### 7. Resource lookup implementation

User-facing text must remain resource-based.

Implementation guidance:

- generated code should prefer strongly-typed static resource properties when they can be resolved semantically
- if a declared resource member cannot be found or is not usable as a string-bearing resource accessor, the generator should emit a build diagnostic
- the runtime helper may contain a tiny shared abstraction for resource retrieval if that reduces repeated generated code, but the contract should remain simple

The first slice only needs resource-based descriptions and long help text. It does not need advanced culture-switching orchestration beyond normal .NET resource behavior.

### 8. Diagnostics plan

The generator should define stable diagnostic IDs for at least these cases:

- missing or invalid command declaration
- unsupported property type
- invalid use of `CommandFlagAttribute`
- invalid positional argument position
- duplicate option name
- duplicate short alias
- duplicate positional argument position
- unsupported collection target type
- unresolved resource member
- invalid explicit default value
- unsupported initializer for inferred default evaluation when it is declared as relied upon

Each diagnostic should include:

- a stable ID
- severity
- short title
- message format with the affected member name
- category such as `CommandlineHelper`

### 9. Build integration

Consumer projects should integrate the generator through normal analyzer references.

The intended shape for a consumer project is:

- reference `CommandlineHelper`
- reference `CommandlineHelper.Generators` as analyzer

The generator output must work in ordinary SDK-style builds inside Visual Studio and command-line builds in the workspace.

### 10. Recommended implementation order

1. Create the runtime project and add the public attribute and parse-result contracts
2. Create the generator project with incremental generator wiring and diagnostic descriptors
3. Implement command discovery and internal semantic model building
4. Implement declaration validation and diagnostics
5. Implement code emission for parsing and result construction, including repeatable collection-typed options
6. Implement usage and help emission
7. Implement resource resolution checks
8. Validate generator output with focused test assets or consumer fixtures

### 11. Definition of done for implementation

This task should be considered complete when:

- both helper projects exist in the workspace
- the public runtime API matches the approved specification closely enough for consumer adoption
- generator output is produced for valid command declarations
- invalid declarations produce actionable diagnostics
- resource-based metadata is supported for user-facing text
- supported collection-typed options are emitted as repeatable one-value-per-occurrence bindings
- the implementation is ready for consumer migration in `T-RepoMigrator-048`

## Deliverables

- New reusable runtime project for `CommandlineHelper`
- New reusable source-generator project for `CommandlineHelper.Generators`
- Generator diagnostics catalog with stable initial diagnostic IDs
- Working generated parser/help output for the supported first-slice declaration model
- Repeatable option handling for supported collection-typed properties

## Validation Evidence

- Implementation details are now broken down into runtime, generator, semantic-model, diagnostics, resource, and build-integration work packages
- The task now defines concrete project boundaries, suggested file layout, and recommended implementation order
- The implementation scope is explicitly aligned with the approved first-slice specification from `T-RepoMigrator-046`
