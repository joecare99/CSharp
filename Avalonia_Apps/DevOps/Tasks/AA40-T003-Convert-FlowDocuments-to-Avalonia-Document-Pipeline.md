# AA40-T003 Convert FlowDocuments to Avalonia Document Pipeline

## Parent
- Follow-up correction for `AA40_Wizzard` after `AA40-T002 Fix Wizzard Document Rendering and Selection Refresh`

## Goal
Replace the direct WPF `FlowDocument`-XAML rendering path with an Avalonia-native document pipeline that can also be reused by `AA25_RichTextEdit` as the basis for editor conversion.

## Scope
- Analyze and implement a shared transformation from WPF `FlowDocument` XAML into an Avalonia-friendly document/view representation.
- Switch `AA40_Wizzard` from direct `LoadXamlString` usage against WPF-style content to the transformed Avalonia representation.
- Prepare `AA25_RichTextEdit` to use the same transformed representation as the source for editor loading and saving workflows.
- Add regression tests for transformation and consumer integration.
- Validate builds and relevant tests.

## Assumptions
- Existing embedded `.xaml` assets remain the source content format for now.
- A practical first slice may support the subset already present in the documents, especially paragraphs, runs, line breaks, alignment, font weight, font style, font size, and underline.
- `AA40_Wizzard` only needs read-only rendering, while `AA25_RichTextEdit` needs a transformation suitable for loading into the editor control.

## Open Questions
- Whether a later slice should add round-trip editing support back to WPF-style FlowDocument XAML without information loss.
- Whether tables, images inside the document body, and additional text decorations need a dedicated later backlog item.

## Tasks
- [ ] Create a shared Avalonia-oriented document transformation component.
- [ ] Integrate the transformed representation into `AA40_Wizzard`.
- [ ] Integrate the transformed representation into `AA25_RichTextEdit`.
- [ ] Add or update tests.
- [ ] Validate build and relevant tests.
- [ ] Mark this task completed after validation.

## Notes
- `Simplecto.Avalonia.RichTextBox` supports loading and saving Avalonia-side XAML, RTF, HTML, and other formats, so the application should stop binding raw WPF `FlowDocument` XAML directly where possible.
- The viewer path in `AA40_Wizzard` should align with Avalonia-native rendering patterns similar to embedded view loading, but without treating content assets as compile-time application views.
