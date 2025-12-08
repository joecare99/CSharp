# Statistic_ausf3 – Structured VM (Opcode Driven) Guessing Game

## Concept
Refactoring of earlier obfuscated versions into a clearer, enumerated opcode virtual machine while still avoiding direct string literals for user visible content. Text is rebuilt from numeric shards.

## Architecture
| Component | Description |
|-----------|-------------|
| `Op` enum | Defines opcodes (secret generation, I/O, comparisons, branching, rendering, halt). |
| `VM` class | Holds execution state: secret number, last input, program counter, comparison flag, RNG. |
| Shards | Array-of-int arrays encoding words (offset + encoded codepoints). |
| Sentences | Lists of shard indices forming full lines/messages. |
| Labels | Shard sequences for statistical metric prefixes. |
| Program | Integer opcode/arg pairs (compact bytecode) executed sequentially. |

## Opcode Summary
| Opcode | Action |
|--------|--------|
| `RndSecret` | Generate secret in 1..N. |
| `ReadNumber` | Read and parse user input. |
| `CmpEq` / `CmpLt` | Store comparison result into `Result`. |
| `BrTrue` | Conditional jump if latest comparison true. |
| `Jump` | Unconditional jump. |
| `PrintSentence` | Decode & write sentence. |
| `PrintLabelAndNumber` | Emit label then random metric (int/double). |
| `PrintRandomStatus` | Random status phrase (analysis heading). |
| `Progress` | Animated progress bar. |
| `Chart` | Random ASCII bar chart. |
| `Halt` | Stop execution. |

## Text Encoding (Shard Mechanism)
Shard layout: `[offset, encoded1, encoded2, ...]` where each `encodedX - offset` = actual character code. Sentences iterate shards and reconstruct characters. This reduces direct literal presence and enables minor obfuscation.

## Program Flow
1. Generate secret number.
2. Prompt for input.
3. Repeated loop:
   - Read guess.
   - Random status phrase + progress bar.
   - Print three labeled pseudo statistics.
   - ASCII chart.
   - Compare guess (eq / lt) branching to under/over/success paths.
4. On success print success message and halt.

## Chart & Progress Rendering
Uses low?level char writes (numeric codes) to avoid string literals: e.g. `'[' = 91`, `'#' = 35`, `'%' = 37`.

## Extensibility
- Add arithmetic or stack ops (e.g., `Push`, `Add`, `PrintNumber`).
- Serialize program to binary form for external loading.
- Introduce dynamic phrase pools or localization via alternative shard tables.

## Running
```
dotnet run --project Statistic_ausf3
```
Guess numbers (1–20) until success message appears.

## Educational Value
Illustrates transforming imperative control flow into a small VM with explicit opcodes, encouraging clear separation between data (program + shards) and interpreter logic.
