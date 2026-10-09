# Entry JSON templates

Entry templates describe how one family or individual entry is rendered into the generated document. They are declarative JSON: they can select typed fields, add literal text, create links, repeat over supported collections, conditionally render blocks, and include reusable fragments. They do not execute C# or arbitrary expressions.

The built-in templates are:

- `gc` — detailed family entries using the GC-style names and event notation.
- `ak` — compact family entries using the AK-style names and event notation.

A project may select either built-in id (`gc` or `ak`) or the path to an external `.json` template. External templates are validated before use.

## Root properties

| Property | Required | Description |
| --- | --- | --- |
| `schemaVersion` | yes | Currently `1`. |
| `id` | yes | Non-empty identifier for the template. |
| `entryRoot` | yes | `Family` for a family-entry template or `Individual` for an individual-entry template. |
| `blocks` | yes | Ordered array of blocks rendered for each entry. |
| `fragments` | no | Named arrays of blocks that can be reused using an `include` block. |

A template's `entryRoot` must match the entry kind supplied by the host. The current DOCX export pipeline generates family entries.

## Block kinds

Blocks are rendered in array order. A block may only use the properties listed for its kind; unknown properties are rejected.

| Kind | Properties | Behavior |
| --- | --- | --- |
| `paragraph` | `role`, `anchor`, `indent`, `hangingIndent`, `content` | Creates a paragraph, optionally with a style role, bookmark, indentation, and inline content. |
| `text` | `value`, `bold`, `italic`, `underline` | Adds literal text. |
| `field` | `path`, `formatter`, `bold`, `italic`, `underline` | Adds a typed value from the current rendering context. |
| `link` | `target`, `content`, `bold`, `italic`, `underline` | Renders inline content as an internal document link when its target is available. |
| `if` | `condition`, `then` | Renders `then` blocks when the condition is true. |
| `forEach` | `items`, `as`, `template` | Renders `template` once per item in a supported collection; `as` names the item variable. |
| `include` | `fragment` | Renders a named fragment in the current context. Include cycles are rejected. |
| `section` | `columns`, `blocks` | Creates a document section with 1–4 columns. Sections are top-level only and require a document provider that supports sections. |

Inline `text`, `field`, `link`, `if`, `forEach`, and `include` blocks can be placed inside paragraph `content` or link `content`. Paragraph-level blocks such as another `paragraph` cannot be nested into inline `content`.

## Paragraph indentation and style roles

- `indent` sets the paragraph's left indentation in points. Use it to move child entries farther right, for example `"indent": 36`.
- `hangingIndent` sets the hanging-indent configuration in points. It is not a substitute for increasing the whole paragraph's left indentation. It is useful for wrapped lines in a bullet/list paragraph.
- Both indentation values must be between `0` and `1440`.
- `role` selects a logical document style. Supported roles are `section-heading`, `family-header`, `family-data`, `adult`, `children-header`, `child`, and `note`. The document provider determines the concrete formatting for each role.
- `anchor` is a typed path to a bookmark field, such as `person.anchor` or `family.anchor`.

## Conditions

Conditions use a supported typed path; they are not general-purpose expressions. Optional values are true when non-empty/non-null. Collections support these tests:

- `family.children.any` — at least one child.
- `family.children.one` — exactly one child.
- `!family.children.any` — no children.
- `!family.children.one` — anything other than exactly one child (normally used in the `else` half of a singular/plural pair).

A single leading `!` negates the truth value of a supported condition. Double negation is rejected. Other supported collection paths include `family.parents`, `family.properties`, and person `occupations`, `properties`, `childFamilies`, `parentFamilies`, `childFamilyTokens`, and `parentFamilyTokens`, using `.any` or `.one`. Supported optional conditions include family `marriagePlaceShort`, person `parentFamily`/`residence`, and occupation/property `place`.

There is no `else` block in schema version 1. Use two `if` blocks: one for the positive condition and another for its negation.

### Singular/plural child heading

```json
{
  "kind": "if",
  "condition": "family.children.one",
  "then": [{ "kind": "text", "value": "Kind:" }]
},
{
  "kind": "if",
  "condition": "!family.children.one",
  "then": [{ "kind": "text", "value": "Kinder:" }]
}
```

The built-in `gc` and `ak` templates use this pattern.

## Iteration and child numbering

Use a `forEach` block to render each child separately. In a family context, `family.children` is a collection of person models. The child variable can use the same person fields as `person` in the table below. Child models currently carry an `ordinal` assigned by the export pipeline, so `{ "kind": "field", "path": "child.ordinal", "formatter": "ordinal" }` renders `1.`, `2.`, and so on. In templates that use `as: "person"`, use `person.ordinal` instead.

Example child paragraph with a 36-point left indent and a hanging indent of 18 points:

```json
{
  "kind": "forEach",
  "items": "family.children",
  "as": "child",
  "template": [
	{
	  "kind": "paragraph",
	  "role": "child",
	  "anchor": "child.anchor",
	  "indent": 36,
	  "hangingIndent": 18,
	  "content": [
		{ "kind": "field", "path": "child.ordinal", "formatter": "ordinal" },
		{ "kind": "text", "value": " " },
		{ "kind": "field", "path": "child.nameGc" }
	  ]
	}
  ]
}
```

## Typed paths

Paths start with a root variable or a `forEach` variable. Property names are case-sensitive and validated against the typed model.

| Context | Available fields |
| --- | --- |
| Family (`family`) | `number`, `anchor`, `union`, `marriageMark`, `marriageDate`, `marriagePlace`, `marriagePlaceShort`, `marriagePlaceAnchor`, `properties`, `parents`, `children` |
| Person (`person`, `child`, `individual`) | `nameGc`, `nameAk`, `anchor`, `reference`, `indexLabel`, `vitalEventsGc`, `vitalEventsAk`, `additionalLifeDataGc`, `birth`, `death`, `indexAnchor`, `occupations`, `properties`, `residence`, `residenceAnchor`, `ordinal`, `parentFamily`, `childFamilies`, `parentFamilies`, `childFamilyTokens`, `parentFamilyTokens` |
| Occupation (`occupation`) | `name`, `date`, `indexAnchor`, `place`, `placeAnchor` |
| Property (`property`) | `name`, `date`, `place`, `indexAnchor`, `placeAnchor` |
| Family reference (`familyReference`) | `number`, `anchor` |
| Family reference token (`familyReferenceToken`) | `text`, `anchor` |

Collection fields can be iterated with `forEach`; conditions can use `.any`/`.one` where supported. Nested values such as `person.parentFamily.number` are available when the optional parent-family reference exists.

## Formatters

Formatters are safe, named transformations. A formatter must be used with a compatible path.

| Formatter | Compatible path | Output |
| --- | --- | --- |
| `childCount` | `family.children` | Child count and the legacy `Kdr:` suffix. |
| `dateSuffix` | Family union/marriage dates and person birth/death dates | Adds a leading space when the date is present. |
| `gedcomDatePrefix` | A `.date` field | Adds the date followed by `: ` when present. |
| `ordinal` | A person `.ordinal` field | Adds a trailing period, for example `2.`. |
| `vitalEvents` | A person value (`person`, `child`, or `individual`) | GC-style vital-event text. |

## Styling inline content

`bold`, `italic`, and `underline` are supported on `text`, `field`, and `link` blocks. On a link, the renderer always applies underline for link semantics; the other style flags are configurable.

## Safe editing checklist

1. Copy `gc.json` or `ak.json` before creating an external variation.
2. Preserve `schemaVersion`, `id`, `entryRoot`, and the required block arrays.
3. Use only documented block kinds, properties, typed paths, formatter names, and conditions; the validator rejects unknown values.
4. Use `indent` for moving a complete paragraph right and `hangingIndent` for wrapped-line alignment.
5. Validate the template in OFBCreator before exporting a large dataset.
