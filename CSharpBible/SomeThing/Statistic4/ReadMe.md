# Statistic4 – MicroVM Demo (C# / .NET 9)

## Purpose
`Statistic4` showcases a tiny stackless virtual machine ("MicroVM") executing a handcrafted integer bytecode that simulates a disguised number?guessing game with pseudo statistical output.

## High?Level Idea
Instead of directly writing the control flow, the program:
1. Builds an `int[]` instruction stream (`prog`).
2. Decodes each element as an *extended opcode* with packed flag bits.
3. Interprets the program producing prompts, random metrics and result evaluation (greater / smaller / hit).

## Core Components
| Concept | Description |
|---------|-------------|
| Registers / Storage | `secret`, `input`, `counter`, `operand2`, plus `lastObj` as a typeless carrier. |
| State Flags | `result` (boolean compare result), `pc` (program counter). |
| Runtime Safety | `MaxSteps` limits infinite loops. |
| Randomness | `rnd` used for secret number and generated pseudo stats. |
| Templates | Language?agnostic string array with formatting placeholders (`{0}`, numeric formats). |

## Instruction Encoding
A raw 32?bit `int` is read and truncated to 8 bits (`byte op`). Bit layout:
```
 High nibble (op >> 4)  => primary instruction group (instr)
 Low  nibble (flags)    => bit0: fParam, bit1: fSet, bit2: fJump, bit3: fOut
```
If `fParam` is set, the *next* `int` in the stream is read as `arg`.

### Instruction Groups (High Nibble)
| Group | Meaning | Selected Flag Combinations (semantic overload) |
|-------|---------|-----------------------------------------------|
| 0 (`NOP/Sleep`) | Idle / sleep | `fParam` + `arg>0` causes `Thread.Sleep(arg)` |
| 1 (IO) | Read / formatted write | `fOut=1` ? template output, else read line / parse int |
| 2 (Move/Func/Load/Store/Imm) | Multifunction data op | Flags choose between init, load, store, function call, immediate load |
| 3 (Compare) | Comparison with `secret` | `fSet=1` equality, else less?than |
| 4 (Jump) | Flow control / halt | No param = halt; with flags = conditional / counter / absolute jump |
| 5 (Rand) | Generate new `secret` | `arg` = inclusive max bound (>=1) |
| 15 (Halt) | Explicit stop | Independent of flags |

### Multifunction (Group 2) Details
| Flags (Out/Set/Param) | Action |
|-----------------------|--------|
| none                  | Reset `lastObj = 0d` |
| Param only            | Load from storage slot `arg` ? `lastObj` |
| Set + Param           | Store `lastObj` into storage slot `arg` |
| Out + Param           | Invoke built?in function by id (0=int,1=double,2=bars '#',3=chart line '|') |
| Out + Set + Param     | Immediate: `lastObj = arg` |

### Jump (Group 4) Variants
| Flags | Meaning |
|-------|---------|
| none          | Unconditional absolute jump to `arg` |
| Jump flag     | Conditional jump if `result == true` |
| Set flag      | Counter loop: decrement `lastObj`, store in `counter`; if still >0 jump to `arg` |
| (No param)    | Halt program (implicit end) |

## Execution Flow of `prog`
1. `Rand 20` – secret number in `[1..20]`.
2. Prompt & read user input.
3. Display a staged "analysis" sequence (sleep + pseudo metrics + bar + percentage + chart line).
4. Load input, compare equality; jump to success if equal.
5. Compare less?than; branch to "under" or fall through to "over".
6. Loop back after each miss; terminate on hit.

## Data Transport Strategy
`lastObj` is a polymorphic shuttle. Conversions are normalized via `SafeToInt` when integer semantics are required (store / compare / counter). Random functions deposit results directly into `lastObj` and subsequent formatted template writes consume it.

## Debugging Support
`Debug` flag prints per?step traces (header + footer) to the Debug output window, showing:
- Decoded flags / opcode.
- Register values before and after.
- `lastObj` type info.

## Error Handling
Any exception inside the interpreter is caught, summarized with opcode context, then halts execution. This prevents corrupted state loops.

## Extending the VM
Suggested easy extensions:
- Add floating comparison group (e.g., tolerance equality).
- Introduce a small stack (push/pop) for expression chains.
- Provide a string pool with indices instead of fixed `templates`.
- Add unsigned / signed distinction for arithmetic.

## Building & Running
```
dotnet run --project Statistic4
```
Input integers 1–20 until "Treffer!" appears; observe pseudo statistics between attempts.

## Educational Value
Demonstrates how multiple semantics can be multiplexed onto a small opcode space by combining flag bits, and how a minimal VM architecture supports experimentation with obfuscation / DSL design.
