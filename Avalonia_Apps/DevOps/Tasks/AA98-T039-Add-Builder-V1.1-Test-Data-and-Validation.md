# AA98-T039 Add Builder V1.1 Test Data and Validation

## Parent
- Backlog Item: `AA98-Bl035 Resume Roslyn Builder V1.1 Starter Slice`

## Tasks
- [x] Add SDK-style sample projects for console, library, test-project, and project-reference scenarios.
- [x] Exclude copied sample source files from the MSTest project compilation and copy them to the output directory.
- [x] Add MSTest coverage for loading, test-project detection, reference resolution, and end-to-end inspection.
- [x] Validate the builder slice with `dotnet test` in the selected starter-slice host.

## Notes
- The current validation host is pinned to `net10.0` to avoid cross-runtime MSBuildLocator incompatibilities in the test process.
- Coverage status: success.
- Test status: success.

## Status
- Completed
