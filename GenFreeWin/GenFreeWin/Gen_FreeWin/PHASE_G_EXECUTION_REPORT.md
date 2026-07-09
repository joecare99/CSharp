# Phase G Execution Report: Scope Separation & Model Extraction

## Overview
Phase G focused on separating search-state concerns from the ViewModel by introducing a dedicated `SearchContext` model. This addresses the broader MVVM goal of moving data/state out of the ViewModel while maintaining backward compatibility with the legacy codebase.

## Objective
- Extract mutable search state from `Listfuell()` into a reusable model
- Reduce local-variable clutter in the search entry point
- Prepare future helper methods to use `SearchContext` for consistent state management
- Validate with build after introducing the model layer

## Changes Made

### 1. Created SearchContext Model
**File**: `ViewModels/Models/SearchContext.cs`

A new dedicated model encapsulating all search-related working state:
- **SearchType** (string) — Selected search type from ComboBox2 (e.g., t308, t311, t312, etc.)
- **SearchText** (string) — Primary search input (Text1_Text)
- **FilterText** (string) — Secondary search filter (Text2_Text)
- **Message** (string) — Error or validation messages to display
- **ParseIndex** (int) — Loop counter for descendant/ancestor number parsing
- **ParseEndIndex** (int) — End boundary for parse loop (was num8)
- **ResultCounter** (int) — Counter for result iteration (was num7 in loops)
- **IncludeSpouse** (bool) — Flag for spouse inclusion (!OmitSpouse_Checked)
- **CurrentResult** (string) — Aggregated result buffer
- **IsError** (bool) — Error state flag

**Methods**:
- `Reset()` — Clears all state for a new search run
- `ToString()` — Debug representation

### 2. Integrated SearchContext into NamenSuchViewModel
**File**: `ViewModels/NamenSuchViewModel.cs`

Added field at line ~54:
```csharp
private readonly SearchContext _searchContext = new();
```

Located alongside existing extracted models (`PersonSearchData`, `SearchUIState`) to create a three-layer model structure for data, UI state, and search context.

### 3. Refactored Listfuell() Entry Point
**Location**: Lines ~2783–2820

- Removed manual local `string text; string str; string text2; string item; string prompt;`
- Added SearchContext initialization:
  ```csharp
  _searchContext.Reset();
  _searchContext.IncludeSpouse = !OmitSpouse_Checked;
  _searchContext.SearchText = Text1_Text;
  _searchContext.FilterText = Text2_Text;
  _searchContext.SearchType = ComboBox2.Text;
  ```
- Preserves UI-binding properties (ComboBox1.Text, View.Cursor, List1_Items) in the ViewModel

### 4. Migrated t308 Descendant Search Branch
**Location**: Lines ~2860–2881

The descendant-number validation and parsing path now uses:
- `_searchContext.Message` for the validation prompt message
- `_searchContext.ParseEndIndex` for the loop end (was num8)
- `_searchContext.ParseIndex` for the loop counter (was num7)

Validation:
```csharp
if (Modul1.UbgT.Length / 3.0 != Conversion.Int(Modul1.UbgT.Length / 3.0))
{
	_ = Interaction.MsgBox(_searchContext.Message);  // ← Now from model
	// ...
	goto end_IL_0001_2;
}
else
{
	_searchContext.ParseEndIndex = Modul1.UbgT.Length;
	_searchContext.ParseIndex = 3;
	goto IL_14cb;
}
```

### 5. Migrated IL_14cb Parse Loop
**Location**: Lines ~2916–2932

The descendant-number parsing loop now directly uses `SearchContext` properties:
```csharp
IL_14cb:
	var num9 = _searchContext.ParseIndex;
	var num10 = _searchContext.ParseEndIndex;
	while (_searchContext.ParseIndex <= _searchContext.ParseEndIndex)
	{
		if (Strings.Mid(Modul1.UbgT, _searchContext.ParseIndex, 1) == ".")
		{
			_searchContext.ParseIndex += 3;
		}
		else
		{
			_ = Interaction.MsgBox(_searchContext.Message);  // ← From model
			break;
		}
	}

	string item2;
	if (_searchContext.ParseIndex > _searchContext.ParseEndIndex)
	{
		// ... process descendant list
	}
```

### 6. Migrated Inner Result Loop (t308 Processing)
**Location**: Lines ~2956–2967

The descendant result iteration now uses `_searchContext.ResultCounter`:
```csharp
int num11 = Modul1.Aus[(int)EOutCfg.o13].AsInt();
_searchContext.ResultCounter = 1;
while (_searchContext.ResultCounter <= num11)
{
	iPers = DataModul.DT_DescendentTable.Fields["Pr"].AsInt();
	Modul1.Person_ReadNames(iPers, Modul1.Person);

	item2 = /* formatting logic */;
	List1_Items.Add(new(item2, (-DataModul.DT_DescendentTable.Fields["Nr"].AsInt(), 0, 0)));
	DataModul.DT_DescendentTable.MoveNext();
	_searchContext.ResultCounter++;
}
```

## Other Search Helpers (Unchanged but Compatible)

The following helper methods remain in place and are unmodified from Phase E:
- `ExecutePersonSearch()` (t315)
- `ExecuteNumberSearch()` (t313)
- `ExecuteAliasSearch()` (t319)
- `ExecuteDeathSearch()` (t312)
- `ExecuteBirthSearch()` (t311)
- `ExecuteAncestorSearch()` (t309)
- `ExecuteFreeSearch()` (t316)
- `ExecutePlaceSearch()` (t317)
- `ExecutePhoneticSearch()` (t318)
- `ExecuteSoundExSearch()` (t320)
- `ExecuteLeitSearch()` (t321)

These methods already encapsulate their local work variables and are isolated from the broader `Listfuell()` logic. They remain good candidates for future `SearchContext` adoption if needed, but their current structure is already sound.

## Build Validation
✅ **Build Status**: SUCCESSFUL

The solution compiled without errors or warnings after all Phase G changes.

## Architecture Impact

### Benefit 1: Separation of Concerns
- Search state (message, counters, index bounds) is now grouped in a dedicated model
- ViewModel focuses on UI binding (ComboBox, List1_Items, View.Cursor)
- Model layer handles reusable search logic

### Benefit 2: Testability
- `SearchContext` can be mocked or tested independently
- Parse loop logic can be extracted to a helper method that accepts `SearchContext`
- Message formatting and validation can be unit-tested

### Benefit 3: Maintainability
- Fewer local variables scattered in `Listfuell()`
- Clear intent of shared state (ParseIndex, ParseEndIndex, ResultCounter)
- Message and flag storage is now self-documenting

### Benefit 4: Forward Compatibility
- Descendant helpers can reuse `SearchContext` without reimplementing state
- Future search types can leverage the same pattern

## Scope for Future Work

1. **Extend SearchContext to other search branches** (t309, t316, t317, t318, t320, t321)
   - Each branch has unique loop/parse/counter patterns
   - `SearchContext` can be extended with branch-specific properties if needed, or new methods created for common patterns

2. **Extract parse-loop logic to a helper**
   - The `IL_14cb` descendant-number validation could become a reusable utility method
   - Accept `SearchContext` and return success/failure instead of using `goto`

3. **Migrate helper methods to use SearchContext**
   - `ExecuteDeathSearch()`, `ExecuteBirthSearch()`, etc., already have isolated local variables
   - A future phase could standardize them to call a shared context-based registration method

4. **Decouple UI from ViewModel**
   - Properties like `Text1_Text`, `Text2_Text`, `ComboBox1_Items` should eventually move to a binding model
   - Properties like `List1_Items`, `View.Cursor` are UI-control references and must remain

## Summary

Phase G successfully introduced a model-based search context, reducing the mutable state burden in `Listfuell()`. The t308 descendant-number search flow is now fully migrated to use `SearchContext`, and the entry point initializes the context cleanly. The solution builds successfully, and the architecture is ready for incremental extension to other search types. The codebase is now positioned to move forward with further state extraction or helper decomposition without breaking existing functionality.

---
**Completed**: Phase G Execution
**Status**: ✅ DELIVERED & BUILD VALIDATED
**Next Phase**: Phase H (if needed) — Further helper extraction or UI binding decomposition
