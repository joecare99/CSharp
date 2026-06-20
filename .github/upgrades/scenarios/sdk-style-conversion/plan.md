# SDK-style Project Conversion Plan

## Overview

**Target**: Convert the remaining legacy non-SDK-style projects in `CSharpBible.slnx` to SDK-style without changing their target frameworks
**Scope**: 3 legacy .NET Framework 4.8 projects in the solution: one WinForms app and two MSTest projects, with per-project validation because the full solution baseline is already red

## Tasks

### 01-convert-csv-viewer: Convert CSV_Viewer to SDK-style

Replace the legacy project structure in `CSharpBible/CSV_Viewer/CSV_Viewer.csproj` with an SDK-style WinForms project while preserving the existing .NET Framework 4.8 target, Windows Forms behavior, explicit resource and source coverage, configuration-specific output paths, and any assembly metadata that currently lives in `Properties/AssemblyInfo.cs`. This task establishes the converted application project before any dependent test project is updated.

The main concerns are keeping WinForms-specific build behavior intact under SDK-style defaults, avoiding duplicate assembly attributes after conversion, and preserving legacy includes or output settings that are currently spelled out explicitly in the project file.

**Done when**: `CSV_Viewer.csproj` uses SDK-style syntax, still targets .NET Framework 4.8, preserves required WinForms/resource/output behavior, and `CSV_Viewer.csproj` builds successfully on its own with no new warnings or errors in the touched project.

---

### 02-convert-csv-viewertest: Convert CSV_ViewerTest to SDK-style

Convert `CSharpBible/CSV_ViewerTest/CSV_ViewerTest.csproj` to SDK-style after `CSV_Viewer` so the test project can keep its project reference aligned with the converted application. Preserve the current .NET Framework 4.8 target, MSTest behavior, and any package or assembly references currently expressed through legacy MSTest and NuGet imports.

Key concerns are removing the old test/NuGet import pattern cleanly, keeping the project reference to `CSV_Viewer` working after both project files are modernized, and ensuring the converted test project still builds and runs its tests independently of unrelated solution failures.

**Done when**: `CSV_ViewerTest.csproj` uses SDK-style syntax, still targets .NET Framework 4.8, no longer relies on legacy MSTest/NuGet import wiring, the project still references `CSV_Viewer` correctly, the project builds successfully, and its automated tests complete successfully or are explicitly reported with their status if execution is blocked for reasons outside the touched project.

---

### 03-convert-csharpbibletest: Convert CSharpBibleTest to SDK-style

Convert the independent legacy test project `CSharpBible/CSharpBibleTest/CSharpBibleTest.csproj` to SDK-style while preserving its .NET Framework 4.8 target and MSTest behavior. Because this project is not part of the `CSV_Viewer` dependency chain, it can be handled as a separate conversion task with its own focused validation.

The main concern is replacing the legacy MSTest/NuGet import model with SDK-style project metadata without changing test discovery or introducing project-local regressions. Validation for this task should stay project-specific because the overall solution baseline is already failing elsewhere.

**Done when**: `CSharpBibleTest.csproj` uses SDK-style syntax, still targets .NET Framework 4.8, no longer depends on legacy MSTest/NuGet import wiring, the project builds successfully, and its automated tests complete successfully or are explicitly reported with their status if execution is blocked for reasons outside the touched project.

---

### 04-validate-remaining-sdk-style-conversions: Validate the remaining SDK-style conversions

Run final validation across the three converted projects and record the outcome in a way that distinguishes touched-project health from the pre-existing red solution baseline. Confirm that `CSV_Viewer`, `CSV_ViewerTest`, and `CSharpBibleTest` all remain on their original target frameworks, that no project-local `packages.config` remnants or legacy MSTest/NuGet import dependencies remain where conversion removed them, and that the dependent test project still tracks the converted application cleanly.

This task is also where the scenario should capture any remaining unrelated solution-level build failures as baseline noise rather than regressions caused by the conversion work.

**Done when**: The converted projects have each been rebuilt successfully, test status for both test projects is clearly recorded as success/skipped/failed with reasons, no regressions are found in the touched projects, no target framework changes were introduced, and the final validation notes explicitly separate unrelated baseline solution errors from conversion results.
