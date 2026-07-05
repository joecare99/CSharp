# Dub (Duplicate Resolution) Refactoring Architecture

## Overview
The `Dub` genealogy duplicate-resolution feature has been refactored from a monolithic `DubViewModel` (2366 lines) into a layered MVVM architecture with clear separation of concerns:
- **Models** (`DubOperationState`, `DubSearchResult`) - domain data structures
- **Services** - business logic and data persistence
- **ViewModel** - UI state management and command orchestration
- **View** - WinForms presentation

## Architecture Layers

### 1. Domain Models
Located in: `..\Gen_FreeWin\GenFreeWin\Gen_FreeWin\Models\`

- **`DubOperationState.cs`**
  - Encapsulates the operational context for duplicate pair analysis
  - Tracks Person1/2 IDs, Family1/2 IDs, differing events, transaction state
  - Passed between ViewModel and services to maintain coherent state

- **`DubSearchResult.cs`**
  - Represents a single search candidate for duplicate detection
  - Contains PersonId, FullName, DateYear, Sex, ParentNames, ChildNames, DisplayText
  - Bindable to list controls in the View

### 2. Service Layer
Located in: `..\Gen_FreeWin\GenFreeWin\Gen_FreeWin\Services\`

#### Interfaces
Located in: `..\Gen_FreeWin\GenFreeWin\Gen_FreeWin\Services\Interfaces\`

- **`IDuplicateDetectionService`**
  - `SearchByNameAsync(searchText, filters)` - finds candidates by name/phonetic pattern
  - `SearchByUidAsync()` - detects duplicate UID conflicts
  - `GetPersonDetailsAsync(personId)` - retrieves genealogy info for a candidate
  - Extracted from: `DubViewModel.Zeig()`, `ZeigNaNum()`, `List1_DoubleClick()`

- **`IDuplicateResolutionService`**
  - `MergePersonsAsync(state, targetPersonId)` - consolidates two person records
  - `MergeFamiliesAsync(state, targetFamilyId)` - consolidates two family records
  - `ComparePersonEventsAsync(p1, p2)` - identifies event differences
  - `SwapPersonEventsAsync(p1, p2)` - exchanges/demotes variant events
  - `ValidateMergeAsync(p1, p2)` - pre-merge safety checks
  - Extracted from: `DubViewModel.Tausch()`, `Ertausch()`, `Famweg()`

- **`IPersonDataService`**
  - `GetFullNameAsync(personId)` - formatted name with prefix/suffix/alias
  - `GetSexAsync(personId)` - sex indicator
  - `ConsolidateNamesAsync(sourceId, targetId)` - merge aliases and remarks
  - `GetAncestorDataAsync(personId)` - ancestry indicators
  - `GetParentFamilyAsync(personId)` - parent marriage link
  - `GetSpouseFamiliesAsync(personId)` - spouse/partnership families
  - Extracted from: `DubViewModel.Namen()`, ancestral traversals

#### Implementations
- **`DuplicateDetectionService.cs`** (Stubs with TODO comments)
- **`DuplicateResolutionService.cs`** (Stubs with TODO comments)
- **`PersonDataService.cs`** (Partially implemented; GetFullName complete)

### 3. ViewModel Layer
Located in: `..\Gen_FreeWin\GenFreeWin\Gen_FreeWin\ViewModels\`

- **`DubViewModel_New.cs`** - New refactored ViewModel
  - Inherits from `CommunityToolkit.Mvvm.ComponentModel.ObservableObject`
  - **Observable Properties** (via `[ObservableProperty]` for source generation):
	- `CurrentState` - active DubOperationState
	- `SearchResults1`, `SearchResults2` - ObservableCollection<DubSearchResult>
	- `SearchText1`, `SearchText2` - input bindings
	- `IsMergeInProgress`, `IsTransactionActive` - operation flags
	- `FilterMaleOnly`, `FilterFemaleOnly`, `IncludeParentMatching`, `IncludeChildMatching` - filter state

  - **Relay Commands** (via `[RelayCommand]` for source generation):
	- `Command1Click(buttonIndex)` - dispatcher for numbered button commands
	- `SearchDuplicates()` - triggers detection services
	- `MergeDuplicates(targetChoice)` - orchestrates merge operations
	- `CloseDialog()` - exit and return to menu

  - **Dependency Injection**:
	- Constructor accepts IDuplicateDetectionService, IDuplicateResolutionService, IPersonDataService
	- All three services are injected and available for command execution

  - **Backward Compatibility**:
	- Event handler properties (Button1_Click, CheckBox2_Click, etc.) map to commands
	- List1_DoubleClick, List1_MouseDown, Text1_TextChanged handlers remain for Dub.cs binding
	- Dub.cs wiring remains largely unchanged; commands are delegated to services

### 4. View Layer
Located in: `..\Gen_FreeWin\GenFreeWin\Gen_FreeWin\Views\`

- **`Dub.cs`** - WinForms view shell
  - Remains largely unchanged; wires IDubViewModel interface
  - Event handlers forward to ViewModel (now as relay command delegates)
  - Can optionally add TextBindingAttribute bindings for two-way sync of SearchText1/2 and filters

## Data Flow Example: Search Operation

1. **User enters search text in TextBox1**
   - TextBox1.TextChanged → Text1_TextChanged event
   - Event calls SearchDuplicatesCommand.ExecuteAsync()

2. **ViewModel.SearchDuplicates()**
   - Reads SearchText1, FilterMaleOnly, etc. from observable properties
   - Calls _detectionService.SearchByNameAsync(SearchText1, filters)

3. **DuplicateDetectionService.SearchByNameAsync()**
   - Queries DataModul.DSB_SearchTable with name pattern
   - Optionally applies sex/parent/child filters
   - Returns List<DubSearchResult>

4. **ViewModel.SearchDuplicates() continued**
   - Populates SearchResults1 ObservableCollection
   - Any View bound to SearchResults1 automatically updates (via binding)

5. **User double-clicks a result in List1**
   - List1.DoubleClick → List1_DoubleClick event
   - Event handler populates CurrentState with selected pair info
   - Calls ComparePersonEventsAsync to identify differences
   - View refreshes to show comparison

## Merge Operation Flow

1. **User clicks "Merge" button (e.g., Command1_1 for swap)**
   - Button.Click → Command1Click(1) relay command

2. **ViewModel.Command1Click(1)**
   - Dispatches to one of private Command1_X_Click methods
   - May call MergeDuplicates("1") or SwapPersonEventsAsync

3. **DuplicateResolutionService.SwapPersonEventsAsync()**
   - Compares events between Person1 and Person2
   - Consolidates into primary/secondary/demoted variants
   - Updates DataModul records via direct table operations
   - Returns success/failure

4. **On Success:**
   - Clear SearchResults1/2
   - Reset CurrentState
   - Show confirmation message
   - Ready for next duplicate pair

## Migration Path

### Phase 1: Parallel ViewModel (Current State)
- Original `DubViewModel.cs` remains unchanged
- New `DubViewModel_New.cs` coexists alongside
- Services are standalone and testable
- Models are framework-agnostic

### Phase 2: Swap ViewModel (Next Step)
1. Backup original `DubViewModel.cs` as DubViewModel_Legacy.cs
2. Rename `DubViewModel_New.cs` to `DubViewModel.cs`
3. Update Dub.cs constructor to inject services (via DI container if available, or manual wiring)
4. Test event forwarding and command execution
5. Validate with existing test suite (if any)

### Phase 3: Complete Service Implementation
- Replace TODO stubs in service implementations with full logic extracted from original ViewModel
- Handle transaction management (Begin/Commit/Rollback)
- Implement full merge workflows (Namen consolidation, event migration, link updates, deletion)
- Add comprehensive error handling and logging

## Testing Strategy

1. **Unit Tests for Services**
   - Mock IModul1 and DataModul dependencies
   - Test search algorithms, merge validation, event comparison
   - Verify name consolidation and link updates

2. **Integration Tests for ViewModel**
   - Wire real services
   - Test command execution and property change notifications
   - Verify SearchResults and CurrentState updates

3. **UI Tests (Manual)**
   - Test Dub.cs form load, event forwarding
   - Verify list item selection and detail display
   - Test merge operations end-to-end

## Dependencies

### Internal
- `Gen_FreeWin.Models` (DubOperationState, DubSearchResult)
- `Gen_FreeWin.Services.Interfaces` (DuplicateDetectionService, etc.)
- `CommunityToolkit.Mvvm` (ObservableObject, RelayCommand attributes)

### External
- `GenFree.*` (IModul1, DataModul, genealogy data interfaces)
- `System.Windows.Forms` (WinForms controls, event types)

## Future Enhancements

1. **Async UI Updates** - All Search operations are already async; leverage for background loading
2. **TextBindingAttribute Integration** - Consider using WFSystem.Data bindings for SearchText1/2 and filter checkboxes
3. **Undo/Rollback** - Add explicit transaction management with user confirmation
4. **Duplicate Algorithms** - Implement phonetic matching (Soundex, Levenshtein) for smarter suggestions
5. **Batch Operations** - Allow marking multiple pairs and batch-processing merges
6. **Audit Logging** - Record all merge operations with timestamps and user info

## Summary

This refactoring achieves:
- ✅ **Separation of Concerns**: UI, state, logic, and data are in distinct layers
- ✅ **Testability**: Services can be unit-tested independently
- ✅ **Maintainability**: 2366-line monolith split into focused, single-responsibility classes
- ✅ **MVVM Compliance**: Proper use of CommunityToolkit.Mvvm patterns (`[ObservableProperty]`, `[RelayCommand]`)
- ✅ **Scalability**: Easy to add new service methods or commands without touching the ViewModel core
- ✅ **Backward Compatibility**: Event handlers and Dub.cs wiring remain stable during transition
