# Test discovery

The test-discovery service automatically uses `*TestAdapter.dll` files beside the built test assembly. To use adapters stored elsewhere inside the project directory, add a `vstest-adapters.json` file beside the project file:

```json
{
  "adapterPaths": [
    "TestAdapters/CustomTestAdapter.dll"
  ]
}
```

Paths are relative to the project directory. Absolute paths, `.`/`..` path segments, missing files, non-adapter DLL names, symbolic links, and junctions are rejected. The manifest is limited to 64 KiB and 32 adapter entries. Adapter packages are not downloaded or installed by the discovery service.

Discovery, single-case execution and debugging use VSTest; Microsoft.Testing.Platform (MTP)-only projects are not supported. Run those projects using their MTP runner, or configure a VSTest adapter for a VSTest-compatible project.

When the assembly's adjacent `.deps.json` identifies the MTP runtime, a missing adapter produces an explicit diagnostic. If adapters exist but VSTest discovers no cases, discovery adds a warning with the same guidance. An MTP reference does not prove that a project is MTP-only: coexistence with VSTest is supported, and nonempty VSTest results are unchanged. Missing, malformed, oversized or inconclusive dependency metadata leaves existing VSTest behavior unchanged; the diagnostic cannot detect every MTP-only project.
