# GEDCOM to DOCX Export

## Status

Core DOCX path and synthetic regression coverage are implemented. The family section is grouped as family-name group → individual family name → numbered family entries; families without a usable surname appear in a final `Familien ohne Namen` group. Family entries precede indices and are navigable through internal bookmarks/hyperlinks. Built-in GC and AK JSON templates and external family-entry templates can be selected with `--template` (GC default). Named project defaults, repeatable dated/undated occupations, and real section/column rendering are implemented. Confirm DocX licensing before distribution.

## Shared OFB project format and occupation rendering

- `OFBCreator.Projects` owns the host-independent project model, persistence, schema migration, and user-local recent-project catalog. Both CLI and future Avalonia UI use this library and portable format.
- Portable project files use the `.ofbproject` extension and can live in any user-selected directory. `generate --project <path.ofbproject>` opens one, or creates it with defaults when absent. A simple project name remains supported through `%APPDATA%\OFBCreator\Projects\<name>.ofbproject`.
- `%APPDATA%\OFBCreator\Projects\recent.json` is only a recent-project catalog; project settings live in the portable file, not in a competing catalog-specific format.
- Project schema 4 stores stable project identity, name/title, source and output paths, data-source choice, place filter, descendant option, preface, legend, selected template, ordered export rules, grouping threshold, and editorial grouping decisions. Relative source/output/template paths are resolved from the owning project file.
- Existing schema-1 `<name>.json` and schema-2/schema-3 `.ofbproject` projects are loaded and migrated in memory without changing their source files. Use `project-migrate --project <legacy-name-or-path> --output <new.ofbproject>` to explicitly save schema 4. Schema 1 receives a new stable project ID; later schemas retain their ID.
- `--project-template <path-or-name>` initializes a new portable project from an existing one. It copies layout, publication, and export-rule settings with fresh rule IDs, but does not copy source or output file paths. It is valid only when the destination does not already exist.
- `--title`, `--input`, `--output`, and `--template` override project defaults for one export. Existing invocation without a project continues to require `--title`, `--input`, and `--output`, and defaults to the built-in `gc` template.
- Project writes use a same-directory temporary file and atomic replacement. The user-local recent-project catalog is updated separately and does not alter portable project contents.
- Project rules are provider-qualified by stable external record identifiers (for GEDCOM, for example, `gedcom:person:I7`). They support person/family include and exclude rules, person field replacements or pseudonyms, date redaction/year/decade generalization, place redaction/replacement, and one-based fact-type occurrence exclusion. Rules are ordered, enabled independently, validated against a closed field/action allowlist, and contain no executable expressions.
- When a scope has include rules, unmatched records in that scope are excluded; matching include/exclude rules are applied in ascending order, with the last matching decision winning. Replacements and redactions are also applied in order. Missing/stale targets are reported as warnings; a date that cannot safely be generalized is an export error.
- The CLI applies project rules to a detached export view before family grouping, sorting, indices, and template rendering. Imported genealogy objects and their facts are not modified. Redacted/replaced places also lose provider IDs, coordinates, notes, and parent-place links from the export view.
- Surname grouping now generates typed phonetic and repeated-parent/family-surname evidence with scores. Families with no real surname are retained under the `Familien ohne Namen` section rather than discarded; placeholder surnames (values with no letters, fully parenthesized values, `NN`, and `NA`, case-insensitive) are excluded when selecting the family's representative surname. Real child surnames take precedence, with real parent surnames as fallback. Exact parent-to-selected-surname continuity suppresses transition evidence for that source family. Candidate edges are retained when they are at either endpoint group's minimum phonetic distance, with ties retained. The project default auto-accept threshold is 85/100; weaker candidates stay separate and reviewable. Automatic acceptance requires stable provider-qualified family targets. Projects can persist `acceptMerge`, `rejectMerge`, or `manualMerge` choices; manual merge labels are applied to the resulting group. Decisions that no longer match current source groups are warned and ignored.
- `OFBCreator.Avalonia` is registered in the solution as the desktop host and edits the same portable `.ofbproject` through `OFBProjectStore`. Its MVVM workspace provides new/open/save and recent-project actions, source/output and publication settings, built-in/external template validation, ordered privacy-rule creation/editing/reordering/removal (including fact occurrence exclusions), grouping evidence preview and accept/reject/manual-merge decisions, diagnostics, export status, and cancellation. Export and grouping preview use the host-neutral `OFBCreator.Publishing` pipeline shared with the CLI; the Avalonia host references Publishing directly and does not depend on the Console executable. The shared pipeline reports progress through a host-neutral callback, which the CLI maps to its existing console output channels. Cancellation is honored by import/preview operations but is not yet wired through every synchronous rendering step.
- CLI project execution now inherits the saved data source, place filter, and descendant setting when there is no command-line override; `--no-include-descendants` explicitly disables a project's descendant option.
- `OFBCreator.Avalonia.Tests` covers project save/open, export-rule validation/editing/order/fact-occurrence exclusion, built-in template validation, grouping decision editing, and an end-to-end CLI-versus-workspace export using the same `.ofbproject` and synthetic GEDCOM. The acceptance test compares paragraph text, bookmark names, and hyperlink targets. It exposed and fixed the CLI's previously missing project place-filter/data-source defaults. Current multi-target verification passes: Avalonia 13/13, Console 6/6, Publishing 31/31, Core 164/164, and Projects 14/14 on net8.0, net9.0, and net10.0 where supported.
- The canonical GEDCOM-to-OFB adapter retains every person `OCCU` fact and optional date. Inline family details show one preferred occupation: the first undated occupation, falling back to the last occupation when every occupation is dated. Every dated occupation, including the last one, also appears as a dated non-vital event with a link to the occupation index. All occupation facts remain available to the occupation index.
- `OFBCreator.Projects.Tests` covers portable-path persistence, recent catalog tracking, schema-1/schema-2/schema-3 migration, future-schema rejection, export-rule and grouping-decision persistence/validation, and project-template cloning.
- Earlier focused verification after schema 4 and evidence-scored grouping passed on net8.0: Projects 14/14, Core 145/145, and Console 22/22. The DOCX integration case confirms pseudonyms reach family entries and indices, a redacted marriage place does not break the original place filter, and source IDs/names do not appear in the exported text.
- Synthetic DOCX integration coverage verifies repeated dated and undated occupations, occupation index bookmarks, and resolved hyperlinks.

## Evidence-scored family grouping

- The Avalonia workspace has a dedicated `Grouping` tab, separate from `Privacy & Filters`, for previewing surname groups, reviewing candidates, and recording editorial decisions. A GEDCOM selected in the source tab or loaded from a project triggers the grouping preview automatically when the input file exists.
- Automatic merge candidates require a complete known-parent transition: each known parent's surname must match an existing family-name group and differ from the current family's surname group. Thus one known parent requires two distinct groups; two known parents require three distinct groups (both parents and the children/family surname). A source family with an exact parent-to-selected-surname match supplies no transition evidence. Placeholder names are centrally classified (no letters, fully parenthesized, `NN`, or `NA`, case-insensitive) and excluded from surname selection. The most frequent real child surname is selected when available; otherwise a real parent surname is used. Families with no real surname remain in the `Familien ohne Namen` section for later publication. Phonetic similarity alone is not a candidate. Among evidence-backed candidates, a candidate is retained when it is at either endpoint surname group's minimum phonetic distance; ties are preserved.
- The grouping tab's two selectors and `Merge` action allow manual merging without an automatic candidate. They create a standard persisted `manualMerge` decision using stable GEDCOM family targets; project persistence still occurs on Save. The optional manual group label defaults to the left selected name.

- The console uses `OFBFamilyGroupingService` in place of the silent global fuzzy merge. It returns typed candidates with phonetic-distance and repeated parent-to-family-surname evidence, scores, stable family anchors, review status, and stale-decision diagnostics.
- Default automatic acceptance requires 85/100 evidence and stable family identifiers. Lower-scoring suggestions remain separate; projects may persist accept/reject/manual-merge decisions, including custom labels. A manual merge can be applied even when automatic evidence is absent.
- `.ofbproject` schema 4 persists the threshold and decisions. Schemas 1–3 migrate in memory and require the explicit `project-migrate` save. Project templates clone grouping settings and decisions with fresh decision IDs.
- `OFBCreator.Projects.Tests` passes 14/14, `OFBCreator.Core.Tests` 145/145, and Console DOCX integration tests 22/22 on net8.0.
- The first Avalonia review editor and cross-host acceptance are implemented. Follow-up UI refinements may add richer before/after previews for rule transformations and route detailed pipeline progress to the desktop instead of `System.Console`.

### Agreed template contract

- First phase templates render only `Family`/`Individual` entries. Title page, family grouping, indices, preface, and legend remain in the existing pipeline; templating additional indices may be considered later.
- The declarative JSON grammar includes `section`, `paragraph`, `text`, `field`, `link`, `if`, `forEach`, and `include`. Includes use named fragments; unknown fragments and include cycles are validation errors. No embedded code or arbitrary expressions.
- A `section` creates a real DOCX section. All page settings are inherited except column count; sections are top-level blocks, may specify 1–4 columns, and omission inherits the active column count.
- `gc` and `ak` are built-in templates; an external JSON file can also be selected with `--template <name-or-path>`, defaulting to `gc`. The former `--entry-format` option has been removed.
- Both roots expose typed family/person data, and all current person/family/index anchors and backlinks must remain valid. Safe named formatters and logical style roles are validated before rendering.

## JSON entry templates

- The template engine validates schema version 1 before rendering. Its declarative blocks are `section`, `paragraph`, `text`, `field`, `link`, `if`, `forEach`, and `include`; includes use named fragments and are checked for missing names and cycles.
- Family entries no longer receive generated empty spacer paragraphs. Built-in templates place each family bookmark on its actual heading/data paragraph; for external templates without a family anchor, the renderer adds the bookmark to the first paragraph the template emitted, preserving navigation without a blank anchor paragraph.
- Built-in GC and AK family entries now restore a half-line gap before each family through six points of paragraph `spaceBefore`, not an empty spacer paragraph. The hierarchy is visible in both layouts: family entry at the left edge, parents and children heading one level in, child rows one further level in with hanging alignment, and event/occupation detail lines aligned beneath the related person. The DOCX adapter applies hanging indentation before explicit left indentation so WordprocessingML preserves both values instead of collapsing a child row to its hanging indent.
- Built-in GC and AK entries render only dated, listable non-vital events as separate paragraphs; undated facts and `Info`, `Sex`, `Title`, `Religion`, and `Description` facts are excluded from that list. Dated occupation facts are included, even when they are the last fact, and link to the occupation index. Only the preferred occupation is shown in the dedicated inline occupation presentation; undated occupations are not repeated in the event list. The event symbol is used when configured, otherwise the template's localized legend meaning (with a German fallback for common GEDCOM fact types); an optional fact-specific place preposition and short linked place are emitted only when a place exists. Additional data and available associated person/family targets become inline text or links. Existing vital inline facts remain inline. The person's title appears on the name line, and religion follows references before vital dates. Property, residence, marriage and reference details keep their dedicated presentations.
- Families with neither a marriage record/date/place nor children are removed after project rules and before grouping, numbering, indices, and document composition. A `MARR` record is retained as marriage evidence even when it has no date; families with children remain even without a marriage record.
- Non-vital event visibility is calculated from the families included in the export: a parent’s list appears only in their first marriage-evidenced family, falling back to their first emitted parent-family when no marriage exists; a child’s list is suppressed when they also occur as a parent in another emitted family. Template includes may bind the `person` context to a child model explicitly so GC and AK use the same event fragment without host-specific rendering logic.
- Fields resolve only through the typed Family/Individual/Person/Occupation model allowlist. Formatters are named and fixed (`childCount`, `dateSuffix`, `gedcomDatePrefix`, `ordinal`, and `vitalEvents`); executable expressions and unknown properties are rejected.
- Templates select the logical roles `section-heading`, `family-header`, `family-data`, `adult`, `children-header`, `child`, and `note`. The DOCX provider maps roles to its built-in styles.
- Top-level `section` blocks create real Word sections. The DOCX provider preserves the page size, margins, orientation, and other section settings, changing only the requested 1–4 column count. If `columns` is omitted, the active count is inherited.
- Built-ins are embedded from `OFBCreator.Publishing/Templates/gc.json` and `ak.json`. External `.json` template paths are accepted by the template store and the `generate --template <name-or-path>` option; the former `--entry-format` option has been removed. `gc` remains the default.
- Family-entry output remains in the established book pipeline, including grouping, family-number anchors, indices, and internal backlinks. The renderer also accepts an `Individual` root; wiring standalone-individual selection into the export pipeline is still open, so `generate` currently requires a `Family`-root template.
- `OFBCreator.Publishing.Tests` passes 36/36 on net8.0, net9.0, and net10.0. It validates both built-ins, external JSON selection, individual-root rendering, project pseudonym application through DOCX entries and indices, rejection of unknown fragments/cycles/expressions/invalid columns, real DOCX section breaks with inherited page settings and column counts (file and stream saves), GC/AK links and occupation behavior, preferred inline occupations, dated occupation events including the final fact, retention of marriage-only/children-only families, removal of empty unmarried families, sequential family numbering, family anchors without blank spacer paragraphs, person/title/religion order, date-only event-list filtering, and child/event indentation in emitted WordprocessingML.

## Provider-neutral genealogy and GEDCOM roundtrip foundation

### Implementation status

- `Genealogy.Core` and `Genealogy.Gedcom` are separate projects beside OFBCreator. The core has no GEDCOM, OFBCreator, or WinAhnenNew project dependency.
- The core model provides stable GUID identity, provider-specific identifiers, typed person/family/source/repository/media/place/note records, ordered facts and user-defined event trees, directed associations, and structured diagnostics.
- The GEDCOM input driver recognizes GEDCOM 5.5, 5.5.1, and 7.0 version headers, reads UTF-8 with or without a BOM, and for invalid UTF-8 with declared `CHAR ANSI` or `CHAR ASCII` attempts a warned Windows-1252 fallback. It projects person/family/source/repository/media/place/note and other cross-reference records plus directed relationships, and retains the complete ordered parsed syntax tree in an opaque provider extension. Unknown/private tags remain in the canonical content tree.
- Cross-version output applies selected conversions from the official [GEDCOM migration guide](https://gedcom.io/migrate/): shared NOTE pointers, legacy identifiers, name transliterations, association roles, and selected version-specific enumerated values are mapped. Unsupported legacy or extension structures are retained as marked owner NOTE text and produce structured warnings. GEDCOM 5.5.1 NOTE text is wrapped with CONC continuation records when necessary to keep generated lines within 255 characters.
- `OFBCreator.Console gedcom-roundtrip --input <file> --output <file> [--version 5.5.1|7.0]` exposes the new drivers through DI. Recovery still blocks output by default. `--allow-recovery` explicitly permits best-effort output and emits `GEDCOM_BEST_EFFORT_OUTPUT`; skipped malformed lines or repaired text may be lost, so the result requires review. Output bytes and the GEDCOM `CHAR` header are UTF-8. The `generate` GEDCOM source reads through the canonical driver and adapts into the existing OFB rendering contract.
- Synthetic same-version roundtrip tests cover 5.5.1→5.5.1 and 7.0→7.0, including repeated facts, references, sources, repositories, media, notes, continuation lines, private extension trees, and associations. Tests also cover cross-version mappings/fallback, canonical model edits, UTF-8 output, strict and explicitly allowed recovery, Windows-1252 fallback, and malformed-line diagnostics. At the GEDCOM checkpoint, all 15 GEDCOM provider tests and 6 then-current Console tests passed on net8.0. No local GEDCOM data is used by repository tests.

### Remaining acceptance work

- The provider is a first roundtrip slice, not yet acceptance of the full GEDCOM 5.5.1/7.0 standards. All parsed child records are retained, but richer standard concepts still need dedicated typed projections and conformance coverage.
- Cross-version conversion covers selected, documented differences, including case normalization for selected `NAME.TYPE`, `FAMC.STAT`, `FAMC.PEDI`, and `RESN` enumerations and the `SLGC.STAT` `DNS/CAN`/`PRE-1970` spelling changes. The full migration guide includes additional special cases; standards-wide target validation and explicit mappings for all structures and values remain unimplemented. Generic fallback preserves the original syntax subtree as JSON text in a marked NOTE rather than silently dropping it.
- Malformed lines are skipped with line-numbered diagnostics; invalid UTF-8, duplicate/unresolved cross-references, invalid record boundaries, unsupported versions, and illegal nesting are diagnosed and mark the import as not roundtrip-accepted. The writer refuses these by default. The CLI's explicit `--allow-recovery` switch creates a warned best-effort output instead.
- The `generate` workflow now uses the canonical GEDCOM reader through a compatibility adapter. The adapter currently projects the core person/family relationships, common person/family facts, dates, and places needed by the OFB pipeline; sources, media, rich citations, and full event data still need end-to-end migration.

## Local integration references

The following GEDCOM files are local integration references and must not be copied into the repository.

| File | Version | Purpose | Characteristics |
| --- | --- | --- | --- |
| `D:\Projekte\Delphi\Data\ParseFB\EntryGC0057ff.ged` | 5.5.1 | Historical family-book import | 35,775 lines; UTF-8; source-backed family narratives; `CONT`; explicit spouse and child links. |
| `D:\Projekte\Delphi\Data\GenData\GRAMLICH_15.01.2017_00.ged` | 5.5.1 | Historical genealogy import | 49,207 lines; UTF-8; qualified dates; hierarchical places; `FAMC` and `FAMS`. |
| `D:\Projekte\Delphi\Data\ParseFB\OsBObr0427ff.ged` | 5.5.1 | Tolerant historical import | 17,340 lines; UTF-8; names without slash delimiters; incomplete families; `CONC` and `CONT`. |
| `C:\Projekte\Delphi\Data\GenData\Muster_GEDCOM_UTF-8.ged` | 5.5.1 | Standard reference | 1,403 lines; UTF-8; multiple names; private tags; location records; line continuations. |
| `C:\Projekte\Delphi\Data\GenData\muster_gedcom7.ged` | 7.0 | GEDCOM 7 integration reference | 18,378 lines; synthetic; persons, families, repositories, sources, objects and shared notes. |
| `C:\Projekte\Delphi\Data\GenData\Muster_GEDCOM_UTF-8_GEDCOM7_erweitert.ged` | 7.0 | Extended GEDCOM 7 reference | 3,799 lines; synthetic; events, notes, sources and family links. |

### Additional local corpus smoke test

The local `C:\Projekte\Delphi\Data\GenData` tree contains 46 `.ged` files (about 200 MB total), including several important `Muster` fixtures. These remain external local references and are never copied into repository tests. The `gedcom-roundtrip` CLI was run against the corpus with outputs directed to temporary directories, which were removed afterward. No input file was changed and no GEDCOM content was copied into the repository.

- 32 files completed import and roundtrip output without import-recovery rejection. This is a smoke test, not proof of full semantic or standard conformance.
- In the initial strict-mode sweep, 14 files were blocked because recovery was required. The results below are the original sweep; targeted re-runs since then include the explicit best-effort mode and the Windows-1252 fallback.
- Version conversion and fallback diagnostics were emitted on some successful outputs. Those outputs still need semantic review before being considered accepted conversions.
- Targeted verification confirmed successful output for `Muster_GEDCOM_UTF-8.ged`, `Muster_GEDCOM_UTF-8_GEDCOM7_erweitert.ged`, and `muster_gedcom7.ged`. The current `Muster_GEDCOM7_erweitert.ged` also writes with `--allow-recovery`, but emits 90 malformed-line and 699 orphan-level diagnostics; this is only an extraction attempt, not a conformant roundtrip. All three `GRAMLICH_15.01.2017_00/01/02.ged` files and `rosewich.ged`, `Rosewich2.ged`, and `Rosewich3.ged` produce output when run with `--allow-recovery`; the CLI reports their specific warnings. `rosewich.ged` declares ASCII but contains Windows-1252 bytes; `Rosewich2/3.ged` declare ANSI. The fallback retained `München` in the tested `rosewich.ged` UTF-8 output. The strict default still rejects a duplicated-TRLR GRAMLICH file without creating an output.

Files blocked from accepted roundtrip output (diagnostic codes; categories may overlap):

| File | Recovery diagnostics |
| --- | --- |
| `Entry0001ff.New.Ged` | `GEDCOM_INVALID_UTF8`, `GEDCOM_UNRESOLVED_REFERENCE` |
| `Entry0001ff2.ged` | `GEDCOM_UNRESOLVED_REFERENCE` |
| `EntryGC0100ff.ged` | `GEDCOM_LEVEL_GAP`, `GEDCOM_MALFORMED_LINE` |
| `GedTestFile.ged` | Previously reported with `GEDCOM_DUPLICATE_XREF`; that result did not reproduce on rerun. Current CLI run completed successfully with no duplicate-XREF diagnostic. |
| `GRAMLICH_15.01.2017_00.ged` | `GEDCOM_TRAILER_COUNT` |
| `GRAMLICH_15.01.2017_01.ged` | `GEDCOM_TRAILER_COUNT` |
| `GRAMLICH_15.01.2017_02.ged` | `GEDCOM_TRAILER_COUNT` |
| `Muster_GEDCOM7_erweitert.ged` | Current check (2026-10-04 16:55:58, SHA-256 `3287384994B89E1886DFFF2486BC620F58AC7AE849580CE700343F578B586E9B`): strict roundtrip succeeds with no recovery diagnostics. A second roundtrip of the generated output also succeeds and is byte-identical to the first output. |
| `Obrigheim_FBU1.ged` | `GEDCOM_UNRESOLVED_REFERENCE` |
| `OsBM0406aff.ged` | `GEDCOM_INVALID_UTF8` |
| `OsbObr5939rr.New.Ged` | `GEDCOM_UNRESOLVED_REFERENCE` |
| `rosewich.ged` | Earlier strict run: `GEDCOM_INVALID_UTF8`, `GEDCOM_VERSION_UNSUPPORTED`; current decoder uses warned `GEDCOM_LEGACY_ENCODING` for its declared `CHAR ASCII` and Windows-1252 bytes. |
| `Rosewich2.ged` | Earlier strict run: `GEDCOM_INVALID_UTF8`, `GEDCOM_VERSION_MISSING`; current decoder uses warned `GEDCOM_LEGACY_ENCODING` for declared `CHAR ANSI`. |
| `Rosewich3.ged` | Earlier strict run: `GEDCOM_INVALID_UTF8`, `GEDCOM_VERSION_MISSING`; current decoder uses warned `GEDCOM_LEGACY_ENCODING` for declared `CHAR ANSI`. |

#### Current `Muster_GEDCOM7_erweitert.ged` re-analysis

The latest 1,665-line file is valid UTF-8, has a complete `HEAD` with `GEDC/VERS 7.0`, and contains no lines rejected by the parser's GEDCOM line syntax. The strict CLI run completed with no recovery or conversion warnings. Its UTF-8 output was then imported and written a second time: both runs succeeded, produced 69,747-byte output files, and the second output was byte-identical to the first. This establishes clean parser-level roundtrip stability for this fixture; it does not by itself prove complete standards semantics for every GEDCOM structure.

#### Duplicate XREF rerun details

Duplicate diagnostics now include the repeated XREF, both record tags, the first and duplicate source line numbers, and the first-definition-wins policy. An earlier local revision of `Muster_GEDCOM7_erweitert.ged` reported:

| XREF | First definition | Duplicate definition |
| --- | --- | --- |
| `@SYN1@` | `SOUR`, line 1734 | `SOUR`, line 1772 |
| `@SYN2@` | `SOUR`, line 1742 | `SOUR`, line 1780 |
| `@SYN3@` | `SOUR`, line 1750 | `SOUR`, line 1788 |
| `@SYN4@` | `SOUR`, line 1758 | `SOUR`, line 1796 |
| `@R1@` | `REPO`, line 1766 | `REPO`, line 1804 |

For each duplicate, the first record remains the target for references. On a later run, those duplicate reports did not reproduce against the current local file; it instead emitted the malformed-line/orphan-level issues in the table above. `GedTestFile.ged` also completes without a duplicate-XREF diagnostic.

## Acceptance criteria

- Detect GEDCOM versions 5.5, 5.5.1, and 7.0 from `HEAD/GEDC/VERS`.
- Read UTF-8 input and preserve Unicode text.
- Resolve `INDI`, `FAM`, `HUSB`, `WIFE`, `CHIL`, `FAMC`, and `FAMS` cross-references.
- Preserve `CONC` and `CONT` text content.
- Retain unparseable date values as text and report non-fatal diagnostics.
- Produce a DOCX file with imported families for every reference that contains families.

## Import behavior

### Supported syntax

- GEDCOM 5.5, 5.5.1, and 7.0 are detected from `HEAD/GEDC/VERS`.
- Input is decoded as UTF-8. A byte-order mark is ignored.
- Records are represented by their numeric level, optional cross-reference, tag, value, and children.
- `CONC` appends to the preceding value without a separator. `CONT` appends a line break and its value.

### OFB mapping

- `INDI` provides a person. `NAME`, `GIVN`, `SURN`, `SEX`, `OCCU`, `RELI`, `BIRT`, and `DEAT` are mapped when present.
- `FAM` provides a family. `HUSB`, `WIFE`, `CHIL`, and `MARR` links are resolved after all records have been read.
- `DATE` values are retained as source text. The importer does not reject GEDCOM date qualifiers.
- `PLAC` values are preserved as display names. Provider-specific place identifiers are optional.
- `BAPM` and `BURI` are mapped to baptism and burial date/place fields. Other event tags, notes, source citations, and private tags are not yet represented in the OFB model and are ignored without stopping the import.

### Diagnostics and failure boundaries

- A missing or unsupported GEDCOM version produces a diagnostic but is not fatal when the line structure is valid.
- Malformed individual lines, unresolved cross-references, duplicate identifiers, and illegal level jumps produce diagnostics and skip only the affected data when possible.
- The import fails when the stream is unreadable, no GEDCOM record structure can be parsed, or cancellation is requested.
- A document export fails when the imported data contains no families after non-fatal parse recovery.
- Duplicate `INDI` or `FAM` cross-references are logged and the first record is retained.
- Malformed GEDCOM lines are currently skipped; comprehensive diagnostics for malformed records and unresolved cross-references remain future work.

## DOCX backend and validation

- The Console references `CSharpBible\Data\DocumentUtils\Document.Docx\Document.Docx.csproj` and scans its provider assembly explicitly at startup.
- The inspected provider project references `Document.Base` and the NuGet package `DocX` 5.2.0; it does not directly reference other CSharpBible libraries.
- The DocX runtime banner states that the package is for non-commercial use only and requires a commercial license for commercial use. Resolve this licensing requirement before distributing or using the application commercially.
- ODT output is explicitly rejected by `UserDocumentFactoryImpl` because no ODT provider is registered.
- Numbered family headings use stable `family-<number>` bookmark targets. Person detail locations and index entries use stable bookmarks; index references navigate to family/person details and person details link back to person index entries. References to families not included in a filtered export remain plain text instead of broken links.
- Export order is title, preface, numbered families, indices, then legend.
- Paragraph and character formatting is carried by the provider-neutral `IDocAttributes` values in `IDocParagraph.DocAttributes` and `IDocSpan.DocAttributes`; standard keys include paragraph indentation and character font size/family, bold, italic, underline, strikeout, and color. The DOCX provider maps these attributes to WordprocessingML, including hyperlinks; templates and renderers should not depend on DOCX model classes for formatting.
- `--template gc` (default) writes an `Ehe:` line, partner details, child rows, and a `PN =` reference line per partner. GC follows the family-text navigation described in the bundled preface: a person's parent family is shown as `<family number>` and families later formed by that person as `[family number]`. Children are ordered by known birth date, then stable person reference; their later families are shown in square brackets. Resolved targets are internal hyperlinks to `family-<number>` bookmarks. A relationship outside the filtered export remains visible as its source family reference, but has no hyperlink. `--template ak` writes a compact `⚭` line, partner rows, a child count, and child rows. Both templates retain internal person anchors and person-index backlinks. Only values available in the imported GEDCOM model can be rendered: GEDCOM source citations/GC-specific `PN` data are not currently imported, so the PN row uses the person's GEDCOM XREF rather than a source citation.
- Additional family references in GC are role-specific. A parent's square-bracket list contains only other exported families where that person is a parent; it excludes the family currently being rendered. A child row's additional-family list uses the same parent-role meaning. The person index prints child-family references under `Kind:` and parent-family references under `Parent:`.
- In GC child rows, a child's surname is omitted when it matches the current family surname (case-insensitive after trimming); the given name is shown instead. Different surnames retain the full GC name. Parent rows, AK entries, and person-index names are unaffected.
- Family-reference lists are sorted numerically and deduplicated. Consecutive runs of three or more numbers are compacted as `first - last`; two consecutive numbers remain separate comma-delimited references. Gaps separate items with commas. In a compacted range only the first and last family numbers are hyperlinks; separators and the omitted intermediate numbers are not emitted. Any referenced family without an exported bookmark is shown as plain text.
- The grouping headings use the actual fused family-name groups from `FamilyNameGroupBuilder`. Local format references reviewed: `OsBM1386/1387.entTxt` (AK) and `OsBObr0006/0011.entTxt` (GC). No same-base-name `.ged` files were present under the inspected `C:/Projekte/Delphi/Data` or `D:/Projekte/Delphi/Data` trees, so the large `GRAMLICH_15.01.2017_00.ged` was used for the real-data export validation.
- A family's single surname is the most frequent non-placeholder child surname; ties are resolved deterministically by case-insensitive ordinal name, then ordinal casing. Childless families fall back to the first real parent surname. If no real child or parent surname exists, the family is retained under `Familien ohne Namen`. Placeholder values are names with no letters, values entirely enclosed in parentheses, `NN`, and `NA` (case-insensitive).
- Family names sort using German `de-DE` collation (`IgnoreCase | IgnoreNonSpace`) with ordinal tie-break; the Console orders groups and family names with `de-DE` ignore-case collation before assigning export family numbers.
- Name groups include only actual family surnames. A parent-surname → family-surname transition merges groups when observed for at least two families. Independently, normalized German surname strings with Levenshtein distance ≤2 are joined as acoustically close. Phonetic grouping can therefore merge names even without repeated transition evidence.
- Synthetic repository fixtures are stored under `OFBCreator.Core.Tests/TestData` and `OFBCreator.Console.Tests/TestData`; no local personal GEDCOM source files are copied into the repository.
- Regression coverage includes UTF-8 BOM/Unicode, malformed-line skipping, duplicate INDI XREF handling, fixture-based family relationships, early missing-input failure, and Console export composition/save calls.
- The generated synthetic demonstration document is `Artifacts/GedcomDocxSample.docx`; it was validated as an OpenXML ZIP containing `word/document.xml` and the expected synthetic title, names, and place.
- Large-file output `Artifacts/GRAMLICH_Familienbuch.docx` was regenerated from the local GEDCOM at `D:\Projekte\Delphi\Data\GenData\GRAMLICH_15.01.2017_00.ged` with GC format and family grouping. Current DOCX is 229,112 bytes with 12,952 paragraphs, 8,696 bookmarks, and 7,079 internal links; all internal targets resolve. OOXML confirms preface → family group → individual family name → family number → person index. (File size of source verified earlier as 994,935 bytes.)
- `dotnet build OFBCreator.slnx` succeeded for the configured target frameworks (116 warnings).
- `OFBCreator.Core.Tests` passed 122/122 tests and `OFBCreator.Abstractions.Tests` passed 39/39 tests on net8.0.
- After new regressions, `OFBCreator.Console.Tests` passed 5/5 on net8.0, including real DOCX OpenXML validation for both GC and AK layouts, hierarchical section order, and link targets. Full solution build succeeded. Broader Core and Abstractions test runs remain to be repeated after the hierarchy change.
- Local DOCX exports succeeded for all six listed GEDCOM references. Output files were written to the local temporary directory; no GEDCOM source data or generated documents were added to the repository.

## Privacy and fixture policy

External GEDCOM files are only local integration inputs. Repository fixtures must be synthetic or anonymized before inclusion.
