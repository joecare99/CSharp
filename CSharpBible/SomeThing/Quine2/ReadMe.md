# Quine2 – Verbatim String Quine

## Difference From Quine1
Uses a verbatim string literal (`@"..."`) allowing backslash and quote handling differences. The logic is analogous: format string referencing itself.

## Source
```
var c=@"var c=@{1}{0}{1};Console.Write(string.Format(c,c,'\x22'));";Console.Write(string.Format(c,c,'\x22'));
```
`'\x22'` supplies a double quote character.

## Mechanism
1. Store template with `{0}` placeholder for its own content and `{1}` for quoting sequence.
2. Format injects the template and `"` character.
3. Output reproduces source identically.

## Run
```
dotnet run --project Quine2
```

## Educational Point
Shows how verbatim strings simplify escaping while retaining quine structure.
