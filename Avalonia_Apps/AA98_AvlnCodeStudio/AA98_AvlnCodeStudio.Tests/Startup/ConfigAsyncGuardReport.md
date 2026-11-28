# Config-UI async lifecycle investigation

Status: fixed in the locally referenced `Config.UI.Avalonia` and
`Config.UI.ConfigService` projects. The original AA98-only investigation was
blocked; the subsequent change was made in those local component projects.

## Reproduction

A temporary MSTest used the actual `ConfigUiViewModel`, `PropertyEditorViewModel`,
`ConfigLayeredUiSectionRegistry`, and `ConfigLayeredSettingsResolver`, with two
registered string-property sections and fake user/system layer stores. The user
store returned an empty snapshot; the system store returned a gated task for its
first load and immediate empty snapshots for subsequent loads. The constructor
started loading First before the test selected anything. The test switched
`SelectedSection` programmatically while `IsBusy` was true:

1. First's system load waits; select Second and wait for its property.
2. Select First again and wait for its second load to display the property.
3. Set that property to a new draft and verify `HasUnsavedChanges`.
4. Release the original First load and wait for `IsBusy` to become false.

On Windows and Linux net10 the editor displayed the old default instead of the new draft.
The first failed assertion was the value check, so the subsequent dirty assertion
was not observed. The code nevertheless unconditionally removes the section from
`_sectionsWithUnsavedChanges` on successful completion. Before the fix, in the external
`ConfigLayeredUiSection.RefreshPropertiesAsync`, the older resolution replaces
`Properties` and invalidates the newer property generation; in
`ConfigUiViewModel.LoadSelectedSectionAsync`, the completion clears dirty state
and displays it whenever the same section is selected, without an operation
generation check. The shared draft itself can still contain the newer value; the
test did not inspect persistence.

A second test selected Second while First waited, then completed First with an
`InvalidOperationException` carrying a non-secret marker. After both loads
finished, Second remained selected but `OperationMessage` contained First's
failure and the marker. The external section copies exception messages into
`ErrorMessage`, and the view model publishes them globally. Provider exceptions
containing sensitive data could therefore expose it in the UI; no actual secret
or logging disclosure was tested. The dialog's standard list and editor are
disabled by `IsBusy`, so these reproductions exercise the public integration
contract, not ordinary mouse interaction with the standard dialog.

The permanent gated tests now verify that A→B→A reuses the pending A load,
preserves subsequent edits and dirty state, and that late failures and
cancellation do not replace the other section's status. Section-internal tests
cover overlapping callers, older completion while a newer load is
pending, and a sibling shared-draft reload during resolution. An explicit reload
defers selection until the operation completes without a false idle handoff.
These guards do not implement a policy for simultaneous mutating operations from
independently edited view models.

## Validation and limits

Initially, Windows and Linux net10 reproduced both failures. After the fix,
`SharedConfigDraftIntegrationTests` passed 23/23, standalone Config.UI.Avalonia
tests 48/48 and Config.UI.ConfigService tests 33/33 on each platform, using
separate isolated artifact roots. The actual dialog smoke exercised cancellation
and close while operations were busy on both platforms. Its first Windows attempt
failed because the `C:\Projekte` junction is rejected by configuration root
protection; running from the physical `D:\Projekte` path passed.

Whitespace format verification passed on both platforms for the changed code,
including a final recheck after the last changes. Builds emitted existing NU1701,
CA1416, CS8600, CS8603 and smoke `Program.cs` AVA2003 warnings; no new warnings
were observed in the changed lines. An earlier format verification attempt on
the temporary test failed during workspace loading with missing MSTest/NSubstitute
symbols despite the successful net10 test build; the temporary reproduction was
removed after durable regressions were added.

No real credentials, full solution suite, physical keyboard testing, or
exhaustive provider/validation race was run. View-model access is expected on
the Avalonia UI thread. Provider exception messages are still presented for the
relevant failing operation; providers must not put secrets in exception text.
