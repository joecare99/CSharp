# Statistik – Simple Console Guessing Game (Pseudo Statistical Output)

## Summary
A minimal number guessing game (secret 1–20) displayed as a faux statistical analyzer. After each user input it prints a random analysis label, a progress bar, and random pseudo statistical values.

## Flow
1. Generate secret `g` in `[1,20]`.
2. Loop:
   - Prompt `In:` and read user input.
   - Emit random phrase from `t` (e.g., `Stdabw...`).
   - Animate progress bar width 20 (0–100% in 5% steps).
   - If input parses to integer:
     - Compare with secret; print `Treffer!`, `Zu klein`, or `Zu groß`.
     - Always print three pseudo metrics: `Avg`, `Var`, `Corr` using random numbers.
   - Else report invalid (`Ungültig`).
3. Terminate only when the user guesses the secret.

## Key Elements
- `Random r` reused for all randomness.
- Phrase selection via indexed array.
- Progress bar uses carriage return `\r` to update same line.
- Basic input validation with `int.TryParse`.

## Extensibility Suggestions
- Limit number of attempts; reveal secret on failure.
- Add input command (`q`) to quit.
- Track statistics: attempt count, min/max deviation.
- Replace blocking `Thread.Sleep` with async version for GUI contexts.

## Run
```
dotnet run --project Statistik
```
Enter guesses until `Treffer!` appears.
