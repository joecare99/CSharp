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

There is currently no product UI culture selector or culture-change notification
for either host. `UiStrings.Culture` controls Workbench UI resource lookups,
not the designer's `CurrentUICulture` lookup, and does not notify existing
views. A direct runtime change to `CurrentUICulture` does not reparse an open
preview: an unchanged resource context and unchanged markup are both no-ops.
The existing preview can therefore retain its old caption until another action
actually reparses it. For example, designer undo reparses using the then-current
culture, but also changes the document and is not a refresh command. Reopening
or changing the resource context may reparse; do not rely on an unchanged
context or reopening identical markup to force a refresh. This is a limitation,
not acceptance of live culture switching. A future supported switch must define
notification and safe refresh semantics for both hosts and preserve document,
selection, drafts and history without executing project code.

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
pseudo-classes, complex selectors, resource dictionaries and
arbitrary objects are not loaded. Resource aliases cannot invoke other markup
extensions.

Direct `Border.Background` resource references additionally support a bounded
literal `LinearGradientBrush` in inline or application resources, including the
AA05 sidebar. Both endpoints must be explicit absolute numeric pairs; two to
32 explicit stops must have finite offsets in [0,1] and six- or eight-digit
hexadecimal colors. Only the `LinearGradientBrush.GradientStops` property-element
form and its specified attributes are accepted. Duplicate keys, resource
dictionaries in the lookup path, unsupported nodes, aliases, bindings, relative
or percentage points, extra properties and malformed values leave the brush
unresolved. An included application style may also set a `Button.Background`
to a direct reference to such an application gradient. A local resource with
the same key, any local resource dictionary, an oversized local resource scope,
or a duplicate application key prevents that resolution. Aliases, local
gradient style setters and gradients on other style properties are unsupported.
A later matching Background setter or a source Background attribute/property
element suppresses the gradient; a supported later literal setter wins.
The preview creates its own brush from validated data, never from runtime XAML;
source attributes stay unchanged.

For the calculator example, neutral/culture RESX captions and the local Button
margin/padding are supported when the host supplies its project context. The
included `Button.Number` style also supplies its font size and solid background;
the local Button margin still wins. The included `Button.Menu` style supplies
the literal sidebar gradient while RESX supplies localized menu captions.
The included `Grid.Calc` solid background
is also rendered on the preview Grid. A literal Grid background takes precedence;
unresolved expressions and unsupported brushes remain unset rather than being
executed. The standalone designer and Workbench use the unique closest project in the selected
workspace; without one, they leave expressions unresolved. Unsupported styles
and brushes remain ignored. The bound status
and numeric fields remain inert binding placeholders, and its foreign converter
and data context are ignored. No live localization updates or executed commands
are provided.

The real calculator's Memory and Accumulator TextBoxes have literal
`TextAlignment="Right"` and `TextWrapping="Wrap"`. These literals are previewed
and editable with undo/redo while the original `Text` binding is preserved;
the converter's resulting value is not evaluated or predicted. Ambiguous or
expression-based alignment and wrapping sources are not editable as literals.

The optional real-calculator regression uses `AA98_CALCULATOR_SAMPLE_ROOT` as
the calculator project directory, falling back to the original Windows sample
location. It is inconclusive when the sample is absent; the synthetic resource
and Grid tests do not require it.
