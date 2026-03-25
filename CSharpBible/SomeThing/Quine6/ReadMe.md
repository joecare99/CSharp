# Quine1 – Minimal C# Quine (Escaped Format String)

## What Is a Quine?
A program that outputs its own exact source code without reading external files or using reflection on its compiled form.

## Technique Used
- Stores a format string in variable `c` that contains placeholders for itself and a quote character.
- Calls `Console.Write(string.Format(c,c,(char)34));`.
- `(char)34` inserts the double quote `"`.
- The format string replicates the assignment and write call producing identical output.

## Source
```
var c="var c={1}{0}{1};Console.Write(string.Format(c,c,(char)34));";Console.Write(string.Format(c,c,(char)34));
```

## Flow
1. Declare `c` containing the full template plus placeholders `{0}` (for the string itself) and `{1}` (for a quote).
2. Print by formatting with arguments: the template itself and `"`.
3. Output equals original source.

## Run
```
dotnet run --project Quine1
```

## Educational Point
Demonstrates self-replication via single format string and minimal syntax in C#.
