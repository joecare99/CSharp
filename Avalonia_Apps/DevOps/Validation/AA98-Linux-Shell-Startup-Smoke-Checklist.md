# AA98 Linux Shell Startup Smoke Checklist

## Purpose
Provide a repeatable manual smoke validation path for the AA98 Linux shell startup baseline until full UI launch automation is available.

## Scope
- Validate AA98 shell startup on a Linux desktop session.
- Confirm that startup success reaches the main window.
- Confirm that startup failure surfaces actionable diagnostics.

## Preconditions
- Build the `AA98_AvlnCodeStudio.UI` project for the target runtime.
- Run inside a Linux desktop session with the required Avalonia rendering dependencies available.
- Use the current startup baseline from `AA98_AvlnCodeStudio.UI/Program.cs` and `AA98_AvlnCodeStudio.UI/App.axaml.cs`.

## Smoke Steps
1. Start the AA98 shell from the published output or build output on Linux.
2. Confirm that the process reaches the main workbench window without an immediate crash.
3. Confirm that the window contains the expected editor host region.
4. Close the shell and confirm that shutdown completes without a follow-up startup exception.
5. Re-run the shell with a deliberately broken startup registration or unavailable dialog dependency in a diagnostic branch.
6. Confirm that startup failure reports the wrapped AA98 diagnostic message rather than an unclassified raw exception.

## Expected Results
- Successful startup shows the main workbench window.
- Failed startup reports an actionable AA98 startup diagnostic.
- Results can be recorded in task and backlog notes for each Linux validation pass.

## Automation Link
- Automated composition coverage lives in `AA98_AvlnCodeStudio.Tests/Startup/AppStartupCompositionTests.cs`.
- This checklist covers the remaining full-UI Linux smoke path that is not yet reliable in headless automated tests.
