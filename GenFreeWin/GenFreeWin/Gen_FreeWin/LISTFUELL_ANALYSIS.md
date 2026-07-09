# Listfuell() Analysis - Phase E Step 1

## Method Size & Scope
- **Lines:** 2861-3237 = ~376 lines
- **Complexity:** HIGH (if/else-if chain with 17+ branches)
- **Current responsibility:** List population based on search mode + filter logic

## Breakdown by Branch (ComboBox2.Text):

### 1. **t314 (Persuch - Personal Name Search)**
- Lines 2899-2911 (~12 lines)
- Sets index to "Persuch", seeks, calls `Zeigfam()` or `Zeig()`
- LOW complexity

### 2. **t315 (Persuchdat - Personal Date Search)**
- Lines 2913-2918 (~5 lines)
- Sets index to "Persuch", seeks, calls `Zeigfamdat()`
- LOW complexity

### 3. **t313 (Nummer - Person Number Search)**
- Lines 2920-2936 (~16 lines)
- Index "Nummer", seeks, calls `Zeigfam()` or `Zeig()`
- LOW complexity

### 4. **t312 (Death/Burial Date Search)**
- Lines 2938-2993 (~55 lines)
- **LARGE**: Iterates `DataModul.Event.ReadAllGt()` for death/burial events
- Filters by gender (`Female2_Checked`, `Male2_Checked`)
- Constructs formatted output
- HIGH complexity - **good extraction candidate**

### 5. **t311 (Birth/Baptism Date Search)**
- Lines 2995-3054 (~59 lines)
- **LARGE**: Iterates events for birth/baptism
- Similar structure to t312
- HIGH complexity - **good extraction candidate**

### 6. **t319 (Aliassuch - Alias Search)**
- Lines 3056-3068 (~12 lines)
- Simple seek + display
- LOW complexity

### 7. **t308 (Descendant Number Search)**
- Lines 3070-3091 (~21 lines)
- Validates descendant number format
- Sets up for IL_14cb label
- MEDIUM complexity

### 8. **t309 (Ancestor Search)**
- Lines 3093-3109 (~16 lines)
- Searches ancestor table
- Iterates and adds to list
- MEDIUM complexity

### 9. **t316, t317 (Family Date Searches)**
- Lines 3111-3130 (~19 lines)
- Calls `Zeigfamanl()` / `Zeigfamanl2()`
- LOW complexity - UI delegation

### 10. **t318, t320, t321 (Phonetic/Sound/Lead Searches)**
- Lines 3132-3172 (~40 lines)
- Phonetic/SoundEx/Lead searches
- MEDIUM complexity

### 11. **IL_14cb (Descendant processing)**
- Lines 3177-3230 (~53 lines)
- Large descendant iteration loop
- HIGH complexity

## Dependencies
- `DataModul` (global data access)
- `Modul1` (global module1)
- `Strings` (VB legacy)
- `List1_Items` (ObservableCollection)
- `View.Cursor` (UI manipulation)
- `Text1_Text`, `Text2_Text`, `ComboBox2.Text` (UI state)
- `OmitSpouse_Checked`, `Female2_Checked`, `Male2_Checked` (filter state)
- Helper methods: `Zeigfam()`, `Zeig()`, `Zeigfamdat()`, `Zeigfamanl()`, `Zeigfamanl2()`, `Listleer()`, `Place_ReadData()`

## Extraction Strategy

### Option A: Big Bang (Risk: HIGH)
Extract entire method to single `ListFillHandler` class. Problems:
- 17 branches difficult to test
- Deep dependencies on global state
- goto statements hard to untangle
- Single point of failure

### Option B: Incremental (Risk: LOW) ← RECOMMENDED
Extract by **search mode** (branch), not entire method:
1. Create `ListFillHandler` with dispatch method
2. Extract each `tXXX` mode into its own handler method
3. Update `Listfuell()` to call `_handler.HandleSearchMode(mode)`
4. Each mode extracted independently and testable

Examples:
```csharp
// In ListFillHandler
public void HandleDeathSearch(string text, bool femaleChecked, bool maleChecked) { ... }
public void HandleBirthSearch(string text, bool femaleChecked, bool maleChecked) { ... }
public void HandlePersonNameSearch(string text, bool omitSpouse) { ... }
// etc.
```

Then in ViewModel:
```csharp
public void Listfuell()
{
	if (ComboBox1.Text == "") { ... return; }

	string mode = ComboBox2.Text;
	if (mode == Modul1.IText[EUserText.t312])
		_handler.HandleDeathSearch(Text1_Text, Female2_Checked, Male2_Checked);
	else if (mode == Modul1.IText[EUserText.t311])
		_handler.HandleBirthSearch(...);
	// ... etc
}
```

### Option C: Hybrid
Extract **only the largest branches** (t312, t311, IL_14cb) first, keep rest inline. Then iterate. Less risky but slower.

## Recommendation for Phase E
**Start with Option B - Incremental:**
1. Create skeleton `ListFillHandler` class
2. Extract the 3-4 largest branches (t312, t311, t309-descendants) as first pass
3. Validate compilation and UI behavior
4. Extract remaining branches in follow-up
5. Eventually refactor `Listleer()` and helper methods

This keeps risk low while making progress on ViewModel size reduction.

## Code Blocks Ready for Extraction

**Death/Burial Search (t312):** Lines 2938-2993 - ~55 lines
**Birth/Baptism Search (t311):** Lines 2995-3054 - ~59 lines
**Descendant Search (t309 + IL_14cb):** Lines 3093-3230 - ~137 lines

**Total from 3 branches:** ~251 lines (67% of method)
**Remaining:** ~125 lines (simple dispatch, validation, early exits)

This aligns with realistic Phase E scope: extract 250+ lines, test, iterate.
