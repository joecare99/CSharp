# Task T-FBPARSER-003 Resume parser parity work after VS restart

## Summary
Capture the current FBParser porting state so work can continue immediately after restarting Visual Studio.

## Target area
- `FBParser\FBEntryParser.cs`
- `FBParserTests\FBEntryParserTests.cs`
- `FBParserTests\UntTestFbDataRegressionResults.cs`
- `FBParserTests\Resources\PasTestResults.xml`

## Current status snapshot
- The Pascal state-machine modes were ported into `FBParser\FBEntryParser.cs`, including the GC, child, parent, reference, and additional-information branches.
- The parser sample tests now resolve the Delphi sample directory dynamically:
  - `C:\Projekte\Delphi\Daten\ParseFB`
  - fallback: `C:\Projekte\Delphi\Data\ParseFB`
- The parser sample tests now load `GNameFile.txt` before running sample entry scenarios.
- Focused regression tests for the Pascal reference samples still exist and currently pass after their documented gap positions were updated.

## Latest validation snapshot
- `dotnet build C:\Projekte\CSharp\Gen_FreeWin\FBParserTests\FBParserTests.csproj -nologo` succeeded.
- Latest broad sample run state:
  - `213` tests discovered
  - `113` passed
  - `100` failed

## Important source of truth
- Original Delphi parser results:
  - `FBParserTests\Resources\PasTestResults.xml`
- Original parser source:
  - `C:\Projekte\Delphi\Projekte\Genealogie\Source\ParseFBEntry\unt_FBParser.pas`
- Sample entries and expected callback sequences:
  - `C:\Projekte\Delphi\Data\ParseFB\*.entTxt`
  - `C:\Projekte\Delphi\Data\ParseFB\*.entExp`

## Recently observed mismatch groups
### 1. Whitespace and trim issues
- Example: `OsBM0746.entTxt`
- Expected: `Hans Jacob D.`
- Actual previously contained extra trailing whitespace.

### 2. `ledig` / `led.` / `ledig.` normalization
Examples still called out from Pascal results:
- `OsBM0061.entTxt`
- `OsBM0062.entTxt`
- `OsBM0101.entTxt`
- `OsBM1193.entTxt`
- `OsBM1227.entTxt`

Expected Pascal behavior:
- emit `ParserIndiData` with `ledig`
- keep the remaining occupation separate
- still emit the default occupation place where applicable

### 3. Occupation/description split edge cases
- Example: `OsBM0442.entTxt`

### 4. Known Pascal-side failure that should not be reintroduced
- Example: `OsBM0854.entTxt`
- `PasTestResults.xml` shows the Delphi original failed there with `Wrong Family reference`.
- The C# parser is already beyond that exact Pascal failure and should not be regressed to match the bug.

## Most recent code adjustments already made
- Added robust leading-marker consumption helper for non-person entries.
- Added trimming on emitted individual names.
- Added handling so `ledig` can be emitted separately from occupations.
- Added fallback default-place emission when `ledig` was consumed before occupation handling.

## Resume order agreed with user
Continue in exactly this order:
1. Whitespace and trim fixes
2. `ledig` / `led.` / `ledig.` normalization fixes
3. Occupation/description split edge cases
4. Do not re-port known Pascal bugs such as `OsBM0854`

## Recommended first files/tests after restart
Start with these sample cases:
- `OsBM0746.entTxt`
- `OsBM0061.entTxt`
- `OsBM0062.entTxt`
- `OsBM0101.entTxt`

Then continue with:
- `OsBM1193.entTxt`
- `OsBM1227.entTxt`
- `OsBM0442.entTxt`

## Notes
- Before the restart there was an intermittent DLL lock from PowerShell during ad-hoc inspection runs. A clean Visual Studio restart should remove that blocker.
- Current active implementation file before restart: `FBParser\FBEntryParser.cs`.
