# Statistik2 – Obfuscated String Reconstruction Guessing Game

## Overview
Enhanced variant of the basic guessing game with:
- Obfuscated dynamic string reconstruction (two schemes: packed 7?bit diffs and incremental diffs).
- Decorative pseudo statistical metrics and ASCII bar chart lines.
- German interface text disguised as an analyzer.

## String Obfuscation Methods
1. `D(ulong k, int n, int s, int w, int b)` (Packed diffs):
   - Treats the 64?bit constant `k` as a sequence of `n` chunks of width `w` bits (here 7 bits).
   - Each chunk yields a value `c`; difference = `c - b` (bias removal).
   - Start with ASCII code `s`; cumulatively add differences to reconstruct characters.
2. `E(int s, int[] d)` (Incremental diffs):
   - Start with initial code `s`.
   - For each delta in `d`, accumulate and append resulting character.

Both return strings ending in ellipsis (`...`) to mimic ongoing analysis.

## Runtime Flow
1. Generate secret number `g` (1–20).
2. Build phrase list `Phr` containing several analysis labels.
3. Loop until correct guess:
   - Prompt and read input.
   - Display random status phrase.
   - Render progress bar (20 steps, ~28 ms delay each).
   - Emit pseudo metrics: Durchschnittswert, Varianz, Korrelation.
   - Render random number (6–9) of vertical bars (`'|'`) with variable height (5–19).
   - Parse input and compare:
     - Equal: success message and break.
     - Less / greater: respective message.
     - Non?numeric: error notice.
4. Final verification line printed after loop.

## Important Variables
- `g`: secret target integer.
- `Phr`: list of decoded status phrases.
- `e`: loop termination flag.

## Functions
| Function | Role |
|----------|------|
| `E` | Diff expansion for moderately sized strings. |
| `D` | Bit?packed diff decoder for compact embedding of short strings. |

## Extending
- Add timing metrics or attempt counters.
- Introduce difficulty scaling (range growth after success).
- Internationalization: store diff sets for multiple locales.

## Running
```
dotnet run --project Statistik2
```
Provide numeric guesses (1–20) until the optimized solution message appears.
