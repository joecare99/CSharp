# Property.Editor

`Property.Editor` defines UI-neutral property metadata and draft-provider contracts. Hosts can use the same typed property editor for application settings, user/group/system values, project or solution properties, and other component settings.

## Provider contract

Implement `IPropertyProvider` to expose editable `IPropertyItem` values with `LoadAsync`, `ApplyAsync`, `DiscardAsync`, and (when supported) draft-only `ResetDraftAsync` operations. `PropertyProviderContext` identifies the ownership scope, selected target, storage location, and provider-level read-only explanation. Use `Unspecified` when a legacy provider cannot prove its scope. A provider should not claim a scope or lock unless it reflects the actual storage and policy behavior.

`IConfigUiSection` also implements this contract. Its legacy `SaveAsync` and `ResetAsync` operations remain available; the shared provider adapter maps apply/discard to save/reload and reserves `ResetDraftAsync` for non-persistent edits.

## Value metadata

Each `IPropertyItem` may carry `PropertyValueContext` to explain where its effective value came from, whether it is inherited, and whether an authoritative policy locks it. `ReadOnlyReason` is shown to users instead of presenting a disabled field without explanation. Existing `IPropertyItem` implementations remain compatible through default metadata values.

The metadata describes behavior; it does not enforce operating-system file permissions or provide credential security. Secret values must be stored using a dedicated credential provider, not ordinary configuration persistence.

## Avalonia editor

`PropertyEditorPanel` consumes the neutral property items through `PropertyEditorViewModel`. It emits edit-started and edit-completed notifications so hosts can preserve their own draft, debounce, validation, and undo lifecycle. The AXAML designer uses this contract for its control properties while keeping binding editing and preview-specific behavior in the designer.

Hosts may set `PropertyEditorItemViewModel.IsVisible` to filter presentation without removing the underlying item or changing its draft, validation, or persistence. It defaults to `true`; replacing editor items creates new view models with default visibility. The host must keep hidden invalid items discoverable and restore visibility when its filter ends. Editable controls expose their property display name to accessibility APIs.

`SetItems` keeps existing editor view models when the ordered property objects are unchanged. When the collection is replaced, it disconnects the old editor view models from their property change notifications; consumers should use the current `Items` rather than retain replaced editor view models. `ConfigLayeredUiSection` also rejects writes through property objects replaced by a successful reload, save, or reset, with a validation message instructing the caller to use the current field. A successful shared-draft reload invalidates properties in every participating section immediately, even before those sections are refreshed or if a subsequent refresh fails. Hosts must reload affected sections and reacquire their properties. If the underlying store reload fails before replacing the snapshot, existing properties remain valid.
