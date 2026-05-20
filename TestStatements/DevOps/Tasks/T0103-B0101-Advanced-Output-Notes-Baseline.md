# T0103A - Advanced Expected-Output Notes Baseline

## Parent Backlog Item

- B0101 - Document Expected Output for Learning-Critical Examples

## Summary

Record baseline guidance for the advanced example areas where the learning value depends on observable runtime behavior, generated artifacts, metadata output, or execution order.

## Baseline Notes by Area

### `TestStatements.Threading.Tasks`

Primary observation goal:

- Understand the difference between sequential work, asynchronous orchestration, awaited completion, and visibly interleaved progress.

Expected output guidance:

- The important learning result is the order pattern of work steps, not exact timestamps.
- Async breakfast-style examples should be documented in terms of which activities can overlap and which completion messages appear after awaited work completes.
- Multiple variants of the example should highlight differences in orchestration style rather than exact line-for-line output.

Caveats:

- Output order may vary slightly between runs depending on scheduling.
- Learners should compare phases and causality, not exact console ordering for every line.

### `TestStatements.Diagnostics`

Primary observation goal:

- Understand how diagnostics reveal runtime behavior, timing, and measurement concerns.

Expected output guidance:

- Stopwatch examples should document elapsed-time output as illustrative measurement, not as a stable number.
- Debug examples should describe where output is expected to appear and what kind of diagnostic signal the example is meant to surface.

Caveats:

- Exact timing values are non-deterministic.
- Debug output visibility may depend on execution environment or tooling.

### `TestStatements.Reflection`

Primary observation goal:

- Understand how assemblies, types, and members can be discovered and printed at runtime.

Expected output guidance:

- Documentation should focus on the kinds of metadata shown, such as assembly identity, types, or members.
- The learner should know what categories of information to expect even if ordering differs.

Caveats:

- Exact ordering of reflected members can vary.
- Output may differ slightly across frameworks if metadata surfaces are not identical.

### `TestStatements.Runtime.Loader`

Primary observation goal:

- Understand that code or assemblies can be compiled or loaded dynamically and then used at runtime.

Expected output guidance:

- Notes should describe the visible success path, such as a generated assembly, loaded type, or executed code path.
- The teaching point is the runtime effect, not only the presence of API calls in code.

Caveats:

- Behavior may depend on framework capabilities, file system state, or dynamic compilation dependencies.
- Errors and success paths should both be documented where they contribute to learning.

### `TestStatements.Runtime.Dynamic`

Primary observation goal:

- Understand that runtime-generated behavior can produce artifacts or outputs not fixed at compile time.

Expected output guidance:

- Documentation should describe what kind of generated result the learner should observe, such as created assemblies or runtime-generated behavior.
- The focus should remain on the difference between static and dynamic behavior.

Caveats:

- Framework-specific restrictions or differences should be noted if they affect the example result.
- Artifact location or naming may need descriptive wording instead of hard-coded expectations.

### `TestStatements.SystemNS.Text.Json`

Primary observation goal:

- Understand how object state becomes serialized JSON output, file content, or async write behavior.

Expected output guidance:

- Notes should describe the expected JSON structure shape, not only raw text.
- File-based examples should document whether the main result is console confirmation, created file content, or both.
- Async serialization examples should note whether completion confirmation is part of the visible learning outcome.

Caveats:

- Formatting may vary if serializer options change.
- File paths or environment-specific write locations should be documented in a robust way.

## Cross-Cutting Output Rules

- Prefer documenting observable patterns over exact transcripts when behavior is non-deterministic.
- Use exact sample output only where values and ordering are expected to remain stable.
- Explicitly separate deterministic content, illustrative content, and environment-sensitive content.
- Note cross-framework differences only when they materially affect the learning value.

## Follow-Up Candidates

- Add per-example output notes for the async breakfast variants.
- Add reflection output examples with annotated metadata categories.
- Add serialization output examples with structure-oriented commentary.
- Add loader and dynamic runtime notes that distinguish success-path observations from prerequisites.

## Done Criteria

- Advanced output areas are covered.
- Each area states what should be observed.
- Non-deterministic and environment-sensitive behavior is clearly marked.
