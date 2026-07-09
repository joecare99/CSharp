# Feature F-WorkbenchBuilder-03 - V1.2 compilation, emit, and debug symbols

## Status

In Progress

## Parent

- Epic `E-WorkbenchBuilder-001` - `Roslyn-based build workbench`

## Goal

Define and implement the first Roslyn-based compilation slice that can emit assemblies or executables, produce debug symbols, and prepare the path toward source-generator-aware builds.

## Summary

This feature is the active major expansion after the V1.1 inspection baseline. The current codebase already turns inspection results into practical Roslyn compilation inputs for the supported first slice and can emit artifacts for ordinary console and library projects.

The user intent already established that executable output is required when the inspected project is neither a library nor a test project. Debugging support should be considered through symbol generation, and source generators are an important later compatibility target. The feature should therefore begin with a tightly scoped first emit slice rather than trying to reproduce the full MSBuild build graph in one step.

The first V1.2 emit slice is now implemented for the supported SDK-style class-library and console-project baseline, continues to classify test projects explicitly, and refuses emit for test-project shapes instead of treating them as ordinary executables. Portable PDB generation is the default symbol strategy for the supported emit scenarios, while source-generator support remains an explicit follow-up planning track rather than an implicit promise of the first implementation increment.

## In Scope

- Define the first compilation-oriented service contracts for V1.2
- Map inspection results into Roslyn compilation inputs
- Emit library artifacts for supported class-library project categories
- Emit executable artifacts for supported non-test console project categories
- Classify test projects explicitly and keep them non-emit in the first V1.2 slice
- Generate Portable PDBs by default for supported emit scenarios where practical
- Add failure-oriented NIO sample coverage for compiler and emit diagnostics
- Format diagnostics so users and IDE consumers can navigate directly to file, line, and column information
- Record the first explicit source-generator integration requirements
- Add tests that validate emit success for small sample projects

## Out of Scope

- Complete reproduction of all MSBuild targets and custom tasks
- Broad support for every UI or platform-specific workload in the workspace
- Full IDE debugging integration
- Advanced incremental build caching in the first emit slice

## Acceptance Criteria

- A documented V1.2 service boundary exists between inspection and compilation
- Supported class-library sample projects can emit the expected assembly artifact
- Supported console sample projects can emit the expected executable assembly artifact
- Debug-symbol generation requirements are explicit, default to Portable PDB, and are testable
- Test projects and non-test executable projects are distinguished correctly for emit decisions, with test projects remaining non-emit in the first slice
- At least one intentionally broken sample project proves that compiler diagnostics remain visible with location data
- Plain-text failure output is precise enough for users to find the faulty source location and follows an IDE-friendly location format
- The feature records the minimum source-generator expectations before implementation proceeds too far

## Dependencies

- Stable V1.1 inspection and reference-discovery contracts
- Current host-neutral builder core architecture
- Roslyn package and emit design decisions to be refined in follow-up backlog and tasks

## Risks

- Source-generator support may require more project-system fidelity than initially expected
- Emit decisions may be incorrect if V1.1 project classification remains too naive
- Debug symbol and path mapping behavior may differ across platforms and environments
- Project categories beyond ordinary console and library workloads may introduce early complexity

## Open Questions

- Which exact supported project shapes belong to the first emit slice beyond the initial class-library and console baseline?
- Should analyzer and source-generator handling be split across separate sub-slices?
- What minimum artifact layout is required for executable scenarios?
- Which host options are needed to trigger emit without destabilizing the current inspection host?
- Which additional NIO project classes should follow the first broken-source sample: missing references, invalid project metadata, or broken project references?

## Next Refinement Steps

1. Continue hardening the implemented V1.2 service boundary and supported emit scenarios
2. Expand failure-oriented samples and diagnostic assertions where gaps remain
3. Split source-generator support into explicit planning and implementation work
4. Broaden end-to-end emit coverage only after the current baseline stays stable
