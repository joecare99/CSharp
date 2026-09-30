# IDR Avalonia

IDR Avalonia is the .NET/Avalonia rewrite of the original Delphi executable analyzer. It currently targets Windows and analyzes 32-bit Delphi PE images.

## Requirements

- .NET 10 SDK
- Windows desktop runtime for framework-dependent published builds

## Build and test

Run these commands from this directory:

```powershell
dotnet restore .\IDR.Avalonia.slnx
dotnet build .\IDR.Avalonia.slnx --configuration Release --no-restore
dotnet test .\IDR.Avalonia.slnx --configuration Release --no-build
```

The real-engine integration test uses external PE and Knowledge Base files when available. Set `IDR_INTEGRATION_PE`, `IDR_INTEGRATION_KB`, and optionally `IDR_INTEGRATION_DELPHI_VERSION` to their paths; the test is inconclusive when either file is unavailable, so binary samples do not need to be committed.

## Command-line batch analysis

Starting the app without arguments opens the Avalonia UI. Use the `analyze` command for batch/remote runs; it writes schema-versioned JSON to standard output, while `--output` writes the JSON to a file. Specify a Delphi version explicitly for statically linked images. A Knowledge Base can be selected by full file path, by directory, or through `IDR_KNOWLEDGE_BASE_DIRECTORY`.

```powershell
$env:IDR_SAMPLE_PE = '.\sample.exe'
$env:IDR_KNOWLEDGE_BASE_DIRECTORY = Join-Path $env:APPDATA 'IDR'
$app = '.\artifacts\win-x64\IDR.App.dll'
dotnet $app analyze `
  --input $env:IDR_SAMPLE_PE `
  --delphi-version 7 `
  --summary-only

dotnet $app analyze `
  --input $env:IDR_SAMPLE_PE `
  --version Delphi7 `
  --knowledge-base (Join-Path $env:IDR_KNOWLEDGE_BASE_DIRECTORY 'kb7.bin') `
  --output .\analysis.json `
  --progress
```

Use `dotnet .\artifacts\win-x64\IDR.App.dll analyze --help` for all options. Invoke the DLL through `dotnet` for a real console host and reliable stdout/exit-code propagation; the `.exe` apphost is built as a Windows GUI executable. `--summary-only` returns counts and diagnostics without the potentially large item/disassembly arrays. Progress is tab-separated on standard error, keeping standard output valid JSON. Exit codes are `0` when analysis completes (diagnostics are included in the JSON), `1` for syntax/required-option errors or input/analysis/output failures, `2` for unsupported Delphi versions or conflicting Knowledge Base options, and `130` for cancellation (Ctrl+C). The command host uses System.CommandLine 2.x and does not initialize the Avalonia UI.

Set `IDR_SAMPLE_PE` to a Delphi EXE or DLL to analyze, and place the matching `kb<Version>.bin` file in the Knowledge Base directory. The sample PE path can be relative to the current directory; the Knowledge Base example uses the current user's standard application-data directory.

## Publish

The managed application targets AnyCPU. Choose a Windows runtime identifier to produce the matching app host and native Avalonia dependencies:

```powershell
dotnet publish .\src\IDR.App\IDR.App.csproj --configuration Release --runtime win-x64 --self-contained false --output .\artifacts\win-x64
dotnet publish .\src\IDR.App\IDR.App.csproj --configuration Release --runtime win-x86 --self-contained false --output .\artifacts\win-x86
```

The published application requires the matching .NET 10 Desktop Runtime. Use `--self-contained true` when publishing a bundle that should include the runtime.

## Knowledge Base files

Place the existing `kb<Version>.bin` files beside the application, or set `IDR_KNOWLEDGE_BASE_DIRECTORY` to the directory containing them. The Knowledge Base files are not included in this repository.

The reader extracts the four supported Delphi runtime-handler signatures without requiring unrelated procedure dump records to be valid.

The application loads an image using **Open EXE/DLL**, detects or asks for the Delphi version, loads the matching Knowledge Base when available, then runs analysis asynchronously. The code scan processes up to 16384 code starts per image and 4096 instructions per code start, reporting a diagnostic if either safety limit is reached. The Cancel command requests cancellation. Analysis results, diagnostics, and disassembly are displayed in the main window.

The Knowledge Base reader extracts bounded code signatures for Delphi's `@HandleOnException`, `@HandleAnyException`, `@HandleAutoException`, and `@HandleFinally` runtime handlers. Relocation-masked signatures are matched in executable PE sections before code-flow analysis, allowing statically linked handlers to be named. Jumps to known handlers are classified as exception or finally handlers in the analysis flags. The analyzer recognizes both bounded Delphi x86 try-registration prologs and follows their handler blocks. It also recognizes the two original Delphi x86 finally-cleanup patterns (push continuation and direct jump) and displays their validated end addresses. `@HandleOnException` tables are bounds-checked; type-info and executable handler RVAs are displayed, and valid handler procedures are scanned. VMTs are searched in all file-backed sections, including executable sections where Delphi may place metadata. Malformed supported records and exception tables produce diagnostics. Full finally-cleanup decompilation and other Knowledge Base routine/type records remain future work.

The code-flow scan also recognizes the bounded six-instruction x86 pattern used for split 64-bit comparisons, marks the four comparison/control-flow instructions hidden by the original analyzer, and reports the highest validated branch target as the pattern end. Both stack-based variants validate the `[ESP+4]`/`[ESP]` operands, saved registers, and branch-to-pop target; the split-cleanup variant also validates the nested conditional/unconditional branch layout.

Indexed x86 indirect jumps with a file-backed absolute jump table are recognized as switches. The analyzer bounds table reads to the containing section and 4,096 entries, validates every case target against executable image sections, marks the dispatch/comparison/table entries, records case cross-references, and follows valid cases under the global code-start limit.

Backward conditional and direct unconditional branches into executable sections mark their target as a loop start, except when the branch instruction has already been classified as an exception or finally handler.

For procedure starts whose scan reaches a return, the engine records the byte size through the return instruction and the `ret N` stack adjustment (zero for a plain `ret`). Both values are available in the results grid and full CLI JSON; branch-only code paths do not claim procedure metadata.

The code scan also marks x86 frame setup/teardown, push/pop operations, and immediate `add`/`sub ESP` adjustments. Per-instruction stack-pointer deltas are exposed in results, while an `EBP`-based frame is recorded on the procedure start.

For procedures with an EBP frame, positive index-free `[EBP+offset]` memory references from `+8` through `+0x10000` are collected as stack arguments on the procedure start. Access sizes are rounded up to 4-byte stack slots (with a 4-byte minimum), and 10-byte accesses are labeled `Extended`. Delphi record and dynamic-array runtime helpers can add a type candidate to directly tracked arguments when valid RTTI is known.

Frame-based procedures receive a `StackArgumentSizeMismatch` flag when the discovered stack-argument byte sizes exceed the `ret N` cleanup amount. This is a conservative warning: arguments the procedure does not access cannot be inferred from code references alone. Standard `ENTER` prologues with a valid nesting level establish frame-pointer state and report their net stack allocation; `LEAVE` is marked as a frame instruction without inventing a fixed stack delta. A direct callee is marked as a function candidate if its EAX result is read or returned before overwrite, or if an immediately following x87 `FST`/`FSTP` consumes its floating-point return value.

For procedures that reach a return, reads of EAX/EDX/ECX before those registers are overwritten are reported as Delphi register-argument candidates. The conventional register order is preserved, and aliases such as AL/AX or DL/DX are grouped with their 32-bit register. This is a conservative heuristic, not a recovered function prototype; candidates appear in the results grid and full CLI JSON.

Calls to `@CallDynaInst`/`@CallDynaClass` resolve a dynamic-method call target when the class in EAX and the method ID are both known; the ID is read from BX through Delphi 5 and SI in newer versions. `@FindDynaInst`/`@FindDynaClass` use DX. Lookup follows parent VMTs, but an unknown Delphi version, a clobbered/partial ID register, or an invalid code target does not produce a method cross-reference. Indirect calls through a known class VMT slot are also resolved when the base-register class and slot offset are known; parent VMTs are searched as needed.

Memory accesses through a tracked class or record register are matched to parsed field ranges, including inherited class fields and nested record offsets. Referenced VMT field RTTI and standard record RTTI field tables are indexed and parsed with bounds checks; Delphi 2010+ extended record tables supply field names when valid, while unnamed/legacy entries use `f<offset>` placeholders. Record and class instance sizes and fields are shown in the UI and CLI JSON. Matching accesses are reported on the instruction with owner type, field path, offset, and known RTTI type. A field load whose RTTI identifies another class or record, or an address-forming `LEA` to an inline record field, also transfers that type to the destination register for subsequent field analysis; indexed addressing, unknown type flow, and unmatched offsets are left unclassified.

The candidate scan also models implicit register effects of `CBW`/`CWDE`/`CWD`/`CDQ`, `SAHF`/`LAHF`, `XLAT`, `PUSHA`/`PUSHAD`/`POPA`/`POPAD`, `CPUID`, `RDTSC`/`RDTSCP`/`RDPMC`, `RDMSR`/`WRMSR`, `XGETBV`/`XSETBV`, `CMPXCHG`/`CMPXCHG8B`, `XADD`, and one-operand multiply/divide instructions; `OUT` port instructions treat their register operands as inputs. `XCHG` reads and overwrites both register operands, including when it consumes a direct call's EAX result. A direct call to its following instruction followed by `POP reg` is treated as an inline PC-acquisition idiom rather than a new procedure or an ordinary caller-saved clobber. Registers used by indirect call targets are counted before the call clobbers caller-saved argument candidates. Read-only register operands of `BOUND`, `VERR`, and `VERW` are counted as inputs rather than mistaken for write destinations. It treats read-modify-write destinations in `ARPL`, `BSWAP`, `ADCX`, `ADOX`, and conditional `CMOVcc` instructions as inputs before they are overwritten. String instructions model implicit EAX input/output, DX port use, and repeat-prefix ECX counters; `LOOP`/`LOOPE`/`LOOPNE` read and decrement ECX, while `JECXZ` reads it. Legacy BCD adjustment instructions (`AAA`, `AAS`, `AAM`, `AAD`, `DAA`, and `DAS`) are modeled as EAX read-modify-write operations. Direct calls to the Delphi runtime checks `@IntOver` and `@BoundErr` are treated as preserving the register arguments, matching the legacy analyzer; unknown and ordinary calls remain conservative clobbers. The test-only `BT` instruction reads but does not overwrite its first operand, unlike `BTS`, `BTR`, and `BTC`.

Absolute immediate code pointers are only followed when their executable-section target is unclassified and passes a bounded procedure-shape check (an EBP frame prologue or a matching initial push/final pop before return). Existing procedure starts remain eligible without revalidation.

When the last detected EAX write before a procedure return is a `SETcc` instruction, or the returned value comes directly from the Delphi runtime helper `@IsClass`, the engine reports a Boolean return-type candidate. The `SETcc` rule matches the legacy analyzer's inference; the named-helper rule uses the helper's known result semantics. `@GetTls` returns the legacy `#TLS` marker. For `@AsClass`, the engine can report a class-name candidate when EDX is loaded or formed with an absolute `LEA` from a recognized VMT address (or copied/swapped from another tracked argument register) before the helper call in a straight-line instruction sequence. `@ClassCreate` can return a known class-name candidate when EAX contains a recognized VMT address; `@BeforeDestruction` preserves a known EAX class type. The register types also survive the known register-preserving checks `@IntOver` and `@BoundErr`. Direct callers and uniquely resolved indirect callers that return these values can inherit the class-name candidate. Based/indexed addresses, unknown register types, and types carried across control-flow instructions remain unclassified. These are conservative candidates rather than definitive prototype types.

`TApplication.CreateForm` assigns the known class type from EDX to the variable addressed by ECX when reached by a direct or uniquely resolved indirect call and the target is a tracked global, local, or positive-offset stack argument. Calls to `@IntfClear` and `@VarClr` annotate directly tracked global addresses with the `IInterface` or `Variant` data-type candidate and type directly tracked local variables and positive-offset stack arguments. `@FinalizeRecord` can annotate a directly tracked global from a known EDX type-info item with a parsed type kind and name; `@FinalizeArray` also uses the known ECX element count to form `array[count] of Type`. `@DynArrayAddRef`, `@DynArrayClear`, `@DynArraySetLength`, and `@DynArrayCopy` mark direct global array arguments with a dynamic-array flag; helpers with known EDX element type information also annotate the array data type. Global candidates include mapped zero-initialized data (`.bss`) as well as file-backed data. The candidate is exposed on analysis items in the UI and CLI JSON. Register values are tracked only through simple full-register moves, absolute `LEA`, and full-register `XCHG`; code addresses and unknown type-info records are not inferred.

`@LStrArrayClr`, `@WStrArrayClr`, and `@UStrArrayClr` annotate EBP-relative local slots when EAX is a directly tracked local address and EDX contains a bounded element count (at most 256). `@FinalizeRecord`, `@FinalizeArray` (with a known nonnegative count representable as a signed 32-bit integer), and dynamic-array helpers also annotate directly tracked local addresses when their RTTI is known; `@DynArrayAddRef` marks a local as `array of ?` when the element type is unknown. A later frame-relative `LEA` can reuse a known local type for subsequent record-field analysis. The resulting local-variable candidates are exposed in the UI and CLI JSON; stack locals reached through unknown pointer arithmetic are not inferred.

The same RTTI-based finalization helpers can type a positive-offset stack argument when its address or value is directly tracked from an EBP-relative access; `@FinalizeArray` also requires a known nonnegative count representable as a signed 32-bit integer. Reloading that argument transfers its known class or record type to the register for subsequent field analysis. Register copies and swaps preserve provenance; a register overwrite, unrecognized call, or unknown RTTI prevents inference.

Calls to `@TryFinallyExit` mark the preceding straight-line instructions back to the nearest conditional branch, the helper call, and its following instruction with the `FinallyExit` flag. Ordinary calls are not marked.

Direct calls to known `@ClassCreate` and `@ClassDestroy` symbols mark the calling procedure as a constructor or destructor candidate, respectively; these candidate flags are included in the normal result flags and CLI JSON.

When a direct callee's EAX result is read before EAX is overwritten, the callee receives a `FunctionCandidate` flag. Imported routines and constructor/destructor candidates are not reclassified.

Calls to the known non-returning Delphi runtime routine `@Halt0` terminate the current code path rather than scanning unreachable fallthrough instructions.

If a procedure returns after a direct call without an intervening EAX write, the callee's available return-type candidate is propagated to the caller after code scanning completes. Propagation repeats to a fixed point so candidates can flow through nested direct-call chains. Non-EAX operations such as stack cleanup do not block propagation.
