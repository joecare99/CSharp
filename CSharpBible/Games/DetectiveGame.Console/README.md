# DetectiveGame.Console

Console UI (text mode) front end for the detective deduction engine (`DetectiveGame.Engine`). Implements an MVVM style pattern using CommunityToolkit MVVM to separate presentation and logic.

## Features
- Start new game with predefined players (configurable extension point)
- Display player list & action history
- Suggestion dialog (modal panel) to pick person / weapon / room
- Accusation & turn advancement commands
- Inline help / instructions display

## Architecture
```
GameView (Console controls & layout)
  ? GameViewModel (commands, observable state)
    ? GameService (engine logic)
```

## Running
```
dotnet build DetectiveGame.Console/DetectiveGame.Console.csproj
(dotnet run ...)
```

## Extending
- Parameterize player entry instead of hard coded names
- Hide revealed card from all but asking player (privacy layer)
- Add AI players (inject strategy service)

## License
Internal sample front end.
