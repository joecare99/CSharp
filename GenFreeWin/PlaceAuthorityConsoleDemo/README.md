# PlaceAuthorityConsoleDemo

Console demonstration application showcasing multi-provider place search using the `GenFreeBrowser.Places` abstractions (e.g. Nominatim, GOV) combined with MVVM patterns and the `ConsoleLib` UI toolkit.

## Purpose
Provide a lightweight, dependency-light environment to iterate on place authority search logic (query shaping, result normalization, history / MRU handling) without full WPF / WinForms overhead.

## Scenario
A user types a query, presses Enter, results from configured authorities are merged and displayed. Arrow keys navigate history (when focus in query) or results (when focus moved to list). Tab toggles focus. Selection status is shown in a footer.

## Key Features
- MVVM separation (`SearchViewModel`, `ConsoleAppViewModel`).
- Search history persistence (`SearchHistoryService`).
- DI with `Microsoft.Extensions.DependencyInjection`.
- Two-way binding for query textbox.
- Result list navigation + selection sync.

## Targets
`net9.0-windows` (depends on modern console & multi-targeted libraries).

## Architecture
```
Program (composition root)
  DI registrations (HttpClient, authorities, services)
  -> ConsoleSearchView (View) + ConsoleAppViewModel
       -> SearchViewModel (domain search logic)
            -> IPlaceSearchService / IPlaceAuthority implementations
```

## Running
```bash
cd Gen_FreeWin/PlaceAuthorityConsoleDemo
 dotnet run
```

## Extending
- Add a new authority: implement `IPlaceAuthority`, register as singleton.
- Introduce filtering: extend `PlaceQuery` (e.g. country code) and adjust input parsing.
- Add result detail panel: create another control bound to `SelectedResult`.

## Example Code (Search Trigger)
```csharp
if (viewModel.DoSearchCommand.CanExecute(null))
    await viewModel.DoSearchCommand.ExecuteAsync(null);
```

## Limitations / Next Steps
- No paging; all results loaded into memory.
- No cancellation exposure in UI (internal timeout only).
- Minimal error reporting.

## Contributing
Follow repository coding / AI guidelines. Prefer small, focused PRs.

## License
(Insert license notice.)
