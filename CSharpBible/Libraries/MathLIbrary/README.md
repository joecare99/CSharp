# MathLibrary

Reusable, UI-independent mathematical and signal-processing helpers targeting the framework versions declared in `MathLibrary.csproj`.

## Common signal processing

`MathLibrary.Common.SignalProcessing` provides scalar operations:

- `ApplyFirstOrderLowPass` updates the caller-owned state using `state = alpha * gain * input + (1 - alpha) * state`. `alpha` must be finite and between 0 and 1; 0 holds the state and 1 immediately applies the gained input.
- `UpdateMovingAverage` replaces the current slot of a caller-owned circular buffer, advances its index, and returns the mean of the entire buffer. Initialize unused slots to the desired startup value (commonly zero).
- `UpdateMovingMedian` applies the same circular update and returns the statistical median; even-sized windows average their two middle values.
- `UpdateCircularQuantile` preserves selectable order-statistic behavior, choosing `floor(length * quantile)` with the upper endpoint clamped to the final item.
- `UpdateCircularMedian` is a compatibility-oriented name for that configurable order statistic; it does not compute the statistical median for even window sizes. Use `UpdateMovingMedian` for the statistical definition.
- `MagnitudePhaseToComplex` converts magnitude and phase in radians to a `Complex` value.

Buffer operations require a non-empty array and an index within its bounds. The caller owns buffer lifetime and synchronization.

## Numeric comparison

`MathLibrary.Common.NumericComparisons.RelativeMatch` preserves the former application test helper's float similarity score and exact equality fallback for non-float inputs. This is a heuristic score, not a general-purpose floating-point equality test.

These APIs contain no UI or application-specific dependencies. Their contracts and edge cases are covered by `MathLibraryTests`.
