# ConsoleLib.ExtCon

ExtendedConsole-backed widget host for `ConsoleLib`.

## Purpose
- Keep the concrete console and host-loop implementation outside the `ConsoleLib` core library.
- Provide the default console-backed `IWidgetSet` implementation for applications that use `IExtendedConsole`.
- Reference `ConsoleLib` for the control model and `ExtendedConsole` for the concrete terminal integration.

## Current Slice
- Hosts the extracted `ConsoleWidgetSet` and `ConsoleFramework` implementation.
- Targets modern Windows .NET TFMs only.
- Serves as the bridge for existing console applications and tests while `ConsoleLib` remains backend-agnostic.
