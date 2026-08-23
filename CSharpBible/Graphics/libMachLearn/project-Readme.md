# libMachLearn

`libMachLearn` contains a small feed-forward neural-network implementation
used by the Graphics learning samples. `NeuralNetwork` exposes sequential and
parallel feed-forward/training paths, configurable activation functions, and
JSON model persistence.

## Test strategy

`libMachLearn.Tests` exercises deterministic behavior by replacing the
`IRandom` service used by `Layer` construction. The tests cover:

- equivalence of sequential and parallel feed-forward evaluation;
- JSON save/load preservation of layer sizes, weights, biases, and learning rate;
- parameter updates during a no-dropout training step.

Run the focused suite with:

```powershell
dotnet test C:\Projekte\CSharp\CSharpBible\Graphics\libMachLearn.Tests\libMachLearn.Tests.csproj
```

The focused coverage run on 2026-08-21 reported 15.16% for the complete
`libMachLearn` assembly and 48.6% for `NeuralNetwork`; the suite is an initial
regression slice, not yet the planned 100% class target.

The library currently has no binary-model reader; `SaveBinary` is therefore
documented as a write-only export path until a matching reader is introduced.
