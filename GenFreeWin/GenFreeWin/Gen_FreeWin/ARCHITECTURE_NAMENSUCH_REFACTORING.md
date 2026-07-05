// ***********************************************************************
// Assembly         : GenFreeWin
// Author           : Mir, Project Team
// Created          : 2025
// ***********************************************************************
// <copyright file="ARCHITECTURE_NAMENSUCH_REFACTORING.md" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary>Architecture documentation for NamenSuchViewModel refactoring</summary>
// ***********************************************************************

# NamenSuchViewModel Refactoring: MVVM-Modernisierung (Phase A)

## Status: In Progress
- ✅ Task 1: Models extracted (SearchCriteria, SearchResult, FilterOptions)
- ✅ Task 2: Service layer created (INameSearchService, NameSearchService)
- ✅ Task 3: Mapper layer created (ISearchResultMapper, SearchResultMapper)
- ✅ Task 4: State adapter created (SearchStateAdapter)
- 🔄 Task 5: ViewModel refactor (Facade pattern - NEXT)
- ⏳ Task 6: Event-Handler deprecation → RelayCommand migration (Phase B)
- ⏳ Task 7: Integration tests + Build validation (Phase C)

## Architecture Overview

### Layers (Inkrementell eingeführt)

```
┌─────────────────────────────────────────────────────┐
│ WinForms View (Namensuch.cs)                        │
│  • UI Controls (TextBox, CheckBox, ListBox, etc.)  │
│  • CommandBindingAttribute.Commit() wiring         │
│  • No business logic                               │
└─────────────┬───────────────────────────────────────┘
			  │ (binds to)
┌─────────────▼───────────────────────────────────────┐
│ NamenSuchViewModel (as MVVM Facade)                │
│  • [ObservableProperty] UI-State properties        │
│  • [RelayCommand] commands (NEW)                  │
│  • ServiceProvider DI injection                    │
│  • Legacy Events (deprecated, for compat)         │
│                                                    │
│ Internal: SearchStateAdapter bridges to Models   │
└─────────────┬───────────────────────────────────────┘
			  │ (delegates to)
┌─────────────▼───────────────────────────────────────┐
│ Services Layer                                     │
│ ├─ INameSearchService / NameSearchService        │
│ │   • ExecuteSearchAsync(criteria)               │
│ │   • ValidateCriteria(criteria)                 │
│ │   • ApplyFilters(results, options)             │
│ │   • SortResults(results, field)                │
│ ├─ ISearchResultMapper / SearchResultMapper      │
│ │   • MapToListItem(result)                      │
│ │   • MapToDisplayText(result, format)           │
│ │   • MapToResultsLabel(count, text)             │
└─────────────┬───────────────────────────────────────┘
			  │ (consumes)
┌─────────────▼───────────────────────────────────────┐
│ Models Layer                                       │
│ ├─ SearchCriteria: Gender, Text, Mode, Flags    │
│ ├─ SearchResult: ID, Name, Date, Type           │
│ ├─ FilterOptions: Aggregate UI-State snapshot   │
│ ├─ SearchMode Enum: PersonBased, FamilyBased   │
│ └─ SearchResultType Enum: Person, Family, Event │
└─────────────┬───────────────────────────────────────┘
			  │ (uses)
┌─────────────▼───────────────────────────────────────┐
│ Data Layer (Via IGenPersistence)                   │
│  • Database/XML access                            │
│  • Cached query results                           │
└─────────────────────────────────────────────────────┘
```

## Key Refactoring Steps for NamenSuchViewModel

### Phase A1: Dependency Injection Setup (🔄 START HERE)
**Create new constructor overload in NamenSuchViewModel:**
```csharp
// KEEP: Old parameterless constructor for backward compatibility
public NamenSuchViewModel() { ... }

// ADD: New DI constructor (preferred)
public NamenSuchViewModel(
	INameSearchService searchService,
	ISearchResultMapper resultMapper)
{
	_searchService = searchService ?? throw new ArgumentNullException(nameof(searchService));
	_resultMapper = resultMapper ?? throw new ArgumentNullException(nameof(resultMapper));

	// Initialize collections as before...
	Option = new(...);
	// ... etc
}

// Internal fields to add:
private INameSearchService _searchService;
private ISearchResultMapper _resultMapper;
private SearchStateAdapter _stateAdapter;

// Initialize adapter in constructor:
_stateAdapter = new SearchStateAdapter(this);
```

### Phase A2: Wrap Core StartSearch() Method (🔄 NEXT)
**Existing method to refactor (LOCATE in file line ~1800-2000):**
```csharp
// OLD: private void StartSearch() { ... /* 200+ lines */ }

// NEW: Add async version that uses service
[RelayCommand]
public async Task ExecuteSearchAsync()
{
	try
	{
		IsLoading = true;
		var criteria = _stateAdapter.BuildSearchCriteria();

		var (isValid, error) = _searchService.ValidateCriteria(criteria);
		if (!isValid)
		{
			StatusMessage = error;
			return;
		}

		var searchResults = await _searchService.ExecuteSearchAsync(criteria);
		var filterOptions = _stateAdapter.CaptureFilterState();
		var filtered = _searchService.ApplyFilters(searchResults, filterOptions);
		var listItems = _resultMapper.MapToListItems(filtered);

		// Populate ListBox1_Items or appropriate collection
		ListBox1_Items.Clear();
		foreach (var item in listItems)
			ListBox1_Items.Add(item);

		StatusMessage = _resultMapper.MapToResultsLabel(listItems.Count, criteria.SearchText);
	}
	catch (Exception ex)
	{
		StatusMessage = $"Search error: {ex.Message}";
		System.Diagnostics.Debug.WriteLine($"Execution error: {ex}");
	}
	finally
	{
		IsLoading = false;
	}
}

// DEPRECATE OLD: Redirect to new command
[Obsolete("Use ExecuteSearchCommand via RelayCommand binding", false)]
private void StartSearch()
{
	try
	{
		_ = ExecuteSearchAsync();
	}
	catch (Exception ex)
	{
		System.Diagnostics.Debug.WriteLine($"StartSearch compat error: {ex.Message}");
	}
}
```

**LOCATE & REFACTOR in NamenSuchViewModel.cs:**
- Line ~200-400: Find `private void StartSearch()`, note its position
- Line ~300-600: Find `Text2List()` (uses IList indexing), keep as utility
- Line ~700-900: Find `SearchTab_GetListItem()`, wrap logic into ValidateFilter()

### Phase A3: Add Missing Properties for Services
**ADD to NamenSuchViewModel near property definitions:**
```csharp
[ObservableProperty]
public partial bool IsLoading { get; set; }

[ObservableProperty]
public partial string StatusMessage { get; set; } = "";
```

## New Files Created (Phase A)
✅ `ViewModels/Models/SearchCriteria.cs` - Typed search input model
✅ `ViewModels/Models/SearchResult.cs` - Typed result model
✅ `ViewModels/Models/FilterOptions.cs` - UI state aggregate
✅ `Services/INameSearchService.cs` - Service contract
✅ `Services/NameSearchService.cs` - Service implementation
✅ `Services/ISearchResultMapper.cs` - Mapper contract
✅ `Services/SearchResultMapper.cs` - Mapper implementation
✅ `ViewModels/SearchStateAdapter.cs` - State bridge

## Integration Points (Manual Steps Needed in NamenSuchViewModel)
1. Add DI constructor with INameSearchService, ISearchResultMapper params
2. Call `_stateAdapter.BuildSearchCriteria()` where Search button clicked
3. Replace `StartSearch()` business logic → call `ExecuteSearchAsync()`
4. Replace `Text2List()` → use `FilterOptions` model
5. Update event-handlers marked [Obsolete] to call RelayCommands

## Backward Compatibility Strategy
- ✅ Keep all existing [ObservableProperty] properties unchanged
- ✅ Keep existing event-handler methods (mark [Obsolete])
- ✅ Add NEW [RelayCommand] commands alongside old events
- ✅ Redirect old events → new commands (adapter pattern)
- ⏳ Eventually phase out old events in Phase B/C

## Next Steps (Phase B)
1. Apply manual edits to NamenSuchViewModel.cs (as documented above)
2. Add [RelayCommand] for Filter operations (CheckMale, CheckFemale, etc.)
3. Build & validate no compile errors
4. Integration test: Ensure search results populate correctly
5. Performance test: Async search doesn't freeze UI

## Future Improvements (Phase C+)
- [ ] Extract Image utilities (PicResizeByWidth, AutoSizeImage) → ImageUtility service
- [ ] Migrate FileSystem-based persistence → IGenPersistence wrapper
- [ ] Replace VB.NET APIs (Strings.Asc, Information.Err()) → Modern C# equivalents
- [ ] Break down remaining 10.000+ lines into functional service layers
- [ ] Add proper dependency injection container registration
- [ ] Comprehensive unit tests for SearchService, Mapper, Adapter

---
**Refactoring Approach**: Incremental, backward-compatible, servicable in phases.
**Current Phase**: A (Model/Service extraction + Adapter)
**Estimated Effort**: Phase A ~4 hours, Phase B ~6 hours, Phase C ~10+ hours (ongoing discovery)
