# Task T-FBPARSER-001 Add tests for new parser port classes

## Summary
Add comprehensive regression-oriented tests for the newly created `FBParser` classes before continuing the larger Pascal-to-C# parser migration.

## Scope
- `FBParser\PascalCompat.cs`
- `FBParser\GNameHandler.cs`
- `FBParser\ParserBase.cs`
- `FBParser\ParserEventType.cs`
- `FBParser\ParseMessageKind.cs`
- `FBParser\FBEntryParser.cs`
- `FBParserTests\*.cs`

## Motivation
The parser port currently introduces several foundational compatibility helpers. Those helpers must be locked down with tests before the remaining state-machine migration continues, otherwise later changes could silently drift away from the Pascal behavior.

## Work completed
- Added focused MSTest coverage for Pascal-compatible string helpers and character set definitions.
- Added persistence and behavior tests for the new `GNameHandler` implementation.
- Added base-class contract tests for `ParserBase`.
- Added enum order/value tests for `ParserEventType` and `ParseMessageKind`.
- Added callback-oriented tests for the currently ported `FBEntryParser` surface and helper methods.
- Fixed the `FBParserTests` project reference so the test project resolves `FBParser` correctly.

## Validation
- `dotnet build C:\Projekte\CSharp\Gen_FreeWin\FBParserTests\FBParserTests.csproj -nologo`

## Follow-up
- Execute the tests after the remaining parser modes are ported.
- Add scenario tests with real sample family-book entries once the full state machine is available.
