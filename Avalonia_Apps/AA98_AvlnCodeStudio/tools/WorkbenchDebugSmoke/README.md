# Workbench project-debug smoke test

Run this smoke in a desktop session with the .NET 10 SDK and NetCoreDbg
installed:

```powershell
$env:AVLN_NETCOREDBG_PATH = "C:\path\to\netcoredbg.exe"
dotnet run --project tools\WorkbenchDebugSmoke\WorkbenchDebugSmoke.csproj
```

On Linux, point `AVLN_NETCOREDBG_PATH` to the installed adapter and run the
same project under a desktop session such as WSLg. The smoke creates a
temporary net10 debuggee and drives the real Workbench project selection,
inspection, build, runtime refresh and DAP services. It verifies stop-at-entry,
a source breakpoint, stack and local-variable inspection, step-over, continue
and stop. The temporary project uses no user source files or credentials.
