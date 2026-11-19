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

The diagnostic reads the adjacent `.deps.json` (up to 16 MiB), selecting only the target named by `runtimeTarget.name`. It recognizes a `Microsoft.Testing.Platform/<version>` library whose `runtime` contains `Microsoft.Testing.Platform.dll` either as a root-level name or as the final path component. A missing adapter then produces an explicit diagnostic; if adapters exist but VSTest discovers no cases, discovery adds a warning with the same guidance. A runtime asset in dependency metadata does not establish a direct assembly reference or prove that a project is MTP-only: VSTest discovery is still attempted when adapters exist, and nonempty results are unchanged.

Missing, unreadable, malformed, oversized or inconclusive metadata leaves existing VSTest behavior unchanged. Dependency-only references and assets under `runtimeTargets` rather than `runtime` are not recognized; the diagnostic cannot detect every MTP-only project. No MTP execution is provided.
