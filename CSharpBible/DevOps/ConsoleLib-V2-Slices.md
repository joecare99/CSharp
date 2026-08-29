# ConsoleLib V2 Slice Status

## CL-08: CXAML reference applications

**Status:** Done

Four parallel, non-invasive reference projects were added:

- `Calc32Cons.Cxaml`
- `Leonardo.ST.Cxaml`
- `DetectiveGame.Console.Cxaml`
- `Ollama.CodingAgent.Console.Cxaml`

Each project embeds a CXAML view and exposes a small factory used by the
application entry point and integration tests. Existing imperative applications
were not replaced.

**Validation:** `ConsoleLib.Cxaml.ExamplesTests`: 4 passed.

## CL-15: Backend coverage hardening

**Status:** Done

Added POSIX contract coverage for ANSI color and cursor output, mouse tracking
and encoding branches, transport lifecycle, resize clamping, and cancellation.
The stale cross-project test includes were removed after the test split.

**Validation:** `ConsoleLib.PosixTests`: 24 passed; Cobertura line-rate
17.2% (780/4533).
