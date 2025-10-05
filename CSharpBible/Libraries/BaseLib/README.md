# BaseLib

Foundational helper library providing utilities, helpers and abstraction classes for multiple projects (Calc32 / 64, ConsoleLib extensions, MVVM infrastructure).

## Contents (Selection)
- Helpers: `ByteUtils`, `FileUtils`, `MathUtilities`, `StringUtils`, `TypeUtils`, `PropertyHelper`, `ClassHelper`, `ListHelper`, `ObjectHelper`.
- Models: `ConsoleProxy` (abstraction over `System.Console`), `CRandom` / `IRandom`, `SysTime` / `ISysTime`.
- Notification / MVVM: `NotificationObjectAdv` (enhanced INotifyPropertyChanged implementation), `INotifyPropertyChangedAdv` interface.
- IoC support: `Helper/IoC.cs` for centralized service resolution.

## Goals
- Reduce duplication across UI and logic projects.
- Provide testable abstractions (time, randomness, console) for isolation in unit tests.

## Design
- Small, focused static helpers instead of monolithic god classes.
- Interfaces for each external dependency (time, console) to enable mocking.

## Usage
Other projects reference `BaseLib.csproj` directly. No UI dependencies – pure .NET classes.

## Extension
Add new helpers only when used in multiple places. Move complex services to dedicated libraries to keep `BaseLib` lean`.
