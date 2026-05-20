# Task T-FBPARSER-004 Refactor FBEntryParser into maintainable components

## Summary
Prepare a maintainability-focused refactoring for `FBParser\FBEntryParser.cs` by separating stable responsibilities without breaking the current regression behavior of the Pascal-port state machine.

## Parent
- Task family: `FBParser` parser port stabilization and maintainability work
- Related existing item: `T-FBPARSER-001` Add tests for new parser port classes

## Motivation
`FBEntryParser` currently combines parser configuration, parse state, callback emission, token analysis, helper utilities, and a large mode-driven state machine in a single class. This makes the parser hard to read, hard to change safely, and difficult to extend while preserving Pascal parity.

## Current findings
- `FBParser\FBEntryParser.cs` contains several distinct responsibilities in one type.
- The parser already has useful regression-oriented tests in `FBParserTests\FBEntryParserTests.cs`.
- The heaviest maintainability hotspot is the `Feed` state machine with many local variables and tightly coupled transitions.
- Helper methods such as `AnalyseEntry`, `HandleNonPersonEntry`, `HandleFamilyFact`, `HandleGCDateEntry`, `HandleGCNonPersonEntry`, `BuildName2`, and `HandleAKPersonEntry` are reasonable first candidates for extraction.
- `FBParser\FBParser.csproj` and `FBParserTests\FBParserTests.csproj` have now been switched to disabled `ImplicitUsings` for this refactoring slice, so touched files use explicit dependencies.

## Work completed
- Added the internal analyzer abstraction `IGenealogicalEntryAnalyzer` in `FBParser\Analysis`.
- Added the immutable analyzer configuration `GenealogicalEntryAnalyzerConfiguration`.
- Added the first extracted collaborator `GenealogicalEntryAnalyzer` for `GetEntryType`, `AnalyseEntry`, `TrimPlaceByMonth`, and `TrimPlaceByModif`.
- Kept `FBEntryParser` as the public facade and delegated the extracted analysis logic via composition.
- Added `InternalsVisibleTo` so the internal analyzer can be covered directly by tests.
- Added focused analyzer tests in `FBParserTests\GenealogicalEntryAnalyzerTests.cs`.
- Moved analyzer-specific assertions out of `FBParserTests\FBEntryParserTests.cs` to reduce duplicated coverage.
- Disabled `ImplicitUsings` in `FBParser` and `FBParserTests` and added explicit `using` directives in touched files.
- Added the internal fact-handler abstraction `IGenealogicalFactHandler` in `FBParser\Analysis`.
- Added `GenealogicalFactHandlerConfiguration` and focused fact-handler delegates for parser-specific callbacks.
- Added `GenealogicalFactHandler` for `HandleNonPersonEntry` and `HandleFamilyFact` behavior.
- Kept debug logging in `FBEntryParser` and delegated fact emission logic via composition.
- Added direct fact-handler tests in `FBParserTests\GenealogicalFactHandlerTests.cs`.
- Added the internal person-name abstraction `IGenealogicalPersonNameHandler` in `FBParser\Analysis`.
- Added `GenealogicalPersonNameHandlerConfiguration` and dedicated delegates for name-related parser callbacks.
- Added `GenealogicalPersonNameHandler` for the extracted `HandleAKPersonEntry` logic.
- Kept debug logging in `FBEntryParser` and delegated person-name handling via composition.
- Added direct person-name tests in `FBParserTests\GenealogicalPersonNameHandlerTests.cs`.
- Added the internal name-token abstraction `IGenealogicalNameTokenBuilder` in `FBParser\Analysis`.
- Added `GenealogicalNameTokenBuilderConfiguration` and the required token-builder delegate for `ParseAdditional`.
- Added `GenealogicalNameTokenBuilder` for the extracted `BuildName` and `BuildName2` logic.
- Delegated parser name-token building via composition while keeping parser-facing method names stable.
- Added direct token-builder tests in `FBParserTests\GenealogicalNameTokenBuilderTests.cs`.
- Added the internal event-emitter abstraction `IGenealogicalEventEmitter` in `FBParser\Analysis`.
- Added `GenealogicalEventEmitterConfiguration` for validation callbacks and parser event sinks.
- Added `GenealogicalEventEmitter` for validated family, individual, relation, and lifecycle callback emission.
- Delegated callback emission and validation through the emitter while keeping parser-facing helper method names stable.
- Added direct emitter tests in `FBParserTests\GenealogicalEventEmitterTests.cs`.
- Added the internal GC-helper abstraction `IGenealogicalGcHelper` in `FBParser\Analysis`.
- Added `GenealogicalGcHelperConfiguration` for GC markers, default-place access, and parser callback injection.
- Added `GenealogicalGcHelper` for `HandleGCDateEntry`, `HandleGCNonPersonEntry`, and `ScanForEvDate`.
- Delegated GC helper behavior through composition while keeping parser-facing helper method names stable.
- Added direct GC-helper tests in `FBParserTests\GenealogicalGcHelperTests.cs`.

## Refactoring options

### Option A Minimal-risk extraction
Keep `FBEntryParser` as the main public facade and extract only helper collaborators.

Candidate classes:
- `FBEntryParserConstants` or `FBEntryParserMarkers`
- `FBEntryParserEmitter` for callback emission plus place/date validation
- `FBEntryParserEntryAnalyzer` for `GetEntryType`, `AnalyseEntry`, `TrimPlaceByMonth`, and `TrimPlaceByModif`
- `FBEntryParserNameHandler` for `BuildName`, `BuildName2`, and `HandleAKPersonEntry`

Pros:
- Lowest regression risk
- Public API can remain nearly unchanged
- Good first step before touching the state machine

Cons:
- `Feed` remains large
- Some coupling remains because many locals stay in the main class

### Option B Stateful context plus flow handlers
Introduce a parser context object and move groups of modes into dedicated flow handlers.

Candidate classes:
- `FBEntryParserContext` for the mutable state currently held in `Feed`
- `FBEntryParserCoreFlow` for modes `0..15`
- `FBEntryParserReferenceFlow` for modes `50..57` and `150..156`
- `FBEntryParserGcFlow` for modes `100..126`
- `FBEntryParserEntryServices` for shared helper logic

Pros:
- Best long-term maintainability
- State becomes explicit and testable
- Mode clusters become easier to reason about

Cons:
- Higher implementation risk
- More adaptation work for tests and internal APIs
- Requires careful design so performance and parity are preserved

### Option C Transitional base class
Create a base class for shared parser services and keep `FBEntryParser` as a specialized orchestration layer.

Candidate classes:
- `FBEntryParserBase` for validation, diagnostics, emitters, shared constants access, and helper services
- `FBEntryParser` keeps orchestration and public events
- Optional additional partial flow classes later

Pros:
- Makes inheritance-based extension possible
- Reduces size of the public parser class
- Can be combined with Option A

Cons:
- Inheritance alone does not solve the large state machine
- May hide dependencies instead of clarifying them if overused

## Recommended staged approach
1. Extract immutable parser markers and grouped lookup arrays into a dedicated internal class.
2. Extract callback emission plus validation into an internal emitter/service that remains owned by `FBEntryParser`.
3. Extract entry-analysis helpers into an internal analyzer service with focused tests.
4. Introduce an explicit parser context for the local variables used across the `Feed` loop.
5. Only after tests are green, split the `Feed` mode ranges into dedicated flow handlers.

## Assumptions
- Behavior parity is currently more important than API redesign.
- Existing tests should stay the primary safety net during the first refactoring slice.
- Internal classes are acceptable even if the public surface remains unchanged.
- No global usings should be introduced for the refactoring.

## Open questions
- Should the first implementation slice stay strictly internal with no public API changes?
- Is a base class actually desired, or is composition preferred once the parser is split?
- Which goal has priority for the first slice: smaller file size, clearer responsibilities, or improved test isolation?
- Should `ImplicitUsings` be disabled in `FBParser.csproj` as part of this parser refactoring, or handled separately?

## Proposed first implementation slice
Extract `AnalyseEntry` and related helper logic into an internal collaborator first, because this area is already covered by tests and is less risky than splitting the state machine directly.

## Validation strategy
- Run `FBParserTests` after every extraction slice.
- Prefer preserving existing regression sequences before introducing any behavioral cleanup.
- Add focused MSTest coverage for each newly extracted collaborator before larger state-machine splits.

## Next refinement steps
- Confirm preferred architecture direction: composition-first, base-class-first, or mixed.
- Decide whether the first coding change should target helper extraction or a parser context object.
- Create a dedicated follow-up task for test additions around newly extracted services.
- Extract the next parser responsibility, likely grouped mode handling from `Feed` or an explicit parser context object, into a dedicated collaborator.
