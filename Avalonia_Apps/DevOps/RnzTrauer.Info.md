# RNZ Trauer migration baseline — Done

## Current implementation plan

The component-first implementation plan is maintained in
[`RnzTrauer.NextSteps.md`](RnzTrauer.NextSteps.md). Each production component
gets its own test project and executable host before the components are composed
into the Avalonia application.

## Increment 2026-08-06 — Independent import component scaffold

### Completed

1. Added `RnzTrauer.Import` with `IHtmlImportPipeline`, `HtmlImportPipeline`,
   `HtmlImportReport`, and a default factory.
2. Added the dedicated `RnzTrauer.Import.Tests` MSTest project with CP1252
   and sixteen-column import coverage.
3. Added `RnzTrauer.Import.Host`, a local JSON-producing CLI host with
   `--html`, `--schema`, and `--output`.
4. Added all three projects to `RnzTrauer.slnx`.
5. Verified the host against the local Vieser I5 HTML fixture.

### Validation

- Import component tests: 2 passed, 0 failed.
- Existing Core tests: 43 passed, 0 failed.
- Complete solution build: successful, 0 errors.

### Next task

Create the next independent component, `RnzTrauer.Acquisition`, with a local
fixture host for HTTP/local HTML acquisition and archive-safe file handling.

## Increment 2026-08-06 — Independent acquisition component

### Completed

1. Added `RnzTrauer.Acquisition` with explicit file/HTTP(S) source handling.
2. Added bounded asynchronous reads, cancellation propagation, status-code
   validation, and media-type reporting.
3. Added archive-safe writes using a temporary file followed by atomic move.
4. Added `RnzTrauer.Acquisition.Tests` for local files, HTTP responses, archive
   output, and maximum-size rejection.
5. Added `RnzTrauer.Acquisition.Host` with `--source`, `--archive`,
   `--output`, and `--max-bytes`.
6. Added all projects to `RnzTrauer.slnx`.

### Validation

- Acquisition tests: 3 passed, 0 failed.
- Acquisition host local-fixture run: successful.
- Complete solution build: successful, 0 errors.

### Next task

Create the independent `RnzTrauer.Media` component for PDF/XML extraction and
image candidates, initially with a deterministic fake/process host rather than
any Windows PDF viewer automation.

## Increment 2026-08-06 — Independent media component

### Completed

1. Added `RnzTrauer.Media` with safe argument-list process execution,
   cancellation, timeout, stdout/stderr capture, and exit-code validation.
2. Added `PdfXmlExtractionService` with bounded XML output, explicit PDF/XML
   paths, and no shell command composition.
3. Added `PdfXmlDocumentParser` for the legacy `DOCUMENT/PAGE/TEXT/IMAGE`
   structure, including text lines and positioned image candidates.
4. Added `RnzTrauer.Media.Tests` with parser and fake-process extraction tests.
5. Added `RnzTrauer.Media.Host` for explicit PDF/tool/XML paths.
6. Added all projects to `RnzTrauer.slnx`.

### Validation

- Media tests: 2 passed, 0 failed.
- Media host help: successful.
- Complete solution build: successful, 0 errors.

### Deliberate boundary

The component does not automate PDF-XChange Viewer windows, keyboard input, or
clipboard access. The configured converter executable is an explicit process
adapter and must be supplied by the host.

### Next task

Create the independent `RnzTrauer.Persistence` characterization component for
repository contracts, SQL behavior, review queues, and transaction boundaries.

## Increment 2026-08-06 — Persistence SQL characterization component

### Completed

1. Added `SqlStatement` and `MySqlNoticeSql` to isolate parameterized query
   construction from database connections.
2. Covered all named review queues, including the previously unhandled
   `DuplicateCandidates` queue using `vNonSingletonName`.
3. Routed repository find, place-name, and link-candidate operations through the
   SQL builder.
4. Added `RnzTrauer.Persistence.MySql.Tests` with queue, parameterization, and
   link-candidate characterization.
5. Added `RnzTrauer.Persistence.Host`, a DB-free CLI that emits queue SQL as
   JSON for inspection and fixture generation.
6. Added all projects to `RnzTrauer.slnx`.

### Validation

- Persistence SQL tests: 10 passed, 0 failed.
- Persistence host DuplicateCandidates run: successful.
- Complete solution build: successful, 0 errors.

### Deliberate boundary

The host does not connect to a production database. Database integration,
transaction behavior, provider-neutral rendering, and row-mapping fixtures
remain the next persistence increments.

### Next task

Add database-test doubles for command/parameter capture and characterize
`SaveAsync`/`UpsertImportedAsync`, including affected-row behavior and
transaction boundaries.

## Increment 2026-08-06 — Vieser fixture expansion and import namespace cleanup

### Completed

1. Added portable Vieser HTML/schema/trace fixtures for I5, I12, I23, I134,
   and I12577 to `RnzTrauer.Import.Tests`.
2. Added parameterized golden characterization for schema advancement,
   `Name:`/`Ref:` emissions, trace headers, and relationship counts.
3. Renamed the extracted import implementation namespace from the temporary
   `RnzTrauer.Core.Services` compatibility namespace to
   `RnzTrauer.Import.Services`.
4. Updated Avalonia and Core test consumers without renaming the unrelated
   domain/parser services in `RnzTrauer.Core.Services`.

### Validation

- Import tests: 7 passed, 0 failed.
- Core tests: 43 passed, 0 failed.
- Complete solution build: successful, 0 errors.

## Increment 2026-08-06 — Persistence command characterization

### Completed

1. Added provider-free ADO.NET test doubles for connection, command,
   parameters, and disposal boundaries.
2. Characterized `SaveAsync` and `UpsertImportedAsync` as one command per
   operation with parameter binding and no explicit transaction.
3. Verified that `UpsertImportedAsync` currently returns `true` for successful
   command execution even when the provider reports zero affected rows.
4. Kept the deliberate transaction boundary: each operation owns one
   connection and one atomic SQL command; no multi-command unit of work exists
   yet.

### Validation

- Persistence tests: 13 passed, 0 failed.
- Existing MSBuild `MSB3539` intermediate-path warning remains unrelated.

### Next task

Evaluate provider-neutral statement rendering only after the remaining
Persistence contracts (reader lifecycle and error propagation) are covered.

## Increment 2026-08-06 — Persistence reader mapping characterization

### Completed

1. Added a `DataTableReader` fixture for the complete `Anzeigen` projection.
2. Characterized nullable dates, date-qualification parsing, nullable
   link/media fields, profile-image counts, and the default category fallback.
3. Kept row mapping independent of MySQL-specific runtime types.

### Validation

- Persistence tests: 14 passed, 0 failed.

### Next task

No provider-neutral renderer is introduced at this stage: the Persistence
component is explicitly MySQL-specific and depends on MySQL syntax and legacy
views. Revisit this boundary only when a second provider is a concrete
requirement.

## Increment 2026-08-06 — Persistence lifecycle and error characterization

### Completed

1. Verified that `FindAsync` disposes the data reader and closes its owned
   connection.
2. Verified that provider exceptions from reader creation are propagated
   unchanged while the connection is still disposed.
3. Evaluated `IDbStatementRenderer`; no additional abstraction is justified
   while the component targets only MySQL-specific SQL and views.

### Validation

- Persistence tests: 16 passed, 0 failed.

### Decision

Keep SQL construction in `RnzTrauer.Persistence.MySql` behind the existing
`MySqlNoticeSql` boundary. Introduce a renderer only with a concrete second
provider or a tested portability requirement.

### Next task

Begin the Places/Geocoding component with offline normalization fixtures and a
small independent host.

## Increment 2026-08-06 — Places normalization component scaffold

### Completed

1. Added `RnzTrauer.Places` with whitespace/Unicode normalization and
   deterministic known/unknown/empty place classification.
2. Added `RnzTrauer.Places.Tests` with offline normalization and resolution
   fixtures.
3. Added `RnzTrauer.Places.Host` with `--place`, optional `--known`, and
   `--output` JSON reporting.
4. Registered all three projects in `RnzTrauer.slnx`.

### Validation

- Places tests: 4 passed, 0 failed.
- Places host help and local normalization run: successful.
- Existing `MSB3539` intermediate-path warning remains unrelated.

### Deliberate boundary

This increment does not call GeoNames or any map service and does not invent
coordinates. External geocoding, rate limiting, caching, and ambiguity
handling remain explicit future adapters.

### Next task

Add an offline alias/ambiguity fixture contract, then add a geocoding adapter
interface without coupling the normalization core to HTTP.

## Increment 2026-08-06 — Places aliases and geocoding boundary

### Completed

1. Added alias resolution with canonical known-place matching.
2. Added explicit ambiguity results that preserve all valid candidates instead
   of selecting one heuristically.
3. Added `IGeocodingAdapter` and `GeocodingResult` as an asynchronous,
   cancellation-aware provider boundary with no HTTP implementation in Core.

### Validation

- Places tests: 6 passed, 0 failed.

### Next task

Add a deterministic offline geocoding adapter fixture and host mode, then
evaluate rate-limit/cache behavior behind that interface.

## Increment 2026-08-06 — Offline geocoding adapter

### Completed

1. Added `OfflineGeocodingAdapter` implementing `IGeocodingAdapter` with
   normalized, case-insensitive fixture lookup.
2. Added cancellation and hit/miss tests without network access.
3. Extended `RnzTrauer.Places.Host` with optional `--geocode <json>` output.
4. Verified a local JSON fixture produces deterministic coordinates and
   display-name output.

### Validation

- Places tests: 8 passed, 0 failed.
- Offline geocoding host fixture run: successful.

### Deliberate boundary

The adapter is intentionally fixture-backed. Rate limiting, caching,
provider-specific retries, and HTTP transport remain outside the core
component.

### Next task

Add cache and rate-limit policies around `IGeocodingAdapter`, using fake time
and a fake adapter before introducing any external provider.

## Increment 2026-08-06 — Geocoding cache and rate-limit policy

### Completed

1. Added `CachingGeocodingAdapter` with normalized case-insensitive cache
   keys and configurable cache lifetime.
2. Added deterministic `TimeProvider` injection for repeatable policy tests.
3. Added explicit `GeocodingRateLimitException` with a calculated retry
   duration for uncached requests made too soon.
4. Ensured cache hits do not consume the remote-request interval.

### Validation

- Places tests: 10 passed, 0 failed.

### Next task

Add a host-level policy report for cache hits, misses, and rate-limit
diagnostics before considering a real provider adapter.

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

## Increment 2026-08-06 — Places host policy report

### Completed

1. Added `GeocodingPolicyDiagnostics` with cache-hit, cache-miss,
   remote-request, rate-limit-rejection, and retry-after fields.
2. Counted policy events inside `CachingGeocodingAdapter` without coupling the
   component to a provider or UI.
3. Updated `RnzTrauer.Places.Host` to include policy diagnostics in its JSON
   output for offline geocoding runs.
4. Added assertions for the diagnostic counters to the cache and rate-limit
   characterization tests.

### Validation

- Places tests: 10 passed, 0 failed.
- Places host fixture run: successful.
- Places host build: successful, 0 errors.

### Next backlog step

Add offline fixtures for cache expiration, misses, and ambiguous aliases
before evaluating a real provider adapter.

## Increment 2026-08-06 — Places offline policy fixtures

### Completed

1. Added `RnzTrauer.Places.Tests/Fixtures/places-policy.json` containing
   known places, canonical/ambiguous aliases, and an offline geocoding result.
2. Added fixture-backed coverage for alias ambiguity and unknown geocoding
   misses without invented coordinates.
3. Added cache-expiration coverage proving an expired entry causes a new
   adapter request.

### Validation

- Places tests: 13 passed, 0 failed.

### Next backlog step

Decide whether coordinate persistence contracts are needed before evaluating a
real provider adapter.

## Increment 2026-08-06 — Provider-neutral place coordinate contract

### Completed

1. Added the immutable `PlaceCoordinate` value model.
2. Added `IPlaceCoordinateStore` with cancellation-aware read and write
   operations.
3. Added `InMemoryPlaceCoordinateStore` for offline hosts and deterministic
   tests; keys are normalized and coordinate ranges are validated.
4. Preserved provider independence: no MySQL columns or external geocoder
   assumptions were introduced.

### Validation

- Places tests: 15 passed, 0 failed.

### Next backlog step

Characterize how persisted coordinates should map to the legacy
`Anzeigen.Ort` model before adding a MySQL implementation.

## Increment 2026-08-06 — Legacy place-coordinate SQL characterization

### Completed

1. Confirmed that `Anzeigen.Ort` remains the textual notice-place field.
2. Confirmed that the legacy Places UI reads `Latitude`/`Longitude` from
   `Orte`, while the historical `CREATE TABLE Orte` statement does not define
   those columns.
3. Added `MySqlPlaceCoordinateSql` with normalized and parameterized read/update
   statements for deployments that provide the optional columns.
4. Added SQL characterization tests and kept the schema mismatch explicit
   instead of adding a silent fallback.

### Validation

- Persistence tests: 18 passed, 0 failed.
- Places tests: 15 passed, 0 failed.

### Next backlog step

Add a MySQL coordinate-store adapter only after defining the diagnostic
behavior for deployments missing `Latitude`/`Longitude`.

## Increment 2026-08-06 — MySQL place-coordinate store

### Completed

1. Added `MySqlPlaceCoordinateStore` implementing the provider-neutral
   `IPlaceCoordinateStore`.
2. Implemented normalized read/update operations against optional
   `Orte.Latitude` and `Orte.Longitude` columns.
3. Preserved explicit failure semantics: provider and missing-schema errors
   propagate unchanged; no silent fallback is introduced.
4. Added command, mapping, disposal, and parameter-binding characterization
   coverage.

### Validation

- Persistence tests: 20 passed, 0 failed.
- Places tests: 15 passed, 0 failed.

### Next backlog step

Expose the coordinate store through the offline/persistence host boundary and
decide whether a real geocoding provider is warranted.

## Increment 2026-08-06 — Offline coordinate-store host boundary

### Completed

1. Updated `RnzTrauer.Places.Host` to save successful offline geocoding
   results through `IPlaceCoordinateStore`.
2. Added `StoredCoordinate` to the JSON report after normalized
   read-back from the store.
3. Kept misses explicit: no coordinate record is created when the geocoder
   returns no result.

### Validation

- Places host build: successful, 0 errors.
- Offline host fixture run: successful.

### Next backlog step

Add persistence-host SQL inspection for coordinate reads/updates and an
explicit diagnostic for deployments missing the optional columns.

## Increment 2026-08-06 — Persistence host coordinate inspection

### Completed

1. Added `RnzTrauer.Persistence.Host --place <name>` for coordinate-read SQL
   inspection.
2. Added `--latitude <n> --longitude <n>` for coordinate-update SQL
   inspection with invariant parsing.
3. Included required optional columns and an explicit unverified-schema
   diagnostic in both JSON reports.
4. Registered the Places project reference in the Persistence host.

### Validation

- Persistence host build: successful, 0 errors.
- Persistence host read and write inspection runs: successful.

### Next backlog step

Add an explicit host/configuration status model for available, missing, and
unverified coordinate columns before any real provider work.

## Increment 2026-08-06 — Coordinate schema status model

### Completed

1. Added `CoordinateSchemaStatus` with `Available`, `Missing`, and
   `Unverified` states.
2. Added `CoordinateSchemaReport` with required columns, diagnostics, and a
   `CanPersist` decision.
3. Added Persistence-host option
   `--coordinate-schema <available|missing|unverified>`.
4. Included structured schema status in coordinate read/write inspection
   reports; default remains `Unverified`.

### Validation

- Places tests: 15 passed, 0 failed.
- Persistence host build: successful, 0 errors.
- Available and missing schema report runs: successful.

### Next backlog step

Wire the default `Unverified` state to an actual database schema capability
probe when database integration begins.

## Increment 2026-08-06 — Coordinate schema capability probe

### Completed

1. Added the provider-neutral `ICoordinateSchemaProbe` contract.
2. Implemented the probe in `MySqlPlaceCoordinateStore` using a bounded
   zero-row column query.
3. Successful probes return `Available`; provider `DbException` results are
   surfaced as explicit `Unverified` diagnostics.
4. Added SQL and schema-report characterization coverage.

### Validation

- Places tests: 16 passed, 0 failed.
- Persistence tests: 21 passed, 0 failed.

### Next backlog step

Expose the live probe through the Persistence host when a real database
connection/configuration is available.

## Increment 2026-08-06 — Live Persistence host schema probe

### Completed

1. Added `RnzTrauer.Persistence.Host --probe`.
2. Reused `MySqlDbConnectionFactory` and the existing `RNZ_DB_*`
   environment-variable convention without emitting passwords.
3. Returned structured `Available`/`Unverified` diagnostics and exit status 1
   when persistence capability is not verified.
4. Added the MySQL provider project reference only to the host composition
   boundary.

### Validation

- Persistence host restore/build: successful, 0 errors.
- Unreachable local fixture probe: structured `Unverified` output and expected
  non-success status.

### Next backlog step

Run the probe against a configured RNZ database and decide whether
missing-column errors need a distinct classification from other provider
failures.

## Increment 2026-08-06 — Missing-column schema classification

### Completed

1. Classified `DbException.ErrorCode == 1054` as confirmed missing coordinate
   columns.
2. Kept connection, permission, and other provider failures as `Unverified`
   rather than misreporting them as schema absence.
3. Added status coverage proving `Missing` disables persistence.

### Validation

- Persistence tests: 22 passed, 0 failed.

### Next backlog step

Run the probe against a configured RNZ database and confirm error-code
behavior for the deployed MySQL version.

Added a provider-free synthetic `DbException` regression for error code 1054;
the probe reports `Missing` and disables persistence without a live database.

### Validation

## Increment 2026-08-10 — Stable schema-probe diagnostics

### Completed

1. Added `DiagnosticCode` to `CoordinateSchemaReport`.
2. Standardized codes for available, missing, unverified, and probe-failure
   states without removing human-readable diagnostics.
3. Executed the live probe. No `RNZ_DB_*` configuration was available, so the
   host correctly used its documented local defaults and returned `Unverified`
   with exit code 1.

### Validation

- Places tests: 16 passed, 0 failed.
- Persistence tests: 23 passed, 0 failed.

## Increment 2026-08-10 — Explicit coordinate schema migration inspection

### Completed

1. Added a MySQL SQL builder for the coordinate-column migration.
2. Added `--coordinate-migration` to the Persistence host.
3. Marked the host operation as inspection-only; no database command is
   executed.
4. Added SQL characterization coverage.

### Validation

- Persistence tests: 24 passed, 0 failed.
- Migration inspection output: successful structured JSON.

### Next backlog step

Have the deployment owner review and execute the migration, or explicitly
choose the deployment mode that keeps coordinate persistence disabled.

## Increment 2026-08-10 — Coordinate save ViewModel coverage

### Completed

1. Added the positive save-path test for an `Available` coordinate schema.
2. Verified store invocation and the `coordinate.saved` diagnostic code.

### Validation

- Avalonia ViewModel tests: 5 passed, 0 failed.

### Next backlog step

Keep the migration execution separate from application startup and obtain a
deployment decision for missing coordinate columns.

## Increment 2026-08-11 — Full component regression

### Completed

1. Ran the complete `RnzTrauer.slnx` test suite.
2. Confirmed all component test projects execute together, including the new
   Avalonia ViewModel tests.

### Validation

- 100 tests passed, 0 failed.

### Next backlog step

Resolve the deployment decision for missing coordinate columns and then
repeat the live probe against a configured RNZ database.

## Decision 2026-08-11 — Missing coordinate columns

### Decision

The safe default is to keep coordinate persistence disabled when the schema
probe reports `Missing`. No application startup or UI action performs a schema
migration. The reviewed migration output must be executed by the deployment
process, followed by a probe that returns `Available`.

### Consequence

Deployments without the optional coordinate columns remain usable for notice
workflows; only coordinate writes stay unavailable and visibly diagnosed.

## Increment 2026-08-11 — Partial coordinate metadata transparency

### Completed

1. Made the current MySQL limitation explicit in the Avalonia save result.
2. Added the stable `coordinate.saved_partial_metadata` diagnostic code.
3. Updated the positive ViewModel test to verify the partial-persistence
   status.

### Validation

- Avalonia ViewModel tests: 5 passed, 0 failed.
- Avalonia build: successful, 0 errors.

### Next backlog step

Decide whether source and approximation require additional database columns or
should remain UI-only metadata.

The current UI now labels both fields as session-only, so the limitation is
visible before editing or saving.

The selection-linkage test also verifies that changing notices clears stale
coordinate values and metadata.

## Increment 2026-08-11 — Probe connection overrides

### Completed

1. Added CLI overrides for probe server, port, user, and database.
2. Kept password input restricted to `RNZ_DB_PASSWORD`.
3. Preserved structured status output and secret-free connection metadata.

### Validation

- Override probe returned structured `schema.probe_failed`/`Unverified` for
  the intentionally unreachable `127.0.0.1:3307/RNZ_Test` endpoint.

### Next backlog step

Run the probe with approved deployment parameters and a configured password
against a safe RNZ database.

Explicit invalid `--port` input now fails with exit code 2 instead of using a
silent default.

## Increment 2026-08-11 — Offline export host

### Completed

1. Added `RnzTrauer.Export.Host` and registered it in `RnzTrauer.slnx`.
2. Added a local JSON notice fixture.
3. Exposed TSV and GEDCOM export through `--format`.
4. Kept the host independent of Avalonia and database services.

### Validation

- TSV fixture export: successful.
- GEDCOM fixture export: successful.
- Temporary output files were removed after verification.

### Next backlog step

Characterize export destination policy, overwrite protection, and additional
Pascal-compatible golden fixtures.

The export host now blocks existing output files unless `--overwrite` is
specified. Both the blocked and explicitly allowed paths were verified with a
temporary TSV target.

An edge fixture with a filtered category and tab/newline content produced one
header plus one sanitized export row, confirming the existing Core behavior
through the standalone host.

Invalid JSON and missing input files now return concise diagnostics with
exit code 2; no unhandled exception is exposed by the host.
- No password was emitted by the probe.

### Next backlog step

Repeat the probe against a configured RNZ database and record the deployed
MySQL provider's actual `Available`, `Missing`, or `Unverified` result.

## Increment 2026-08-10 — Avalonia coordinate-service composition

### Completed

1. Added the `RnzTrauer.Places` project reference to the Avalonia application.
2. Registered one `MySqlPlaceCoordinateStore` instance as both
   `IPlaceCoordinateStore` and `ICoordinateSchemaProbe`.
3. Kept geocoding provider registration deferred; no external service is
   contacted by the composition root.

### Validation

- Avalonia build: successful, 0 errors.

### Next backlog step

Add a UI-facing ViewModel that invokes the schema probe asynchronously and
surfaces its structured status without blocking application startup.

## Increment 2026-08-10 — Asynchronous Avalonia schema status

### Completed

1. Injected `ICoordinateSchemaProbe` into `MainWindowViewModel`.
2. Started the initial probe asynchronously and added a manual retry command.
3. Added status, diagnostic-code, and diagnostic-text bindings to the Orte
   tab.
4. Kept failed or unavailable probes non-persistent and visibly diagnosable.

### Validation

- Avalonia build: successful, 0 errors.

### Next backlog step

Design the provider-neutral place selection and coordinate editing workflow;
keep write actions disabled until the schema report is `Available`.

## Increment 2026-08-10 — Provider-neutral coordinate editing

### Completed

1. Added provider-neutral coordinate fields and asynchronous Load/Save
   commands to `MainWindowViewModel`.
2. Connected the commands to `IPlaceCoordinateStore`.
3. Disabled and guarded Save until schema capability is `Available`.
4. Added stable diagnostics for required input, missing coordinates, and store
   failures.

### Validation

- Avalonia build: successful, 0 errors.

### Next backlog step

Connect the coordinate editor to the selected notice place and add dedicated
ViewModel tests for schema gating, validation, load, and save behavior.

## Increment 2026-08-10 — Selected-notice place linkage

### Completed

1. Synchronized `SelectedNotice.Place` with the coordinate editor place field.
2. Cleared coordinate values when the selected notice changes to avoid stale
   edits.
3. Added explicit diagnostics for a selected place and for missing place data.

### Validation

- Avalonia build: successful, 0 errors.

### Next backlog step

Create dedicated ViewModel tests covering selection linkage, schema gating,
input validation, coordinate loading, and coordinate saving.

## Increment 2026-08-10 — Avalonia ViewModel test coverage

### Completed

1. Added `RnzTrauer.Avalonia.Tests` as a dedicated MSTest project.
2. Covered selected-notice place synchronization.
3. Covered schema-gated save rejection and invalid coordinate input.
4. Covered loading stored coordinates into the editor.
5. Registered the test project in `RnzTrauer.slnx`.

### Validation

- Avalonia ViewModel tests: 4 passed, 0 failed.

### Next backlog step

Decide the operational response to missing `Orte.Latitude` and
`Orte.Longitude` before enabling production coordinate writes.

- Persistence tests: 23 passed, 0 failed.
