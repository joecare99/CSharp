# CsEpiphany

A collection of exploratory C# coding challenges / epiphanies packaged as a game or interactive console scenarios. Serves as a sandbox to demonstrate language features, patterns and micro-optimizations.

## Focus Areas
- Modern C# syntax demonstrations
- Span / memory experiments
- LINQ vs imperative performance comparisons
- Pattern matching & records usage in small puzzles

## Structure
Multiple small scenarios or mini levels each highlighting a concept. The project can be extended by dropping in new modules adhering to a simple interface (e.g. `IRunnableDemo`).

## Running
```
dotnet run --project CsEpiphany/CsEpiphany.csproj
```
Select a demo from the menu (if implemented) or run a default showcase.

## Contributing New Demos
1. Create a class implementing the agreed interface or base class.
2. Register it in a factory / list.
3. Provide a short textual explanation printed before execution.

## Potential Enhancements
- Benchmark harness integration
- Live code reloading for rapid experimentation
- Result diffing vs baseline output

## License
Internal educational sandbox.
