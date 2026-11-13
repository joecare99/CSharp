# Inert designer resource preview

The preview never loads project assemblies, XAML at runtime, bindings, converters,
view models, resource getters or external URLs. Source attributes remain unchanged;
resolved values are display-only and cannot make an expression editable.

When the caller supplies the document path and its project directory, an
`{x:Static prefix:Type.Key}` expression can display an untyped string from
`Type.resx`. The prefix must map to `clr-namespace:` or `using:` beneath the
namespace of the single `.csproj` file in the project directory. Namespace
segments map to directories; no MSBuild evaluation occurs. The current UI
culture, its parents and the neutral file are checked in that order. XML DTDs,
typed/serialized resources, duplicate keys, project-internal links/reparse points, network paths,
oversized files and values are not loaded. Missing or unsupported entries keep
the original expression as a placeholder. Changes to RESX are reflected on the
next preview parse. Changing the resource context reparses the current preview
without changing markup, undo/redo history or an active grouped property edit.

Inline control resources support literal `x:String`, literal
`SolidColorBrush.Color`, and simple local `StaticResource`/`DynamicResource`
references, with nearest-scope lookup and cycle detection. Styles declared in
ancestor `*.Styles` support exact control-type and `Type.Class` selectors and
allowlisted literal visual setters. Local source attributes override styles.
These are deliberately not a full Avalonia style cascade. Style includes,
application-level resources/styles, theme dictionaries, templates, pseudo-classes,
complex selectors, gradients, resource dictionaries and arbitrary objects are not
loaded. Resource aliases cannot invoke other markup extensions.

For the calculator example, neutral/culture RESX captions and the local Button
margin/padding are supported when the host supplies its project context. The
standalone designer and Workbench use the unique closest project in the selected
workspace; without one, they leave expressions unresolved. Its
application-wide included styles and brushes are not applied. The bound status
and numeric fields remain inert binding placeholders, and its foreign converter
and data context are ignored. No live localization updates or executed commands
are provided.
