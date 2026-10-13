# Task T-831194 - Read Beckhoff SVDX trace data

## Status

In Progress - observed layout implemented; independent validation pending

## Goal

Read Beckhoff TwinCAT Scope `.svdx` recordings into the canonical trace data set and enable `TraceCsv2realCsv` to convert their measured channel values without loss or guessed values.

## Scope

- Implement a first reader for the directly verified SVDX layout represented by the forensic sample
- Decode recording metadata and sample blocks for supported SVDX versions and channel types
- Preserve channel names, data types, units, timing, missing samples, and parse diagnostics
- Integrate the reader through `IAnalyzableInputFilter` and register it in `TraceCsv2realCsv`
- Add synthetic or redistributable fixtures and tests for successful reads, malformed inputs, and conversion output

## Out of Scope

- Treating an XML-only metadata read as complete SVDX trace support
- Guessing sample values, timing, channel-to-block mappings, or binary field meanings
- Adding the user's `BreakEarly.svdx` to the repository
- Depending on a locally installed TwinCAT application without a documented availability and licensing strategy

## Evidence and Current Blocker

- Direct read-only verification of `C:\Projekte\2026\BreakEarly.svdx` confirmed a 567,020-byte file with SHA-256 `febe975d71cfcf8150d73dade5135d646aa01ddbb8f685ffc8ce4e22c96e3ea4`. A little-endian `UInt32` at file offset `0x00` is `431085` (`0x693ED`), the exact byte offset of `<?xml`; the `UInt32` at `0x08` is `135935` (`0x212FF`), the exact XML suffix length. The XML declaration is at `0x693ED`, `<ScopeProject` at `0x69415`, and the XML suffix ends at EOF.
- The header's `UInt32` at `0x10` is `11`. Starting at `0x14`, eleven 20-byte descriptors parse consistently as `{ UInt64 dataOffset, UInt64 dataLength, UInt32 fileHandle }`. All eleven IDs match the XML `FileHandle` values 1–11. Sorting these extents by offset gives exact, contiguous coverage of `[0xF0, 0x693ED)` with no gaps or overlaps and total length `430845` bytes. The header's descriptor order is not completely offset-sorted (handles 5 and 6 are reversed).
- Each extent's `FileHandle` maps directly to an XML acquisition in this sample. Handles 1–6 are `REAL64`, length `51723` bytes each; handles 7, 8, 10, and 11 are `BIT`, length `23289` bytes each; handle 9 is `INT16`, length `27351` bytes. All list `BaseSampleTime=100000` and `Oversample=0`.
- All eleven extents share an identical 143-byte prefix. At relative offset `0x123`, each block contains 224 index records with the same 160-ms timestamp grid. Their widths are 24 bytes for `REAL64` (FILETIME + two doubles), 10 bytes for `BIT` (FILETIME + two bytes), and 12 bytes for `INT16` (FILETIME + two signed 16-bit values). This grid spans 35.68 seconds and appears to be a coarse/index series, not the complete 10-ms sample stream.
- Immediately after the 15 leading type-width records and 224 index records, each extent contains 224 fixed-size detail chunks. The tails divide exactly: `REAL64` 45,696 bytes = 224 × 204; `BIT` 20,608 bytes = 224 × 92; `INT16` 24,192 bytes = 224 × 108. Each chunk size matches a 16-byte header, the first typed value, then 15 `{ UInt32 relativeTime, typed value }` entries: `16 + valueSize + 15 × (4 + valueSize)`. The chunk header begins with an 8-byte FILETIME and a 32-bit count. In this sample, 223 chunks have count 16 and one has count 7, yielding exactly 3,575 values per channel. The first 15 leading records are a separate preamble/summary region and are not part of the sample count. This interpretation is strongly supported by exact extent arithmetic, valid counts, monotonic timestamps, and 100,000-tick (=10-ms) sample steps.
- DataBlock 1's 3,575 decoded `REAL64` values range from `-1847.7713736791784` through `2050.3827937367391`; every absolute value is below 10,000 and all values are finite. Its first and last timestamps are `12:38:56.917` and `12:39:32.657` UTC. Their difference is exactly 357,400,000 FILETIME ticks = 35.74 seconds. Thus 35.74 seconds contain 3,574 intervals but 3,575 inclusive sample instants. The user expectation of 3,574 values is off by one if both endpoints are included.
- The same chunk arithmetic and count distribution were verified for all eleven extents in this sample. The 224 index records align one-to-one with the detail chunks: every index FILETIME equals its corresponding chunk base time (224/224 for each channel). For every REAL64, BIT, and INT16 channel, the index record's two values equal the minimum and maximum of the corresponding chunk's decoded samples, in either order (224/224 chunks per channel). This identifies the index pair as per-chunk extrema in this sample. The 224 extrema timestamps are spaced by 1,600,000 FILETIME ticks (160 ms); the decoded detail samples are spaced by 100,000 ticks (10 ms). The semantics of the 15 leading preamble records and any alternate format versions remain unestablished. These are strong reverse-engineering results but still require independent Beckhoff-export validation before production decoding.
- The earlier `svdx_analyse.md` contains incorrect hex offsets: decimal XML start `431085` is `0x693ED`, not `0x6929D`; the closing tag begins at `567005` (`0x8A6DD`), not `0x8A4AB`.
- Beckhoff documentation describes `.svdx` as a recording containing data and configuration and documents the Scope Export Tool as an export route. The cited sources do not publish a byte-level SVDX specification.
- The public `pytcs` project reads TwinCAT Scope CSV exports; its sample-data generation invokes `TC3ScopeExportTool.exe` rather than implementing an SVDX binary reader.
- The Beckhoff exporter was not found at `C:\TwinCAT\Functions\TF3300-Scope-Server\TC3ScopeExportTool.exe` in the current environment.
- The reader will initially target only this observed binary/XML layout and will reject structural mismatches rather than guess. Later clarification items are the 15 leading preamble records, format/version compatibility, unit/scaling semantics, independent comparison against Beckhoff-exported CSV, and additional representative files. These deferred items do not block reading/converting the known layout; the implementation must not claim broad or version-independent SVDX support.

## Required Inputs to Unblock

For later validation and broader format support, obtain at least one of the following:

1. A Beckhoff-supported .NET API or a redistributable command-line exporter, including its supported versions, licensing, and runtime requirements; or
2. The binary format specification; or
3. Multiple representative SVDX files paired with Beckhoff-generated CSV exports, covering all supported channel types, sampling patterns, and relevant format versions, plus enough data to validate exact timestamps and values.

The original `BreakEarly.svdx` remains outside the repository. Any new fixtures must be synthetic or explicitly redistributable/anonymized.

## Test Strategy

- Assert exact channel names, timestamps, sample values, data types, and units against Beckhoff-generated expected exports.
- Cover REAL64, INT16, BIT, missing/irregular samples, multiple blocks, and malformed/truncated files as supported by the eventual format evidence.
- Verify end-to-end `.svdx` to `.csv` conversion without silently accepting metadata-only results.
- Keep fixtures independent of the original `BreakEarly.svdx` file.

## Done Criteria

- The binary reader is based on a supported API/specification or has been validated against representative Beckhoff-exported reference pairs.
- Decoded canonical records preserve measured values and timing for documented supported SVDX versions and data types.
- `TraceCsv2realCsv` selects the reader for valid `.svdx` inputs and produces verified output.
- Relevant malformed-input and conversion tests pass, with no original user sample committed.
