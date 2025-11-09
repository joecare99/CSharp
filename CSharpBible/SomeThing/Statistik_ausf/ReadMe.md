# Statistik_ausf – User Friendly Statistical Guessing Game

## Purpose
Readable, user–friendly version of the guess?the?number game with themed statistical analysis output.

## Features
- Random secret integer 1–20.
- Themed status phrases (standard deviation, Fourier transform, normalization, covariance, Monte Carlo).
- Animated progress bar (21 frames, 0–100%).
- Pseudo statistical metrics (average, variance, correlation) per attempt.
- Clear success / failure messaging with emoji.

## Flow Summary
1. Initialize secret and user guidance banners.
2. Loop:
   - Prompt for user number.
   - Show randomized analysis phrase.
   - Display progress bar (`#` fill, fixed width 20, 5% increments).
   - If parse succeeds:
     - Compare to secret and output status.
     - Generate three pseudo metrics.
   - Else print parse error message.
3. Terminate when the secret number is guessed.

## Design Notes
- Keeps literals directly (no obfuscation) for clarity.
- Uses `Thread.Sleep(50)` to create perceptible progress animation.
- Percentage computed using loop index * 5 (20 steps ? 100%).

## Potential Enhancements
- Colorize output (already partially present in other variants) using `Console.ForegroundColor`.
- Track attempt count and display on success.
- Add adaptive hints (distance feedback).
- Replace blocking sleep with asynchronous approach if ported to GUI / TUI framework.

## Run
```
dotnet run --project Statistik_ausf
```
Input integers until the success confirmation appears.
