# Quine3 – XOR-Encoded Quine With Tuple Deconstruction

## Concept
More obfuscated quine leveraging:
- Tuple deconstruction for multi-variable assignment.
- XOR transformation of a payload string to reconstruct printable source.
- Minimal loops and character arithmetic instead of format strings.

## Source Structure
```
var(c,d,e)=("!/!ubq+`/g/f*>+_!!/!!*8elqfb`k+ubq#e#jm#`*f(>+`kbq*+e]0*8@lmplof-Tqjwf+g(`(f*8","var(c,d,e)=(\""," ");foreach(var f in c)e+=(char)(f^3);Console.Write(d+c+e);
```
(Whitespace added here for readability.)

## Mechanism
1. `c` contains XOR-encoded content (each char original^3).
2. `d` starts the literal source prefix: `var(c,d,e)=("`.
3. `e` accumulates decoded characters.
4. Loop: decode each encoded char by `f ^ 3` and append to `e`.
5. Print `d + c + e` forming the original program text.

## Run
```
dotnet run --project Quine3
```

## Educational Point
Demonstrates code self-replication using lightweight symmetric encoding rather than format placeholders; adds obfuscation to the quine pattern.
