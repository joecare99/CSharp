# Statistik_Ausf2 – Colored Obfuscated Phrase Statistical Guessing Game

## Overview
Evolution of the guessing game theme adding:
- Console coloring for status categories.
- Two complementary string reconstruction routines (`DecodeDiffs`, `DecodePackedDiffs`).
- Random ASCII bar chart after each attempt.
- Thematic analysis phrases cycling pseudo scientific terminology.

## String Reconstruction
1. `DecodePackedDiffs(ulong packed, int steps, int startChar, int bitWidth, int bias)`
   - Iteratively extracts `steps` slices of width `bitWidth` from `packed`.
   - Each slice becomes `diff = chunk - bias`.
   - Accumulate starting at `startChar` to build phrase (e.g., "Analyse...").
2. `DecodeDiffs(int startChar, int[] diffs)`
   - Adds each diff to a rolling code point; simple and transparent.

## Display Elements
| Element | Description |
|---------|-------------|
| Status Phrase | Randomly selected from decoded phrase list. |
| Progress Bar | 21 frames (0..width) with percentage (integer). |
| Metrics | Random integer (avg) and two rounded doubles (variance, correlation). |
| ASCII Chart | 6–9 vertical bars of `'|'` height 5–19. |

## Flow
1. Choose secret 1–20.
2. Prompt user for numbers within range.
3. For each attempt: show phrase, progress, metrics, chart, then evaluate guess.
4. Success prints green highlighted final message and exits.
5. Print final verification line.

## Coloring Strategy
- Title: Cyan.
- Status phrase: Yellow.
- Success: Green.
- Bars: DarkCyan.
- Prompts: DarkGray.

## Functions
| Function | Role |
|----------|------|
| `DecodeDiffs` | Linear diff expansion for phrases. |
| `DecodePackedDiffs` | Bit?packed diff decoding (space efficient). |
| `RenderProgressBar` | Fixed width progress animation with sleep delay. |
| `RenderAsciiChart` | Random vertical bar plot. |

## Extending
- Add classification of performance (attempt count tiers).
- Provide hint metrics (distance, hot/cold) between guesses.
- Localize by swapping diff arrays.
- Add command to reveal secret or quit.

## Run
```
dotnet run --project Statistik_Ausf2
```
Guess the integer until success confirmation appears.
