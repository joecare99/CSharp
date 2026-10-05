# AhnWin52Backup

Standalone .NET console application and core library for working with AhnWin `.hej` backups.

## Current functionality

`backup` reads the five mapped Paradox tables, including encrypted record blocks and memo text, and writes a complete `.hej` file. It creates the output through a temporary file, validates it, refuses to overwrite an existing target, and never writes to the source database. `inspect` validates an existing HEJ file; `inspect-db` reports a Paradox 5/7 table schema and record count without displaying record contents:

```powershell
dotnet run --project AhnWin52Backup.Cli --framework net10.0 -- backup C:\path\to\database C:\path\to\backup.hej
dotnet run --project AhnWin52Backup.Cli --framework net10.0 -- inspect C:\path\to\backup.hej
dotnet run --project AhnWin52Backup.Cli --framework net10.0 -- inspect-db C:\path\to\AWD.DB
dotnet run --project AhnWin52Backup.Cli --framework net10.0 -- --set-passwd
```

`--set-passwd` prompts for the Paradox password twice without echoing it, then stores it as a Windows Generic Credential for the current Windows user. Do not pass the password as a command-line argument. The credential is machine-persistent but remains protected by the Windows user profile. The current Paradox reader does not yet retrieve or use this credential; wiring retrieval into a future encrypted table writer remains part of the blocked restore work.

The reader accepts the legacy individual-record variant with one omitted trailing field by padding it. Marriage records use the verified 22-field schema. Legacy control bytes other than the field separator and embedded-newline marker are discarded in the same way as the existing reader; each occurrence type is reported as a compatibility warning.

Paradox-to-HEJ export is implemented. Full HEJ-to-Paradox restore remains **blocked pending AhnWin compatibility testing**. An experimental C# writer can insert a single data block, update its primary `.PX` index, and generate all 17 supported `.XG`/`.YG` pairs for the five mapped tables. Its indexed blocks have been compared against AhnWin-generated files, but the latest C#-written candidate has not yet been opened by AhnWin. This is experimental code, not a supported restore workflow. The source mapping has been identified against the original data-module definitions and the local Paradox table headers:

| HEJ section | Paradox table | Fields written to HEJ |
| --- | --- | --- |
| Individuals | `AWD.DB` | 51 of 52 fields; the binary `Bild` field is not part of HEJ |
| Marriages | `MRG.DB` | 22 of 23 fields; the `Numr` auto-increment field is omitted |
| `adop` | `adp.DB` | All 3 fields |
| `ortv` | `LOC.DB` | All 13 fields |
| `quellv` | `sour2.DB` | 11 of 12 fields; the `N` auto-increment field is omitted |

Paradox record blocks and the `AWD.MB`/`sour2.MB` memo data are encrypted in the supplied test database. The C# adapter ports the necessary pxlib 0.6.8 decryption and memo-reading behavior, validates the five table schemas, and exports all HEJ sections. No current CLI command modifies a database. An opt-in integration test exercises experimental single-block `.DB`/`.PX` insertion and `.XG`/`.YG` secondary-index generation; memo writes and multi-block indexes remain unsupported.

### Isolated empty-template insertion experiment

The user supplied `C:\ProgramData\AHNENWIN_Empty2` as a blank AhnWin database. The five mapped tables and 17 secondary-index pairs have zero records. An opt-in integration test copied the template and wrote synthetic records only to `C:\ProgramData\AHNENWIN_Empty2_pxlib_trial3`: two people linked by one marriage, one adoption, one place, and one source. The test re-read the encrypted records and verified their values; the normal backup/inspect workflow also exported and validated the resulting HEJ sections. The 17 `.XG`/`.YG` sidecars remained byte-identical to the blank template. The untouched template and `C:\ProgramData\LiveInstance` were not modified.

AhnWin reported “Index nicht mehr gültig” for indexes `num` and `tit`; after the dataset closed, changing person records produced “Datei Beschädigt, aber nicht der Vorspann, [path]\AWD.DB Kein Datensatzwechsel.” The user had to force-close AhnWin. The secondary `.XG`/`.YG` sidecars had zero records while the DB tables contained 1–2 records, so header-only sidecars are insufficient. A separate header comparison also found that populated `.PX` files have nonzero/change-varying byte `0x2C`, while the experimental writer had left this primary-index change marker untouched. The pxlib format notes describe the byte as advancing for primary-index changes; the writer now advances it by the number of inserted primary keys, but that correction has not yet been validated in AhnWin. Trial3 was later observed locked and left untouched. Do not use it as a database or claim restore support; preserve it, the blank template, and `C:\ProgramData\LiveInstance`.

A follow-up candidate at `C:\ProgramData\AHNENWIN_Empty2_pxlib_trial4` advanced `.PX` byte `0x2C`, but AhnWin still reported “Index nicht mehr gültig: [PAF]\AWD.db”, followed by the closed-dataset error, and could not be closed normally. The user clarified that the message named only the table path `[path]\AWD.DB`, not a particular index file. Trial5 encoded `.PX` references as pxlib does (big-endian Paradox short values with the positive-value flag), but AhnWin reported the same table-level error. Trial8 added all 17 populated `.XG`/`.YG` pairs and AhnWin still showed no change; the error therefore is not explained solely by empty secondary sidecars.

The pxlib `pxformat.txt` and `paradox4.txt` format descriptions confirm the user's hypothesis: `.XG` is a DB-like secondary-index record file, with one record per source-table record; `.YG` is the primary index for its `.XG` file and follows the `.PX` index structure. `.XG` records contain the secondary key fields, primary-key fields where needed, and a final block hint. The supplied live headers match this layout: the five mapped tables' `.XG` counts match their row counts (100 AWD, 50 MRG, 5 adp, 5 LOC, 110 sour2), and corresponding `.YG` counts match the number of XG data blocks (including 8 entries for `sour2.XG0`).

The candidate `C:\ProgramData\AHNENWIN_Empty2_pxlib_trial16` was opened in AhnWin after the sidecar comparison, but the user reports the same index error as in earlier trials. Therefore, matching secondary-index blocks alone is insufficient to make this database acceptable to AhnWin. AhnWin was subsequently closed: the candidate files are unlocked, and all five mapped tables retain their expected record counts. The mapped-file modification timestamps advanced during the attempt, so preserve trial16 as post-attempt evidence; do not re-test or treat it as pristine C# output.

Before that attempt, a read-only decrypted-block comparison found exact matches in 22 of 34 supported sidecar files: every AWD and adp XG/YG block, plus the LOC pair. This includes all eight AWD indexes, including the birthplace index. The LOC comparison exposed a position-sensitive rule: AhnWin uppercases the first `Ort` index key but preserves the original case in the duplicate primary-key field. The writer now encodes duplicate field names by position to reproduce that representation. The comparison after the failed open still has the same 22/34 matches; AhnWin did not rebuild the differing MRG or sour2 secondary blocks.

The 12 remaining blocks are explained by data generated by AhnWin: all five MRG index pairs differ only in the auto-increment field `Numr` (`2833280` in the native import versus `1` in the candidate), and `sour2` has two native records versus one candidate record because AhnWin creates a second source titled `ProbeSrc` for the reference. The native LOC record also leaves `Land` empty although the HEJ probe supplied `Testland`; this data-level difference remains to be assessed separately. The writer remains limited to one data block per generated index file and is not a production restore path. The current full test run passes 17 tests and skips 1 on each of `net8.0`, `net9.0`, and `net10.0`; the opt-in trial16 generation test also passes on `net10.0`.

The first reverse-reference import used `C:\ProgramData\AHWREF`, a separate copy of the 107-file blank template. AhnWin restored the people, marriage, and adoption, but left `LOC.DB` and `sour2.DB` empty because the standalone place/source rows were not referenced. For `C:\ProgramData\AHWREF2`, the linked `PROBE2.HEJ` import produced 2 AWD, 1 MRG, 1 adp, 1 LOC, and 2 sour2 records (the second source was generated automatically). All inspected reference files are closed and have only been read by the comparison. The original template and `C:\ProgramData\LiveInstance` remain untouched.

### AhnWin live restore smoke test (2026-10-05)

The synthetic `Demo-100-Personen.hej` was restored successfully by the user's AhnWin instance into an isolated, complete database under `C:\ProgramData\LiveInstance`. The database files were copied there because AhnWin had problems with the longer test-directory path. AhnWin created 100 people, 50 marriages, 5 adoptions, 5 places, and source records.

The user notes that AhnWin automatically adds source-table entries for source references in person and marriage records, and may also add place-table entries. Therefore, the 110 source records in `Demo-100-Personen_2.hej` versus 5 explicitly listed in the input are not by themselves evidence of a restore defect; lookup tables may be expanded from references during restore. The returned place count is 5. Comparing person records still shows changes in `Text`, `AKA`, `Index`, `AdrPlaceAdd`, and `Free1` for all 100 people, and in `Free3` for 36 people; those field differences need separate analysis before treating this as a complete semantic round-trip. The restore implementation in this tool remains blocked; no database was written by this tool.

The user also observed that some tables in the copied live directory still have 2014 modification timestamps and were not touched by AhnWin. In the inspected directory, the mapped `AWD.DB`, `MRG.DB`, `adp.DB`, `LOC.db`, and `sour2.DB` files have a 2026-10-05 01:37 modification time, while ancillary tables such as `ahntab.DB`, `alfls.db`, `ftab.DB`, `gede.DB`, `lc.DB`, `loc2.db`, and `VORF.DB` retain 2014 timestamps. This is useful evidence about which files changed during the test, but timestamps alone do not establish which ancillary tables AhnWin reads or requires.

The user subsequently exercised all available AhnWin functions with the restored synthetic data and reports that the application works correctly with this dataset. The password is known to the user; its value is intentionally not recorded here. This completes the AhnWin live-application compatibility check for this test dataset, but does not test a Paradox restore generated by this tool. The user prefers encrypted output using that password. The intended secret-storage approach is Windows Credential Manager, so the password will not be stored in source, project configuration, or planning files. The application can retrieve it when running under the same Windows user account that owns the credential.

### Live Paradox index inventory

The five mapped tables in `C:\ProgramData\LiveInstance` were opened read-only and all `.PX`, `.XG`, and `.YG` headers were decoded. Every table has its primary `.PX` plus all of its paired `.XG<n>`/`.YG<n>` secondary-index files. The `.XG` entry count matches the database row count, and the secondary index field names/types/lengths match the underlying table schema. This confirms the index inventory for the tested AhnWin database:

| Table | Rows | Primary `.PX` key | Secondary `.XG<n>` keys |
| --- | ---: | --- | --- |
| `AWD.DB` | 100 | `Nummer` | 0: `Mutter, Gebjahr, Gebmonat, Gebtag, Taufjahr, Taufmonat, Tauftag, Nummer`; 1: `Vater, Gebjahr, Gebmonat, Gebtag, Taufjahr, Taufmonat, Tauftag, Nummer`; 2: `Name, Vornamen, Gebjahr, Indj, Indm, Indt, Nummer`; 3: `Name, Vornamen, Gebjahr, Gebmonat, Gebtag, Taufjahr, Taufmonat, Tauftag, Nummer`; 4: `Indj, Indm, Indt, Name, Vornamen, Nummer`; 5: `Mutter, Indj, Indm, Indt, Nummer`; 6: `Vater, Indj, Indm, Indt, Nummer`; 7: `Gebort, Nummer` |
| `MRG.DB` | 50 | `Numr` | 0: `Nummer, Numr`; 1: `Epnum, Indj, Indm, Numr`; 2: `Nummer, Indj, Indm, Numr`; 3: `Epnum, Numr`; 4: `Nummer, Hjahr, Hmonat, Htag, Sajahr, Samonat, Satag, Numr` |
| `adp.DB` | 5 | `Nummer` | 0: `Am, Nummer`; 1: `Av, Nummer` |
| `LOC.db` | 5 | `Ort` | 0: `Ort, Ort` |
| `sour2.DB` | 110 | `N` | 0: `Titel, N` |

The eight `AWD` keys match the field sequences in `AhnWin52\Table9IndexDefinitions.pas`. The legacy DFM's `AWD` lookups by `Nummer` and `Vater` are covered by the primary key and the `Vater`-leading secondary indexes; `MRG`'s named `num` lookup is covered by `.XG0` (`Nummer, Numr`). The duplicate `Ort, Ort` key in `LOC.XG0` is present in both index headers and works in the tested database, though why AhnWin keeps this redundant-looking secondary index alongside its primary key is not yet established.

This removes uncertainty about the *required index definitions* for these five tables, but not how to create them. The user clarified that the required table/index files must exist for AhnWin to open the database, but may initially contain only valid headers and no records. pxlib can create generic `.XG`/`.YG` files, so header-only stubs may be sufficient for this initial state. However, source inspection shows `PX_insert_record`/`PX_put_record` updates the `.DB` data blocks and its internal primary-block map; it does not populate or rebuild secondary `.XG`/`.YG` indexes. Therefore, header-only stubs simplify file creation but do not by themselves provide a complete index solution when records are inserted through pxlib.

The user provided `C:\ProgramData\AHNENWIN_Empty2` as the blank AhnWin template. The five HEJ-mapped tables all report zero active records, and all 17 `.XG`/`.YG` sidecar pairs declare zero index records. A separate working copy at `C:\ProgramData\AHNENWIN_Empty2_pxlib_test` was created and verified byte-for-byte by SHA-256 across all 107 files. The source template is preserved. The project currently contains only a Paradox reader, not a pxlib insert writer; this environment also has no native pxlib binary or C/C++ build tool available. The next step is to implement/port a minimal test-only insert path, write only to the working copy, then have the user test AhnWin open/use and secondary-index behavior. No records have been inserted yet; `C:\ProgramData\LiveInstance` remains untouched.

Before a restore writer is started, the following must be resolved using synthetic tables and AhnWin/BDE format evidence:

- Implement a supported way to generate all 17 required secondary index pairs from the now-verified key definitions.
- Use Windows Credential Manager for the user's password when implementing encrypted tables and memo sidecars. The `--set-passwd` command is implemented; retrieval and use by a restore writer remains future work. The password itself must never be stored in source, project configuration, or planning files.
- Follow the accepted HEJ information-loss policy: `Bild` is restored empty; `Numr` and `N` may be regenerated and are not original identifiers.
- Allow only explicitly classified, known legacy HEJ deviations. Unknown warnings, non-representable values, and out-of-range values must fail before writing.
- Prove all five synthetic tables and memo sidecars round-trip.
- Verify AhnWin compatibility in an isolated synthetic database directory before claiming restore support.

See `C:\Projekte\CSharp\DevOps\Tasks\RESPONSE-COPILOT-2-PARADOX-RESTORE.md` for the pxlib write-path assessment and `C:\Projekte\CSharp\DevOps\Tasks\TASK-AHWB-001.2-restore-paradox.md` for the blocked restore work item.

## Build and tests

The projects import the shared `..\WinAhnen.props` configuration and target `net8.0`, `net9.0`, and `net10.0` when the installed SDK supports each target:

```powershell
dotnet test AhnenWin52Backup.slnx
```

Tests use synthetic records and do not copy real genealogy data into the repository.

## HEJ wire format confirmed so far

- Text is encoded as Windows-1252.
- Lines and records end with CRLF.
- Fields are separated by byte `0x0F`.
- Embedded line breaks use byte `0x10`.
- Record sections occur in order: individuals, `mrg`, `adop`, `ortv`, `quellv`.
- Individual rows in legacy files may omit one trailing field; the reader pads it as empty.
- Marriage rows contain 22 fields.

`ParadoxBlockCipher` is derived from pxlib 0.6.8. Its GPL-2.0 license is included in `LICENSES\pxlib-COPYING`; distribution of the combined application must comply with that license.
