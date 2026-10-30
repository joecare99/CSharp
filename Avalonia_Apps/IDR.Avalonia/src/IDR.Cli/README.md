# IDR.Cli

`IDR.Cli` is the directly invokable console front end included in the `IDR.Avalonia` solution. It reuses the sibling `IDR.Core` and `IDR.Infrastructure` projects and does not initialize Avalonia.

## Requirements

- .NET 10 SDK to build
- .NET 10 runtime to run framework-dependent publishes
- A matching `kb<version>.bin` Knowledge Base, supplied with `--knowledge-base`, `--knowledge-base-directory`, or `IDR_KNOWLEDGE_BASE_DIRECTORY`

## Build and run

From `C:\Projekte\CSharp\Avalonia_Apps\IDR.Avalonia`:

```powershell
dotnet restore .\IDR.Avalonia.slnx
dotnet test .\IDR.Avalonia.slnx --configuration Release
dotnet run --project .\src\IDR.Cli\IDR.Cli.csproj -- decompile `
  --input .\sample.exe `
  --delphi-version 7 `
  --knowledge-base C:\path\to\kb7.bin `
  --output-directory .\out `
  --progress
```

After publishing, invoke `.\artifacts\cli-win-x64\IDR.Cli.exe decompile ...` directly; unlike the Avalonia apphost, this is a console executable.

The command emits `analysis.json` (the same schema-versioned report used by `IDR.App analyze`), `classes.pas`, and `disassembly.asm`. Full JSON includes every analyzed item and cross-reference and can be large for real-world images; `--summary-only` keeps report counts and diagnostics while omitting item and disassembly arrays. An existing non-empty output directory is rejected unless `--force` is specified. New artifacts are staged before replacing the output directory.

`classes.pas` contains evidence-based class declarations from parsed VMT/RTTI metadata, including parent VMTs, fields, and named methods. Unknown field types fall back to `Pointer`; method signatures are not recovered, and unknown dynamic/virtual method names remain comments with addresses. Identifier cleanup and RVA comments preserve provenance. This output is a declaration draft, not reconstructed original source or a complete, guaranteed-compilable unit.

The `decompile` name describes the batch workflow only: **the first release does not generate Pascal pseudocode**. A full control-flow/decompiler backend is separate work.

Exit codes: `0` success, `1` input/analysis/output failure, `2` invalid version/options or conflicting Knowledge Base arguments, and `130` cancellation. Progress is written to standard error.

## Publish

```powershell
dotnet publish .\src\IDR.Cli\IDR.Cli.csproj --configuration Release --runtime win-x64 --self-contained false --output .\artifacts\cli-win-x64
dotnet publish .\src\IDR.Cli\IDR.Cli.csproj --configuration Release --runtime win-x86 --self-contained false --output .\artifacts\cli-win-x86
```
