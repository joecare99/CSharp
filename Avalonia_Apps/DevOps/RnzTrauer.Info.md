# RNZ Trauer migration baseline — Done

## Delivered

- Created standalone `RnzTrauer` Avalonia solution with UI-agnostic `RnzTrauer.Core2` and DI-composed Avalonia desktop app.
- Reused `DbCoreAbstractions` and `DbProviderMySql` through `IDbConnectionFactory` and `IDBSettings`.
- Ported typed notice/filter/category model, parameterized persistence baseline, conservative German text parsing, review UI, and TSV/GEDCOM export.
- Documented source-level behavior and intentional migration boundary in `Delphi/docs/concepts/genealogy/rnz-anzeigen-application-analysis-and-migration.md`.

## Remaining backlog

1. Characterize legacy schemas/HTML/parser row outputs, MySQL view results, OCR text, TSV, and GEDCOM with fixtures.
2. Implement schema-driven HTML acquisition, download, PDF/XML, image, and place/geocoding components behind interfaces.
3. Add MSTest coverage for parser/export/repository SQL and secret-backed configuration UI.

## Validation

`dotnet build CSharp/Avalonia_Apps/RnzTrauer/RnzTrauer.slnx`

## Increment 2026-08-01 — visual form parity refinement

### Completed

1. Reworked `RnzTrauer.Avalonia` main window to mirror the legacy Lazarus form structure more closely:
   - top toolbar strip,
   - `TabControl` with `RNZ-Web`, `Anzeigen-DB`, `Orte`, and `Einstellungen`,
   - queue shortcut strip similar to legacy short filter buttons,
   - enlarged DB grid with legacy-like column set,
   - 3-way detail region (person fields, text/link panel, media panel),
   - dedicated bottom export/status footer.
2. Added ViewModel state for visual parity controls and read-only DB environment summary.
3. Added queue shortcut command wiring (`LoadQueueCommand`) so compact queue buttons immediately apply the selected queue.

## Increment 2026-08-01 — Core test baseline

### Completed

1. Added `RnzTrauer.Core.Tests` as a dedicated MSTest project referencing `RnzTrauer.Core2`.
2. Added parser fixtures for German birth/death dates, maiden names, places, ages, and quiet-burial category adjustment.
3. Added export fixtures for UTF-8 BOM TSV sanitization/filtering and GEDCOM header/person/event/note output.
4. Added the test project to `RnzTrauer.slnx`.

### Validation

- `dotnet test CSharp/Avalonia_Apps/RnzTrauer/RnzTrauer.Core.Tests/RnzTrauer.Core.Tests.csproj --no-restore`
  - 4 passed, 0 failed.
- `dotnet build CSharp/Avalonia_Apps/RnzTrauer/RnzTrauer.slnx --no-restore`
  - 0 errors.
  - Existing `DbProviderMySql` `CS9124` warnings remain on older target frameworks.

## Increment 2026-08-01 — Compact desktop typography

### Completed

1. Added application-level compact typography for the Avalonia desktop UI.
2. Reduced default control and text sizes to 13 px.
3. Reduced tab headers to 12 px and tightened their padding.
4. Tightened button padding without changing Fluent theme or theme-variant support.

## Increment 2026-08-01 — HTML normalization seam

### Completed

1. Added UI-agnostic `IHtmlTextNormalizer` and `HtmlTextNormalizer` to `RnzTrauer.Core2`.
2. Ported the legacy HTML-to-text behavior with entity decoding, tag removal, script/style suppression, and whitespace normalization.
3. Registered the normalizer through Avalonia DI for the upcoming schema-driven acquisition/import component.
4. Added two MSTest fixtures covering entity decoding and hidden markup.

### Next backlog step

Implement the schema line model and state-machine importer on top of the normalized acquisition text, preserving the legacy `+`, `[`, `j`, and `J` filter semantics.

## Knowledge-base policy

The authoritative Delphi/Lazarus knowledge base for this project is `Delphi/docs/**`, especially `docs/concepts/genealogy/rnz-anzeigen-application-analysis-and-migration.md`. Migration work must consult the cited Pascal units before changing behavior, and source discoveries must be added to that wiki page (or a linked page) before becoming implementation assumptions.

## Increment 2026-08-01 — Core2 schema filter

### Completed

1. Added `ISchemaFilter`, `SchemaFilter`, and `SchemaFilterEmission` to `RnzTrauer.Core2`.
2. Implemented the documented legacy `TBaseFilter` semantics for `+`, `[`, `jNN`, and `JNN` lines, case-insensitive prefix matching, zero-based jump destinations, and reset behavior.
3. Registered `ISchemaFilter` through Avalonia DI.
4. Added four MSTest fixtures covering emission, enable mode, jumps, and reset.

### Next backlog step

Build a schema-line importer that combines normalized HTML/parser tokens with `ISchemaFilter` emissions and maps emitted modes to the legacy import columns.

## Increment 2026-08-01 — Schema import accumulator

### Completed

1. Added `ISchemaImportAccumulator`, `SchemaImportAccumulator`, and `SchemaImportRow` to `RnzTrauer.Core2`.
2. Ported the documented callback mapping for modes `0`, `2`, and `3`, including `A`, `D`, and `N` behavior.
3. Registered the accumulator through Avalonia DI.
4. Added three MSTest fixtures for anchor paths, data-cell column advancement, and next-file extraction.

### Next backlog step

Compose HTML callback tokenization, `ISchemaFilter`, and `ISchemaImportAccumulator` into one complete import orchestration service.

### Validation

- Core tests: 13 passed, 0 failed.
- Full solution build: 0 warnings, 0 errors.

## Increment 2026-08-01 — HTML schema import orchestration

### Completed

1. Added `IHtmlSchemaImporter`, `HtmlSchemaImporter`, and `HtmlSchemaImportResult` to `RnzTrauer.Core2`.
2. Composed HTML tokenization, `ISchemaFilter`, and `ISchemaImportAccumulator`.
3. Preserved `TS`, `TE`, `S`, and comment callback vocabulary and returned completed/partial rows plus next-file events.
4. Registered the orchestration service through Avalonia DI.
5. Added two MSTest fixtures for anchor/text import and next-file extraction.

### Deliberate boundary

The tokenizer is a narrow migration seam, not yet exact Pascal parser parity. Tag-path bookkeeping, exact malformed-tag behavior, and encoding acquisition remain separate backlog items.

### Validation

- Core tests: 15 passed, 0 failed.
- Full solution build: 0 warnings, 0 errors.

## Increment 2026-08-01 — Incremental callback tokenizer

### Completed

1. Added `IHtmlCallbackTokenizer`, `HtmlCallbackTokenizer`, `HtmlCallback`, and `HtmlCallbackEvent`.
2. Added chunk buffering for split tags and comments, plus script callback handling.
3. Routed `HtmlSchemaImporter` through the callback tokenizer.
4. Preserved the Pascal distinction that tag modifiers are filtered but not directly forwarded to the row accumulator.
5. Changed filter/accumulator/importer registrations to transient lifetime so each import starts with isolated state.
6. Added two tokenizer characterization fixtures.

### Remaining boundary

Tag-path bookkeeping, exact Pascal malformed-tag behavior, encoding acquisition, and golden portal fixtures remain incomplete.

### Validation

- Core tests: 17 passed, 0 failed.
- Full solution build: 0 warnings, 0 errors.

### Backlog updates

1. Replace RNZ-Web placeholders with production components for acquisition/schema/output.
2. Replace placeholder media panel with real profile-image loading and selection.
3. Replace read-only settings view with secure editable configuration component.

## Increment 2026-08-01 — Byte-oriented encoding acquisition seam

### Completed

1. Added `IHtmlEncodingDecoder`/`HtmlEncodingDecoder` to `RnzTrauer.Core2`.
2. Implemented UTF-8/UTF-16 BOM handling, strict UTF-8 detection, and Windows-1252 fallback for legacy German portal bytes.
3. Added a raw-byte overload to `IHtmlSchemaImporter`, preserving the encoding boundary until import.
4. Registered the decoder through Avalonia DI.
5. Added MSTest coverage for UTF-8 BOM input and CP1252 umlaut fallback.

### Deliberate boundary

Exact Pascal `GuessEncoding` heuristics and malformed-byte golden fixtures remain open.

### Validation

- Core tests: 19 passed, 0 failed.
- Full solution build: 0 warnings, 0 errors.

## Increment 2026-08-02 — Tag-path bookkeeping

### Completed

1. Added uppercase backslash-delimited tag-path state to the incremental tokenizer.
2. Preserved the Pascal singleton exclusions for `P`, `BR`, `META`, `IMG`, and `!DOCTYPE`.
3. Added active `TagName`/`TagPath` metadata to callbacks and corrected `TM:` filter tokens to include the owning tag.
4. Added recovery for closing an outer tag with unbalanced descendants.
5. Added nested-path, modifier-owner, and recovery fixtures.

### Validation

- Core tests: 21 passed, 0 failed.
- Full solution build: 0 warnings, 0 errors.

## Increment 2026-08-02 — Malformed and partial markup fixtures

### Completed

1. Preserved incomplete tags across incremental feeds without emitting fabricated callbacks at completion.
2. Corrected attribute modifier tokenization so quoted values containing spaces remain one modifier.
3. Added characterization fixtures for unterminated tags and quoted attributes.
4. Documented the remaining deliberate boundary: this is still a narrow callback parser, not an HTML5 repair engine.

### Validation

- Core tests: 23 passed, 0 failed.

## Increment 2026-08-02 — Immutable schema/HTML golden fixture

### Completed

1. Added `RnzTrauer.Core.Tests/Fixtures/rnz-import-golden.json`.
2. Added a fixture-driven importer test comparing all sixteen positional columns.
3. Made new `SchemaImportRow` columns deterministic empty strings instead of null values.
4. Configured JSON fixtures to copy to the test output directory.

### Validation

- Core tests: 24 passed, 0 failed.

## Increment 2026-08-02 — Pascal AnzText1 golden parser fixture

### Completed

1. Ported `Delphi/Daten/data/GenData/AnzText1.txt` and `AnzText1_Erg.txt` into the C# test fixture corpus.
2. Added a fixture-driven parser test for Pascal birth, death, burial, and canonical place expectations.
3. Fixed full German month normalization (`Juli`, etc.) before abbreviation replacement.
4. Added whitespace-compacted OCR place matching so `Neckars tei nach den` resolves to configured `Neckarsteinach`.

### Validation

- Core tests: 25 passed, 0 failed.
- Full solution build: 0 warnings, 0 errors.

## Increment 2026-08-02 — Pascal AnzText2 golden parser fixture

### Completed

1. Ported `Delphi/Daten/data/GenData/AnzText2.txt` and `AnzText2_Erg.txt`.
2. Added a second fixture-driven parser test for dates, burial, OCR-split place, and maiden name.
3. Fixed CRLF-safe wrapped-hyphen normalization and `geb.`/case-insensitive maiden-name recognition.

### Validation

- Core tests: 26 passed, 0 failed.
- Full solution build: 0 warnings, 0 errors.

## Increment 2026-08-02 — Pascal AnzText3 golden parser fixture

### Completed

1. Ported `Delphi/Daten/data/GenData/AnzText3.txt` and `AnzText3_Erg.txt`.
2. Added a fixture-driven test for OCR-split `Sinsheim` place matching and `geb.` maiden-name extraction.

### Validation

- AnzText golden tests: 3 passed, 0 failed.

## Increment 2026-08-02 — Pascal AnzText4 golden parser fixture

### Completed

1. Ported `Delphi/Daten/data/GenData/AnzText4.txt` and `AnzText4_Erg.txt`.
2. Added age/place parser coverage for `im Alter von ... Jahren`.
3. Added fallback recognition for dates immediately preceding `verstorben ist`.

### Validation

- AnzText golden tests: 4 passed, 0 failed.

## Increment 2026-08-02 — Pascal test-harness inventory

### Documented

1. Recorded `fpcTestRNZAnzeigen.lpr` and its RNZ text/data-procedure registrations as characterization sources.
2. Recorded `fpcTestHtmlParse.lpr`, the Vieser `.Exp` callback fixtures, `tst_Filter`, `tst_h2gStep2`, and GEDCOM helper tests.
3. Identified `Delphi/Daten/data/GenData` as the shared fixture root for `AnzText1..16`, Vieser HTML/schema files, callback traces, plain-text expectations, and GEDCOM/event outputs.

### Next backlog step

Extract one portable C# golden fixture from the existing Vieser `.Exp` or `AnzText*_Erg.txt` references, preserving the Pascal expected output instead of inventing new expected values.

## Increment 2026-08-02 — Pascal AnzText5 golden parser fixture

### Completed

1. Ported `Delphi/Daten/data/GenData/AnzText5.txt` and `AnzText5_Erg.txt`.
2. Added coverage for date-before-`verstarb` wording, age extraction, and the expected absence of a configured place.
3. Extended the date-before-marker fallback to `verstarb`.

### Validation

- AnzText golden tests: 5 passed, 0 failed.

## Increment 2026-08-02 — Pascal AnzText6 golden parser fixture

### Completed

1. Ported `Delphi/Daten/data/GenData/AnzText6.txt` and `AnzText6_Erg.txt`.
2. Added wrapped burial-date and OCR place coverage.
3. Added case-insensitive maiden-name comparison for the Pascal mixed-case `BÖHM` expectation.

### Validation

- AnzText golden tests: 6 passed, 0 failed.

## Increment 2026-08-02 — Pascal AnzText7 golden parser fixture

### Completed

1. Ported `Delphi/Daten/data/GenData/AnzText7.txt` and `AnzText7_Erg.txt`.
2. Added multi-word maiden-name parsing for `von der Meden`.
3. Preserved lower-case name particles while applying title casing to OCR names.

### Validation

- AnzText golden tests: 7 passed, 0 failed.

## Increment 2026-08-02 — Pascal AnzText8 golden parser fixture

### Completed

1. Ported `Delphi/Daten/data/GenData/AnzText8.txt` and `AnzText8_Erg.txt`.
2. Added standard birth/death/burial/place golden coverage for an urn-burial notice.

### Validation

- AnzText golden tests: 8 passed, 0 failed.

## Increment 2026-08-02 — Pascal AnzText9 golden parser fixture

### Completed

1. Ported `Delphi/Daten/data/GenData/AnzText9.txt` and `AnzText9_Erg.txt`.
2. Added standard birth/death/burial/place coverage for the Sinsheim notice.

### Validation

- AnzText golden tests: 9 passed, 0 failed.

## Increment 2026-08-02 — Pascal AnzText10 partial-date fixture

### Completed

1. Ported `Delphi/Daten/data/GenData/AnzText10.txt` and `AnzText10_Erg.txt`.
2. Added coverage for age/place extraction while preserving missing birth/burial data.
3. Explicitly kept the year-less Pascal death date unset in the current `DateTime?` model instead of fabricating a year.

### Validation

- AnzText golden tests: 10 passed, 0 failed.

## Increment 2026-08-02 — Pascal AnzText11/12 fixture review

### Completed

1. Verified `AnzText11.txt` is byte-for-byte identical to `AnzText10.txt` and intentionally avoided redundant fixture duplication.
2. Ported distinct `AnzText12.txt` and `AnzText12_Erg.txt`.
3. Added OCR-split `Neckarsteinach` and abbreviated `Sep.` burial-date coverage.

### Validation

- AnzText golden tests: 11 distinct cases passed, 0 failed.

## Increment 2026-08-02 — Pascal AnzText13–16 fixture parity

### Completed

1. Ported distinct `AnzText13.txt` through `AnzText16_Erg.txt`; `AnzText11` remains intentionally deduplicated.
2. Added negative-fact coverage for thanks notices, unmarked birth/death date pairs, burial wording, date-before-`geschlossen`, and `seinem 81. Lebensjahr`.
3. Kept year-less dates unset while allowing unmarked full date pairs to populate birth/death facts.

### Validation

- Notice parser tests: 17 passed, 0 failed.

## Increment 2026-08-02 — First Vieser callback trace reference

### Completed

1. Ported the line-level content of Pascal `Delphi/Daten/data/GenData/vieser/I5.Exp` to `RnzTrauer.Core.Tests/Fixtures/Pascal/Vieser_I5.Exp`; fixture line endings and encoding are normalized.
2. Preserved the legacy callback categories, schema jump lines, and escaped newline representation as a portable reference for the next importer-parity increment.
3. Added an executable fixture check proving the Core2 tokenizer/filter can advance the Vieser I5 schema through its `Name:` and `Ref:` emissions; Vieser descriptive callbacks remain intentionally separate from RNZ positional row accumulation.
4. Fixed `!DOCTYPE` tokenization: the singleton was documented but rejected by the tag-name pattern, preventing the Vieser schema from advancing past its first line.
5. Added a direct tokenizer regression test for `<!DOCTYPE ...>`.

## Increment 2026-08-02 — Vieser I12 trace coverage

### Completed

1. Ported `vieser/I12.Exp` as a second callback-trace golden fixture.
2. Added assertions covering burial, marriage, and six child relationship sections.

## Increment 2026-08-02 — Provider-neutral Core dependency

### Completed

1. Kept `RnzTrauer.Core2` dependent only on `CSharp/Gen_FreeWin/DbCoreAbstractions/DbCoreAbstractions.csproj`.
2. Moved the concrete `DbProviderMySql.csproj` reference to the Avalonia composition project, where `MySqlDbConnectionFactory` is registered.
3. Preserved the provider-neutral `IDbConnectionFactory`/`IDBSettings` boundary in Core.
4. Recorded the remaining SQL portability boundary: `MySqlNoticeRepository` still emits MySQL-specific SQL directly and does not yet consume `IDbStatementRenderer`.
5. Moved `MySqlNoticeRepository` out of Core and into the provider-specific `RnzTrauer.Persistence.MySql` project, keeping the Avalonia shell as composition only.

### Follow-up backlog

- Replace hardcoded MySQL SQL with provider-neutral statement construction through `IDbStatementRenderer`, or keep it isolated behind a dedicated provider-specific adapter project before adding another database provider.

### Validation

- Full Core tests: 41 passed, 0 failed.
- Full solution build: 0 warnings, 0 errors.

## Increment 2026-08-02 — Reusable importer state reset

### Completed

1. Added an explicit `ISchemaImportAccumulator.Reset()` contract.
2. Reset rows, media events, partial row state, and compute mode at the start of every import.
3. Added repeated-import coverage to prevent state leakage when an importer instance is reused.

### Validation

- Full Core tests: 43 passed, 0 failed.
- Full solution build: 0 warnings, 0 errors.
