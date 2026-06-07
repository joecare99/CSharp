# Task T-RepoMigrator-046 - Specify reusable attribute-driven command-line model and generated API

## Status

Completed

## Parent

- Backlog Item `BI-RepoMigrator-013` - `Create reusable CommandlineHelper infrastructure`

## Goal

Specify the concrete attribute model, inference rules, generated API shape, and localization contract for the first slice of reusable command-line helper infrastructure.

## Scope

- Define command-level metadata attributes for command name, resource type, description, and extended help
- Define property-level metadata attributes for option names, aliases, descriptions, and optional inference overrides
- Define how requiredness is inferred from `required`, nullability, and property initialization
- Define how default values are inferred and where explicit metadata overrides take precedence
- Define the generated parse-result and usage/help API used by `Program.cs`
- Define how resource lookup failures and invalid metadata should be reported
- Define how initial adopter applications consume the generated API without coupling the helper to RepoMigrator-specific concepts

## Acceptance Criteria

- The attribute surface is documented with clear responsibilities and precedence rules
- The generated API surface for parsing and help writing is documented concretely enough for implementation
- Localization rules require user-facing text to come from resources
- Open points for later slices are separated from the first implementation slice

## Deliverables

- Written technical specification in the task file or linked refinement notes
- Example target coding for at least one adopter console tool
- Explicit precedence rules for inferred versus declared metadata

## Dependencies

- `BI-RepoMigrator-013` - `Create reusable CommandlineHelper infrastructure`

## Technical Specification

### 1. First-slice target

The first slice defines a reusable `CommandlineHelper` that supports attribute-driven command-line declaration on dedicated options classes. The helper is intentionally limited to the scenarios needed by the current initial adopters, while keeping the contracts product-agnostic and suitable for broader reuse.

The first slice must generate only the repetitive command-line infrastructure:

- raw argument parsing
- required-value validation
- default-value application
- usage and help output
- developer-facing diagnostics for invalid declarations

Application-specific startup logic remains outside the helper. Existing `Program.cs` files stay responsible for dependency injection, service resolution, runtime orchestration, exception handling, and exit-code mapping.

### 2. Project shape

The helper should be split into two projects:

1. `CommandlineHelper`
   - runtime types only
   - metadata attributes
   - parse-result support types
   - resource access abstractions or helpers used by generated code
2. `CommandlineHelper.Generators`
   - Roslyn source generator
   - metadata validation
   - generated parser and help writer emission
   - diagnostics for unsupported declarations

The runtime project should not depend on RepoMigrator-specific types. The generator should emit code into the consuming assembly and should not require runtime reflection for ordinary parsing behavior.

### 3. Declaration model

#### 3.1 Command declaration target

Commands are declared on dedicated options classes, not on mutable static fields in `Program`.

Requirements:

- one options type represents one command
- the options type should be a non-abstract class
- the options type may be `partial` so the generator can attach companion generated members or generated companion types cleanly
- properties are the only first-slice binding targets
- fields, constructor parameters, and methods are out of scope for the first slice

#### 3.2 Command-level metadata

The options class is decorated with a command descriptor attribute.

Proposed shape:

```csharp
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class CommandDescriptorAttribute : Attribute
{
	public CommandDescriptorAttribute(string name);

	public string Name { get; }
	public Type? ResourceType { get; init; }
	public string? DescriptionResourceName { get; init; }
	public string? HelpTextResourceName { get; init; }
}
```

Semantics:

- `Name` is the canonical command or application name shown in usage output
- `ResourceType` identifies the resource container used for all command-level and option-level user text that omits an explicit resource type override
- `DescriptionResourceName` resolves a short summary shown near the usage header
- `HelpTextResourceName` resolves a longer help block or epilog shown after the options list

The helper must treat command-level user-facing text as localized content and therefore require resource-based lookup for visible descriptions and long help text in normal usage.

#### 3.3 Property-level metadata

Each bound property is decorated with one of the supported option attributes.

First-slice supported forms:

- first positional argument
- named scalar option
- repeatable named option for collection-typed properties
- boolean flag option

Out of scope for the first slice:

- multiple values consumed from a single option occurrence
- subcommands
- mutually exclusive groups
- custom converters beyond a narrow built-in set

The first positional argument is intended to cover a highlighted command selector such as `run`, `build`, `test`, or `help` without requiring a `--` prefix. In the first slice, only position `0` is supported.

Proposed positional argument shape:

```csharp
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class CommandArgumentAttribute : Attribute
{
	public CommandArgumentAttribute(int position);

	public int Position { get; }
	public Type? ResourceType { get; init; }
	public string? DescriptionResourceName { get; init; }
	public bool? Required { get; init; }
	public object? DefaultValue { get; init; }
}
```

Semantics:

- `Position` identifies the positional token index
- the first slice only permits `Position = 0`
- the positional argument participates in the same requiredness and default-value model as named options
- the intended primary use is an enum-backed verb-like selector, but the contract is modeled as a general positional argument to keep the design uniform

Proposed scalar option shape:

```csharp
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class CommandOptionAttribute : Attribute
{
	public CommandOptionAttribute(string longName);

	public string LongName { get; }
	public string? ShortName { get; init; }
	public Type? ResourceType { get; init; }
	public string? DescriptionResourceName { get; init; }
	public bool? Required { get; init; }
	public object? DefaultValue { get; init; }
}
```

Proposed flag shape:

```csharp
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class CommandFlagAttribute : Attribute
{
	public CommandFlagAttribute(string longName);

	public string LongName { get; }
	public string? ShortName { get; init; }
	public Type? ResourceType { get; init; }
	public string? DescriptionResourceName { get; init; }
	public bool? DefaultValue { get; init; }
}
```

Semantics:

- `LongName` is the canonical option token such as `--source`
- `ShortName` is an optional short alias such as `-s`
- `ResourceType` optionally overrides the command-level `ResourceType`
- `DescriptionResourceName` resolves the user-visible option description
- `Required` is optional and overrides inferred requiredness when specified
- `DefaultValue` is optional and overrides inferred default behavior when specified
- when the bound property type is a supported collection such as `T[]`, `IList<T>`, `IReadOnlyList<T>`, or `List<T>`, the option becomes repeatable and each occurrence contributes exactly one value
- the first slice does not support one option occurrence consuming multiple consecutive values

### 4. Supported property types

The first slice should support these property types:

- `string`
- `string?`
- `bool`
- `bool?`
- `int`
- `int?`
- `long`
- `long?`
- `enum`
- nullable enum

For named repeatable options, the first slice should support collection-typed properties whose element type is one of the supported scalar types above, especially `string`, `int`, `long`, and enum types.

For the first positional argument, `enum` and nullable enum should be treated as the preferred target types because they provide a natural and diagnosable model for verb-like values such as `run`, `build`, `test`, and `help`.

Support for additional primitives can be added later, but the first slice should stay deliberately narrow and only include types needed by the first adopters plus a small generally useful baseline.

### 5. Inference rules

#### 5.1 General precedence

Precedence order must be:

1. explicit attribute metadata
2. inferable property semantics
3. type default where neither of the above applies

In other words, declared metadata always wins over inferred behavior.

#### 5.2 Requiredness inference

Requiredness should be inferred by default from property shape.

Recommended first-slice rules:

- `required` property without a usable default implies required CLI input
- non-nullable reference type without initializer implies required CLI input
- nullable reference type implies optional CLI input
- nullable value type implies optional CLI input
- property with a supported default initializer implies optional CLI input
- `bool` flag properties are optional by default
- a positional argument follows the same inference rules as a named option or flag based on its property shape
- a collection-typed option is optional by default unless explicit metadata requires at least one occurrence

The `Required` property on `CommandOptionAttribute` and `CommandArgumentAttribute` overrides these rules when specified.

For collection-typed options, `Required = true` means at least one value must be supplied.

The specification intentionally treats requiredness as CLI semantics rather than pure CLR nullability semantics. A property may therefore remain nullable or otherwise technically optional in code while still being required at the command-line layer through explicit metadata.

#### 5.3 Default-value inference

Default values should be inferred when the property has a supported constant or literal initializer.

Supported first-slice inferred defaults:

- string literal
- numeric literal
- boolean literal
- enum literal
- `null` on nullable properties where applicable

Not required in the first slice:

- computed initializers
- `static readonly` indirection
- method calls
- environment-dependent expressions
- collection initializers used as inferred defaults

If `DefaultValue` is provided on the attribute, it overrides any inferred default.

This same rule applies to `CommandArgumentAttribute`, so a first positional verb-like selector can be optional with an inferred or explicit default in exactly the same way as a named option.

For collection-typed options, the first slice should prefer an empty collection as the implicit default. Explicit collection defaults through attribute metadata are out of scope for the first slice.

If a property has neither an explicit override nor an inferable initializer, normal type semantics apply.

### 6. Generated API

The generator should produce a deterministic and easy-to-consume API surface per options type.

Recommended first-slice generated companion type naming:

- for `GitBranchSplitOptions`, generate `GitBranchSplitOptionsCommand`

Recommended generated members:

```csharp
public static class GitBranchSplitOptionsCommand
{
	public static CommandParseResult<GitBranchSplitOptions> Parse(string[] args);
	public static void WriteUsage(TextWriter writer);
	public static void WriteHelp(TextWriter writer);
}
```

Recommended runtime parse-result shape:

```csharp
public sealed class CommandParseResult<TOptions>
{
	public bool Success { get; }
	public bool RequestHelp { get; }
	public TOptions? Options { get; }
	public string? ErrorMessage { get; }
}
```

Semantics:

- `Parse` performs token parsing, conversion, default application, and required-value validation
- `RequestHelp` is true for `--help` and `-h`
- `Success` is true only when parsing and validation complete successfully
- `Options` is populated only on success
- `ErrorMessage` is intended for direct display to `stderr`
- `WriteUsage` emits the concise usage form
- `WriteHelp` emits usage plus description, options, defaults, and extended help text

The generator may internally share a common support layer, but the public consuming shape should remain simple and stable.

### 7. Expected Program.cs usage

The first-slice usage pattern in consumer applications should look like this:

```csharp
internal static class Program
{
	public static async Task<int> Main(string[] args)
	{
		var parseResult = GitBranchSplitOptionsCommand.Parse(args);
		if (parseResult.RequestHelp)
		{
			GitBranchSplitOptionsCommand.WriteHelp(Console.Out);
			return 0;
		}

		if (!parseResult.Success)
		{
			Console.Error.WriteLine(parseResult.ErrorMessage);
			GitBranchSplitOptionsCommand.WriteUsage(Console.Out);
			return 1;
		}

		var options = parseResult.Options!;
		// application-specific orchestration remains here
		return 0;
	}
}
```

This keeps the generator focused on command-line concerns and avoids hiding application control flow.

### 8. Localization contract

Localization is mandatory for user-facing command metadata.

Rules:

- visible descriptions and extended help text must be resolved indirectly from resources
- resource lookup should use `ResourceManager`-compatible generated resource classes or equivalent strongly-typed resource access patterns
- generated code must not require hard-coded localized display text literals for ordinary command metadata
- diagnostic text for developers may remain non-localized in the first slice

Command-level lookup precedence:

1. attribute `ResourceType` on the command
2. no fallback to literal description text in the first slice

Option-level lookup precedence:

1. option attribute `ResourceType`
2. command attribute `ResourceType`

If a description or help resource name is specified but cannot be resolved, the generator should report a build diagnostic rather than silently emit broken help output.

### 9. Diagnostics

The generator must emit clear developer-facing diagnostics for unsupported or invalid declarations.

Examples of diagnostic cases:

- missing `CommandDescriptorAttribute` on an options class selected for generation
- duplicate option names or aliases within one command
- duplicate positional argument positions within one command
- unsupported property type
- `CommandFlagAttribute` applied to a non-boolean property
- unsupported positional argument position
- unsupported collection target type
- unresolved resource property
- invalid explicit default value for the target property type
- impossible combinations such as required flag semantics on a simple boolean switch

Diagnostics should identify both the affected type or property and the violated rule.

### 10. Help output contract

`WriteUsage` should produce a compact summary.

`WriteHelp` should produce, in order:

1. usage line
2. optional short description
3. option listing with names, requiredness, and default information where appropriate
4. optional extended help text

The output format should be stable and readable, but exact alignment rules do not need to be fully optimized in the first slice.

### 11. Example target declaration

```csharp
[CommandDescriptor(
	"git-branch-splitter",
	ResourceType = typeof(CommandResources),
	DescriptionResourceName = nameof(CommandResources.GitBranchSplitter_Description),
	HelpTextResourceName = nameof(CommandResources.GitBranchSplitter_Help))]
public sealed partial class GitBranchSplitOptions
{
	[CommandOption(
		"--repo",
		DescriptionResourceName = nameof(CommandResources.GitBranchSplitter_Repo_Description))]
	public required string RepositoryPath { get; init; }

	[CommandOption(
		"--source",
		DescriptionResourceName = nameof(CommandResources.GitBranchSplitter_Source_Description))]
	public required string SourceBranch { get; init; }

	[CommandOption(
		"--prefix",
		DescriptionResourceName = nameof(CommandResources.GitBranchSplitter_Prefix_Description))]
	public string BranchPrefix { get; init; } = "split";

	[CommandFlag(
		"--overwrite",
		DescriptionResourceName = nameof(CommandResources.GitBranchSplitter_Overwrite_Description))]
	public bool OverwriteExistingBranches { get; init; }
}
```

This example expresses the intended first-slice target clearly:

- options class instead of static mutable program state
- descriptions from resources
- requiredness inferred from `required`
- default inferred from initializer
- minimal attribute noise unless behavior must be overridden

### 11.1 Example with highlighted first positional command selector

```csharp
public enum BuildToolVerb
{
	Run,
	Build,
	Test,
	Help
}

[CommandDescriptor(
	"build-tool",
	ResourceType = typeof(CommandResources),
	DescriptionResourceName = nameof(CommandResources.BuildTool_Description),
	HelpTextResourceName = nameof(CommandResources.BuildTool_Help))]
public sealed partial class BuildToolOptions
{
	[CommandArgument(
		0,
		DescriptionResourceName = nameof(CommandResources.BuildTool_Verb_Description))]
	public BuildToolVerb Verb { get; init; } = BuildToolVerb.Run;

	[CommandOption(
		"--configuration",
		DescriptionResourceName = nameof(CommandResources.BuildTool_Configuration_Description))]
	public string Configuration { get; init; } = "Debug";
}
```

This example shows the intended uniformity:

- the highlighted first token is modeled as a positional argument rather than a special-case parser rule
- `Required` and `DefaultValue` semantics match the general property-based inference model
- an enum-backed selector allows the generator to validate and describe allowed values clearly

### 11.2 Example with repeatable named option

```csharp
[CommandDescriptor(
	"archive-smoke-test",
	ResourceType = typeof(CommandResources),
	DescriptionResourceName = nameof(CommandResources.ArchiveSmokeTest_Description),
	HelpTextResourceName = nameof(CommandResources.ArchiveSmokeTest_Help))]
public sealed partial class ArchiveSmokeTestOptions
{
	[CommandOption(
		"--source",
		DescriptionResourceName = nameof(CommandResources.ArchiveSmokeTest_Source_Description))]
	public required string SourceDirectoryPath { get; init; }

	[CommandOption(
		"--extension",
		DescriptionResourceName = nameof(CommandResources.ArchiveSmokeTest_Extension_Description))]
	public IReadOnlyList<string> AllowedExtensions { get; init; } = Array.Empty<string>();
}
```

The intended command line is:

- `--extension .zip --extension .tar.gz`

and not:

- `--extension .zip .tar.gz`

### 12. Deferred work

The following are intentionally deferred beyond the first slice:

- additional positional arguments beyond position `0`
- list and repeated-value binding
- nested commands and subcommands
- custom converters and custom validators
- environment-variable and configuration-file merging
- localized developer diagnostics
- advanced usage formatting and examples modeling

## Decision Summary

- dedicated options classes are the binding model
- user-facing texts are resource-based
- `Required` is inferred by default and may be overridden explicitly
- `DefaultValue` is inferred by default where supported and may be overridden explicitly
- a highlighted first token such as `run|build|test|help` is modeled as positional argument `0` and uses the same required/default inference and override rules as named options
- collection-typed option properties are supported as repeatable named options, with one value bound per occurrence
- command-level description and extended help are defined on the options class
- generated code covers parsing and help only; `Program.cs` remains orchestration

## Validation Evidence

- Documented the first-slice `CommandlineHelper` project split into runtime and generator projects
- Specified command, option, flag, and first-positional-argument attribute contracts
- Defined precedence and inference rules for `Required` and `DefaultValue`, including explicit override priority
- Defined the generated parse/help API and the expected `Program.cs` consumption pattern
- Captured the localization contract requiring resource-based user-facing text
- Added example target declarations for both named-option and highlighted first-positional-token scenarios
