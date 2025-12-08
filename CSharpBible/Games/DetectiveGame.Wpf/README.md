# DetectiveGame.Wpf

WPF front end for the detective deduction engine (`DetectiveGame.Engine`). Provides a basic windowed interface for starting games and executing suggestions / accusations.

## UI Elements
- Player name text boxes (default values Alice, Bob, Carol)
- Log list box showing history events
- Buttons: Start, Suggestion, Accusation, Next
- Automatic window title update with current turn or winner
- Help shown at load (instructions message box)

## Separation
Currently code-behind handles interactions. Could be refactored to MVVM (e.g. using CommunityToolkit.Mvvm) mirroring the console project.

## Extending
- Replace text inputs with dynamic player add/remove list
- Add modal dialog for suggestion selection (person / weapon / room)
- Provide visual card icons & animations
- Integrate persistent settings (last player names)

## Build
```
dotnet build DetectiveGame.Wpf/DetectiveGame.Wpf.csproj
```

## License
Internal sample WPF UI.
