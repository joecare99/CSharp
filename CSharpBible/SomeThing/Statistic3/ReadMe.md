# Statistic3 – Obfuscated Mini VM / Bytecode Game

## Overview
`Statistic3` implements an obfuscated, table–driven virtual machine that runs a number?guessing game disguised as a statistical analyzer. Text output (status lines, prompts, result messages, labels) is reconstructed from integer tables instead of plain string literals.

## Key Data Structures
- Shard table `s`: List of `int[]`. Each array encodes a word: first element is a base offset, following elements are offset+character code. Decoding subtracts the first value from each subsequent value producing the word.
- Sentence table `w`: Each entry is an array of indices into `s` composing a multi–word sentence (prompt, status line, etc.). The final index is a newline shard.
- Label table `l`: Labels for statistical metrics (average, correlation) built the same way.
- Status selection array `x`: Indices into `w` for random status phrases.
- Program `p`: Integer sequence representing opcodes and operands (simple even pairing: opcode, argument).

## VM State Class
`class V` holds:
- `a` = secret number.
- `b` = last user input.
- `c` = program counter (index into `p`).
- `d` = boolean result of last comparison.
- `r` = `Random` instance.

## Instruction Set
Loop in `q(V v, int[] p, ...)` fetches opcode `o` and argument `a` (two consecutive ints). Switch cases:
| Op | Purpose | Behavior |
|----|---------|----------|
| 1  | Init secret | `v.a = Next(1..arg)` |
| 2  | Read input  | Parse line ? `v.b`, else `int.MinValue` |
| 3  | Compare EQ  | `v.d = (v.b == v.a)` |
| 4  | Compare LT  | `v.d = (v.b < v.a)` |
| 5  | Branch true | If `v.d` then `v.c = arg` |
| 6  | Print sentence | Decode `w[arg]` via shard table |
| 7  | Print label & value | Decode label; emit random number / double with formatting rules |
| 8  | Random status | Pick index from `x` and print that sentence |
| 9  | Progress bar | Animated textual bar with percentage (width = arg) |
| 10 | Chart bars | `arg` packs (min<<8)|max for count of random vertical bar rows |
| 11 | Unconditional jump | `v.c = arg` |
| 99 | Halt | Return from interpreter |

## Text Decoding (Function `y`)
```
for each shard index in sequence:
  shard = s[index]
  base = shard[0]
  for each codepoint value after first:
      write (char)(value - base)
```
This compact form hides literal strings.

## Program Flow (Simplified)
1. Opcode 1: Generate secret 1–20.
2. Loop: Prompt for input, read number.
3. Show random status, progress bar, statistical pseudo metrics, ASCII chart.
4. Compare equality; if hit ? success message and halt.
5. Otherwise compare less than; branch to “too low” or else “too high”.
6. Loop back and continue.

## Packed Argument (Op 10)
`(min<<8)|max` allows two 8?bit values in a single int for variability without extra opcodes.

## Obfuscation Techniques
- No string literals for user?visible text (all reconstructed).
- Minimal variable names (`a,b,c,d` in `V`).
- Unified integer array program reduces readability.

## Extending
Add new opcodes (e.g. arithmetic) by reserving numeric IDs and extending the switch. Introduce a proper header (magic number + version) for serialized programs.

## Running
```
dotnet run --project Statistic3
```
Enter guesses (1–20) until the success phrase appears.

## Educational Points
Demonstrates table?driven text reconstruction, simplistic VM dispatch, and a compact opcode design suitable for puzzle / obfuscation challenges.
