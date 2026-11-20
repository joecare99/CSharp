# Config.UI.Avalonia

The settings view model keeps drafts across section selection. Apply saves changed
sections; explicit Load and Cancel discard the affected drafts.

A failed or cancelled reload may have replaced a provider's backing draft even
when its old properties remain visible to the provider. The affected section is
therefore hidden and cannot be saved until reload recovery succeeds. Its dirty
state is retained until then. Shared-draft recovery also blocks its sibling
sections; retrying recovery does not discard independent drafts. Cancel attempts
to discard all dirty sections, including independent drafts.
