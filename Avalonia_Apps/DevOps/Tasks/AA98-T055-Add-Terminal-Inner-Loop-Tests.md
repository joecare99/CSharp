# AA98-T055 Add Terminal Inner Loop Tests

## Parent
- Backlog Item: `../BacklogItems/AA98-Bl041-Terminal-Inner-Loop-Baseline.md`

## Goal
Add tests for terminal wrapper behavior and Linux-oriented configuration boundaries.

## Scope
- Test shell selection and session configuration logic.
- Test component wrapper behavior that does not require an interactive terminal.
- Document remaining manual smoke checks.

## Execution Notes
1. Avoid brittle tests that require a specific developer shell unless explicitly configured.
2. Separate interactive smoke checks from automated tests.

## Implementation Notes
- Prioritize deterministic tests for shell resolution, session settings, process-launch requests, and output mapping that do not require a live interactive terminal.
- Cover Linux-oriented configuration defaults and fallback behavior explicitly.
- Keep interactive shell behavior in a documented smoke checklist when automation would be environment-specific or flaky.

## Delivered
- Added repeatable fallback terminal tests for the provider-neutral AA98 OS boundary, including explicit shell argument preservation and deterministic stop/exit behavior.
- Added dedicated terminal host tests for Linux-oriented explicit shell configuration, fallback shell selection, and forwarding of working directory, workspace root, shell arguments, and environment variables into process-launch requests.

## Manual Smoke Checklist
1. Launch `AA98.Terminal.Host` on Windows and confirm the default shell starts when no explicit shell path is configured.
2. Launch `AA98.Terminal.Host` with an explicit Linux-oriented shell configuration on a Linux-compatible environment and confirm the session starts with the expected shell identity.
3. Type a command that emits both complete lines and delayed partial output and confirm the console surface shows both correctly.
4. Stop the session from the UI and confirm the terminal reports a stopped state without leaving the child shell process running.
5. Restart a session after stopping and confirm a fresh terminal session is created.

## Acceptance Criteria
- Terminal wrapper logic has repeatable tests.
- Manual smoke checks are documented where automation is not practical.

## Validation
- Run targeted terminal tests.

## Status
- Completed
