# ConsoleLib V2 Release Readiness

## Scope and support matrix

| Surface | Project | Validation |
| --- | --- | --- |
| Host-neutral controls and CXAML runtime | `ConsoleLib` | `ConsoleLib.CoreTests` |
| Windows/ExtendedConsole backend | `ConsoleLib.ExtCon` | `ConsoleLib.ExtConTests` |
| ANSI/VT POSIX backend | `ConsoleLib.Posix` | `ConsoleLib.PosixTests` |
| Avalonia CXAML designer | `ConsoleLib.Cxaml.Designer` | `ConsoleLib.Cxaml.DesignerTests` |
| Reference applications | four `*.Cxaml` projects | `ConsoleLib.Cxaml.ExamplesTests` |

The modern host projects target `net8.0`; the core target matrix remains
`net8.0`, `net9.0`, and `net10.0` where the existing project configuration
supports it. Legacy imperative applications remain available and are not
replaced by the examples.

## CXAML v1 contract

CXAML uses the ConsoleLib control names and a single root element. Supported
attributes currently include `Text`, `Width`, `Height`, `X`, `Y`, `Visible`,
`Enabled`, `BackColor`, `ForeColor`, and `IsChecked` for check boxes. Invalid
XML, empty documents, multiple roots, unknown elements, and invalid scalar
values are reported as `CxamlParseException` or generator diagnostics.

The runtime loader and generator are deliberately deterministic. The designer
loads the same markup through the runtime loader, so preview and application
materialization do not use separate interpretation rules.

## Migration checklist

1. Keep the existing imperative startup until the CXAML view has a passing
   loader test.
2. Add a project reference to `ConsoleLib` and embed the `.cxaml` view.
3. Load the root through `CxamlLoader`; do not instantiate controls from
   arbitrary reflection or execute shell commands in a designer preview.
4. Select `ConsoleLib.ExtCon` for the Windows backend or `ConsoleLib.Posix` for
   ANSI/VT terminals.
5. Add a project-specific test assembly and cover loading, focus, commands,
   rendering, and cancellation before switching the application entry point.

## Terminal and SSH/tmux/screen guidance

- Run with a real TTY for raw keyboard and SGR mouse behavior.
- Redirected input/output uses the non-interactive fallback and must not enable
  raw mode.
- SGR mouse is optional; keyboard navigation remains the fallback when the
  terminal or multiplexer does not advertise mouse support.
- Resize and cancellation must be exercised through the host abstraction,
  followed by a terminal-restore assertion.
- When diagnosing a black screen, first disable mouse and raw mode, then use
  the in-memory/frame tests to isolate input dispatch from rendering.

## Clipboard security model

OSC 52 is opt-in and bounded by a payload limit. A host may reject clipboard
read/write requests; rejection is reported explicitly and is never returned as
success. Applications should keep clipboard directions disabled unless their
terminal trust boundary is understood. Redirected and remote sessions should
prefer the host fallback.

## Widget UX acceptance

Keyboard focus must remain visible without relying on color alone. Tab and
Shift+Tab traverse eligible controls; hidden and disabled controls are skipped.
Tree and tile controls provide keyboard operation when pointer capabilities are
missing. Text editing preserves selection and cursor boundaries. Form
controls expose disabled, busy, and validation states through the shared
rendering contract.

## Test and coverage procedure

Run each test project separately to avoid shared output-path contention:

```text
dotnet test Libraries\ConsoleLib.CoreTests\ConsoleLib.CoreTests.csproj --collect:"XPlat Code Coverage"
dotnet test Libraries\ConsoleLib.ExtConTests\ConsoleLib.ExtConTests.csproj --collect:"XPlat Code Coverage"
dotnet test Libraries\ConsoleLib.PosixTests\ConsoleLib.PosixTests.csproj --collect:"XPlat Code Coverage"
dotnet test Libraries\ConsoleLib.Cxaml.DesignerTests\ConsoleLib.Cxaml.DesignerTests.csproj --collect:"XPlat Code Coverage"
dotnet test Libraries\ConsoleLib.Cxaml.ExamplesTests\ConsoleLib.Cxaml.ExamplesTests.csproj --collect:"XPlat Code Coverage"
```

Coverage reports are generated under each test project's `TestResults`
directory. Use the reports to target untested production lines; test count
alone is not a release criterion.

## Latest validation result

The complete split suite passes sequentially: **158 tests, 0 failures**.
Coverage collection produced Cobertura reports for all five test projects.
The reports intentionally remain project-specific because referenced projects
share output folders and because backend coverage must be improved against the
production assemblies it exercises, not hidden behind an aggregate test count.

The backend hardening follow-up adds six POSIX transport/output/encoder
contract tests. The POSIX project now passes **24 tests**, and its measured
line-rate increased to **17.2% (780/4533 lines)**.
