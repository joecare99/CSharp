# GEDCOM to DOCX Export

## Status

Core DOCX path and synthetic regression coverage are implemented. The family section is grouped as family-name group → individual family name → numbered family entries; family entries precede indices and are navigable through internal bookmarks/hyperlinks. GC and AK family-entry layouts can be selected with `--entry-format gc|ak` (GC default). Confirm DocX licensing before distribution.

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
- `--entry-format gc` (default) writes an `Ehe:` line, partner details, and a `PN =` reference line per partner. `--entry-format ak` writes a compact `⚭` line, partner rows, a child count, and child rows. Both layouts retain internal person anchors and person-index backlinks. Only values available in the imported GEDCOM model can be rendered: GEDCOM source citations/GC-specific `PN` data are not currently imported, so the PN row uses the person's GEDCOM XREF rather than a source citation.
- The grouping headings use the actual fused family-name groups from `FamilyNameGroupBuilder`. Local format references reviewed: `OsBM1386/1387.entTxt` (AK) and `OsBObr0006/0011.entTxt` (GC). No same-base-name `.ged` files were present under the inspected `C:/Projekte/Delphi/Data` or `D:/Projekte/Delphi/Data` trees, so the large `GRAMLICH_15.01.2017_00.ged` was used for the real-data export validation.
- A family's single surname is the most frequent non-empty child surname; ties are resolved deterministically by case-insensitive ordinal name, then ordinal casing. Childless families fall back to husband's surname, then wife's surname, then `Unbekannt`.
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
