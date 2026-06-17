# SDK-style Project Conversion Plan

## Overview

**Target**: Convert `CSharpBible/AddPageWPF/AddPageWPF.csproj` from legacy MSBuild format to SDK-style
**Scope**: Single WPF desktop project targeting .NET Framework 4.8 with no `packages.config` and no project-specific tests

## Tasks

### 01-convert-addpagewpf: Convert AddPageWPF to SDK-style

Replace the legacy project structure in `CSharpBible/AddPageWPF/AddPageWPF.csproj` with an SDK-style WPF project while preserving the existing .NET Framework target, output paths, application entry points, generated resource/settings files, and assembly metadata behavior. The conversion must keep the project building in Visual Studio without changing the application's runtime behavior.

Key concerns are the WPF project type, the current `AssemblyInfo.cs` metadata that can conflict with SDK-generated attributes, and preserving configuration-specific output paths that differ from SDK defaults.

**Done when**: `AddPageWPF.csproj` uses SDK-style syntax, keeps the .NET Framework 4.8 target, preserves required WPF/resource/settings behavior, and the project builds successfully after conversion.

---

### 02-validate-addpagewpf: Validate the converted project

Rebuild the converted project and confirm there are no build regressions relative to the baseline. Check that no `packages.config` file is left behind for the converted project and record the validation outcome, including the absence of project-specific automated tests if none exist.

**Done when**: The converted project rebuilds successfully, no project-local `packages.config` remains, and validation notes clearly state the test status for this project.
