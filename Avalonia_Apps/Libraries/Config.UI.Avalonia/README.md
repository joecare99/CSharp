# Config.UI.Avalonia

The settings view model keeps drafts across section selection. Apply saves changed
sections; explicit Load and Cancel discard the affected drafts.

A failed or cancelled reload may have replaced a provider's backing draft even
when its old properties remain visible to the provider. The affected section is
therefore hidden and cannot be saved until reload recovery succeeds. Its dirty
state is retained until then. Shared-draft recovery also blocks its sibling
sections; retrying recovery does not discard independent drafts. Cancel attempts
to discard all dirty sections, including independent drafts.
Layered-provider cancellation releases the loading state and is propagated;
after a cancelled shared reload the draft may already have been replaced.

Repeated selection of a section already loading reuses that in-flight load;
other sections may still load independently. An older selection completion cannot
clear a newer draft or publish its failure as the current section's status. Selection-load cancellation
is contained at the unawaited selection boundary while recovery is retained.
Programmatic selection during an explicit apply, reset, discard, retry or load is
deferred until that operation finishes; busy remains set through the handoff.
Layered sections also guard their own property generations and retry resolution
if a sibling reload replaced their shared draft. As with other Avalonia view
models, hosts must access view-model state on the UI thread. Sharing mutable
sections between independently edited view models or concurrently invoking
mutating provider operations is not a transaction or conflict-resolution API.

If saving is cancelled, the provider may already have written settings. The
dialog reports this uncertainty, retains pending drafts and propagates the
cancellation; runtime activation for that section was not attempted. The
layered provider also reports a confirmed write if cancellation occurs during
the subsequent refresh, and permits an explicit Apply retry or discard.
The dialog handles cancellation at its async event boundaries: Apply, OK,
Cancel and window-close discard keep the dialog open rather than allowing a
cancelled operation to escape the UI dispatcher. Awaited view-model operations
still propagate cancellation to their callers. The view's Load, Reset, Save and
retry buttons also await their commands at the UI boundary and handle
cancellation there; direct `ExecuteAsync` callers still receive it.

A cancelled draft reset may already have changed the shared draft; recovery
must discard/reload that group, while independent drafts remain intact. For a
persistent layered reset, cancellation during the write leaves persistence
uncertain, whereas cancellation after the write reports it as confirmed.

The Workbench `tools\ProjectSettingsReviewSmoke` exercises the shared dialog
with in-memory layer stores, including in-flight cancellation through the
actual section buttons, post-save cancellation, draft-reset recovery,
independent drafts and retry. Its interaction is programmatic, not physical
keyboard or accessibility acceptance.
