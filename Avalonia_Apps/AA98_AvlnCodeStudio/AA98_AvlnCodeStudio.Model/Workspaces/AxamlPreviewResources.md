# Inert designer resource preview

The preview never loads project assemblies, XAML at runtime, bindings, converters,
view models, resource getters or external URLs. Source attributes remain unchanged;
resolved values are display-only and cannot make an expression editable.

The shared standalone and Workbench palette includes ToggleButton. Its literal
content and Boolean state are previewed inertly; bindings and commands are not
executed. Insertion and literal property edits use the designer's undo history.

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
These are deliberately not a full Avalonia style cascade. With a unique project
and document context, the root `App.axaml` also supplies literal application
resources and direct `StyleInclude` files under the project root. Only relative,
simple slash-separated `.axaml` paths are accepted; links, network paths,
duplicates, DTDs, nested includes and foreign namespaces are ignored. At most
16 includes, 256 included styles and one million bytes/characters per file are
read. Included exact type/class styles precede local styles and attributes;
later included styles win over earlier included styles. Duplicate include paths
are ignored. Included-style resource references are resolved only against the
application resources: if a local resource shadows any key in the alias chain,
or a local `ResourceDictionary` is present, that setter is omitted rather than
guessing Avalonia's resource semantics. Included-style other markup extensions
are ignored. Direct control references still use local-then-application lookup.
This approximation does not claim Avalonia cascade parity. Theme dictionaries, templates,
pseudo-classes, complex selectors, gradients, resource dictionaries and
arbitrary objects are not loaded. Resource aliases cannot invoke other markup
extensions.

For the calculator example, neutral/culture RESX captions and the local Button
margin/padding are supported when the host supplies its project context. The
included `Button.Number` style also supplies its font size and solid background;
the local Button margin still wins. The
standalone designer and Workbench use the unique closest project in the selected
workspace; without one, they leave expressions unresolved. Unsupported styles
and brushes remain ignored. The bound status
and numeric fields remain inert binding placeholders, and its foreign converter
and data context are ignored. No live localization updates or executed commands
are provided.
