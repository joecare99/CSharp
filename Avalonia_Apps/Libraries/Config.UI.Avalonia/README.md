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

If saving is cancelled, the provider may already have written settings. The
dialog reports this uncertainty, retains pending drafts and propagates the
cancellation; runtime activation for that section was not attempted. The
layered provider also reports a confirmed write if cancellation occurs during
the subsequent refresh, and permits an explicit Apply retry or discard.

A cancelled draft reset may already have changed the shared draft; recovery
must discard/reload that group, while independent drafts remain intact. For a
persistent layered reset, cancellation during the write leaves persistence
uncertain, whereas cancellation after the write reports it as confirmed.
