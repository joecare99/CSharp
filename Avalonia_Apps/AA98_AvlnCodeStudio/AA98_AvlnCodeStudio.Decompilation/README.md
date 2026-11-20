# Read-only assembly decompilation

This component provides metadata listing and opt-in per-type C# decompilation for managed PE assemblies. It reads assembly files without loading or executing their code. `InspectAsync` is metadata-only and creates no files. `DecompileTypeAsync` writes only the selected generated source under a configured artifact root and returns a `CodeLocation` that an editor host can open and navigate to.

Navigation locates type declarations in the generated text, including delegate declarations with a return type and generic delegates. Declaration-like text within an attribute line is not treated as a class or delegate declaration. It is a text-based best effort; if no declaration matches, the location falls back to line 1.

Workbench DI registration stores generated files in the current user's local application data at `AA98_AvlnCodeStudio/Decompilation`, outside the project tree. The service requires an explicit artifact root when constructed directly, rejects paths overlapping the inspected assembly's directory, rejects reparse points in artifact paths, writes with create-new semantics, and bounds input assemblies to 512 MiB, metadata to 50,000 types/25,000 members per type, and generated output to 16 Mi characters. Consumers can inspect the `CodeLocation` or use the existing editor navigation service to open its artifact and move to the selected type.

In the Workbench, choose **File → Decompile assembly...**, explicitly select one local managed DLL or EXE, select a listed type, and choose **Open source**. The resulting artifact opens through the normal editor navigation, separately from the original assembly and project sources. Closing the dialog cancels pending work; changing the selection invalidates older results. Empty assemblies and errors are reported in the dialog.

The type list is keyboard-focusable and named for automation; the named status is a polite live region. Escape closes the dialog without opening source. Dialog, menu, picker and status strings use UI resources, with German translations and neutral English fallback.

There is no automatic DLL discovery, project scanning, project mutation, source-file creation in the assembly directory, or execution of the target assembly. This is not a sandbox for malicious input: the decompiler parses untrusted PE/metadata and may read local dependency metadata while resolving references. Use only inputs from sources you are willing to inspect and keep the decompiler package current.

## Workbench window smoke

`tools/WorkbenchDecompilationSmoke` runs a real desktop Workbench and dialog with German resources. It substitutes the file picker with a known local fixture assembly and wraps the real service to delay responses; it checks the actual list, automation properties, Escape, status, generated artifact, editor and `CodeLocation` caret. It also checks picker cancellation, a late picker result after close, closing during a delayed response, and changing tabs during a delayed response. The smoke does **not** exercise the native file picker or constitute a physical screen-reader acceptance test. Run it from a physical (non-reparse) project path with a graphical desktop and isolated build artifact root, after a successful restore.

## Dependency and legal review

- Direct dependency: [`ICSharpCode.Decompiler` 11.2.0.10050](https://www.nuget.org/packages/ICSharpCode.Decompiler/11.2.0.10050), the ILSpy decompiler engine; package metadata declares the MIT license and targets .NET Standard 2.0 (compatible with the supported .NET 8/9/10 targets).
- Upstream: [ILSpy repository](https://github.com/icsharpcode/ILSpy), pinned package repository commit `c86190ef4e24f3137c13a9275316042842d2fada`; upstream [MIT license](https://raw.githubusercontent.com/icsharpcode/ILSpy/master/LICENSE). The MIT copyright and permission notice must accompany redistributed copies/substantial portions. No third-party implementation source is copied into this repository.
- The package declares `System.Collections.Immutable` 9.0.0 and `System.Reflection.Metadata` 9.0.0 for its .NET Standard target; both NuGet manifests declare MIT. On this component's .NET 8/9/10 targets, the assets file resolves them from the shared framework. The [third-party notices](THIRD-PARTY-NOTICES.md) record package versions, notice obligations, and the dependency-manifest evidence.
- This package/license review covers the selected direct package and its declared runtime dependencies, not every package already used by the application. Redistributors should include the upstream notice in their third-party notices and re-run license/security review when upgrading dependencies.
- Decompilation legality depends on the assembly, license, jurisdiction, and intended use. The tool does not grant permission to reverse engineer or redistribute results and is not legal advice. Users are responsible for ensuring they have the rights to inspect and use the output.
