# Sudoku_Base

Core logic for a Sudoku puzzle engine. Manages board state, candidate handling, validation and solving helper hooks.

## Features
- 9x9 grid model with subgrid (box) indexing
- Validation of row / column / box constraints
- Candidate set management per cell
- Basic solving strategies (extendable) placeholder

## Extending
- Implement advanced strategies (naked pairs, x-wing, etc.)
- Add puzzle generator with difficulty grading
- Add solver progress event stream for UI visualization

## Consuming UI
Console or GUI layers render board, request user input or auto-solve steps. This engine remains independent of presentation.

## Build
```
dotnet build Sudoku_Base/Sudoku_Base.csproj
```

## License
Internal puzzle engine sample.
