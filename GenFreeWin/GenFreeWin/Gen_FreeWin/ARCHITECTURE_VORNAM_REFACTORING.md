# Vornam ViewModel Refactoring Architecture Documentation

## Overview

This document describes the refactored layered architecture for `VornamViewModel` (Vorname/given name management), decomposing a monolithic 400-line legacy VisualBasic port into clean MVVM layers: Domain Model, Data Service, Use Case, and Thin ViewModel.

## Problem Statement

**Legacy Code Issues:**
- Mixed concerns: UI logic, database access, business validation, presentation all in one class (404 lines)
- Hard to test – heavily coupled to legacy `DataModul`, `Modul1`, and global state
- Difficult to maintain – goto statements, nested switches, complex VB conversion remnants
- Not async-safe – synchronous database calls block the UI thread
- Low discoverability – no clear separation of responsibility

## Target Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                    VornamViewModel (Thin UI)                     │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │ [ObservableProperty] + [RelayCommand]                    │   │
│  │ - SearchResults, CurrentNames, SearchPattern            │   │
│  │ - SearchNamesOnTextChanged() → UseCase                   │   │
│  │ - SaveAllNamesAsync() → UseCase.SaveBatchNamesAsync()   │   │
│  │ - SelectFromSearchResults() → UseCase.GetTextByIdAsync() │   │
│  └──────────────────────────────────────────────────────────┘   │
│                          ↓ delegates                             │
├─────────────────────────────────────────────────────────────────┤
│          VornamSearchUseCase (Business Logic)                    │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │ - LoadNamesAsync(personId, textKennz)                   │   │
│  │ - SearchNamesAsync(textKennz, pattern)                  │   │
│  │ - SaveBatchNamesAsync(personId, kinds, names)           │   │
│  │ - ValidateName(vorname)                                 │   │
│  │ - DeleteNamesByKindAsync(personId, textKennz)           │   │
│  └──────────────────────────────────────────────────────────┘   │
│                          ↓ calls                                 │
├─────────────────────────────────────────────────────────────────┤
│         IVornamDataService (Data Abstraction)                    │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │ async Task<List<VornamModel>> LoadNamesForPersonAsync() │   │
│  │ async Task<short> SaveNameAsync(vorname)                │   │
│  │ async Task<bool> UpdateNameAsync(vorname)               │   │
│  │ async Task<int> DeleteNamesByKindAsync(...)             │   │
│  │ async Task<(string, string)> GetTextByIdAsync(textId)   │   │
│  └──────────────────────────────────────────────────────────┘   │
│                          ↓ implements                            │
├─────────────────────────────────────────────────────────────────┤
│      VornamDataService (Legacy DB Wrapper)                       │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │ Wraps DataModul.DB_NameTable in async Task.Run()        │   │
│  │ Maps legacy IRecordset fields ↔ VornamModel             │   │
│  │ - Direct field indexing avoided (dynamic binder issues)  │   │
│  │ - Uses reflection-based safe field access               │   │
│  └──────────────────────────────────────────────────────────┘   │
│                          ↓ uses                                  │
├─────────────────────────────────────────────────────────────────┤
│           VornamModel (Domain Model)                             │
│  ┌──────────────────────────────────────────────────────────┐   │
│  │ - PersonId: int                                          │   │
│  │ - PrimaryName: string                                   │   │
│  │ - Synonym: string                                       │   │
│  │ - TextKennz: ETextKennz (F_/V_)                          │   │
│  │ - LineNumber: short (sequence)                          │   │
│  │ - IsCalledName, IsNickname: bool                        │   │
│  │ - IsValid(), Clone(), GenerateDisplayText()             │   │
│  └──────────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────────┘
```

## Key Components

### 1. VornamModel (Models/VornamModel.cs)

**Responsibility:** Domain data & validation

```csharp
public class VornamModel
{
	public int PersonId { get; set; }
	public string PrimaryName { get; set; }
	public string Synonym { get; set; }
	public ETextKennz TextKennz { get; set; }
	public short LineNumber { get; set; }
	public bool IsCalledName { get; set; }
	public bool IsNickname { get; set; }

	public bool IsValid() => !string.IsNullOrWhiteSpace(PrimaryName) && ...
	public string GenerateDisplayText() => "Name [marker] (synonym)"
}
```

**Dependencies:**
- GenFree.Data (ETextKennz)
- None on DataModul or UI

**Tests:**
- `VornamModel_ValidatesMinimumLength()`
- `VornamModel_GeneratesDisplayText_WithMarkers()`
- `VornamModel_Clone_CreatesProperlyCopiedInstance()`

---

### 2. IVornamDataService (Services/Interfaces/IVornamDataService.cs)

**Responsibility:** Data persistence contract

```csharp
public interface IVornamDataService
{
	Task<List<VornamModel>> LoadNamesForPersonAsync(int personId, ETextKennz textKennz);
	Task<short> SaveNameAsync(VornamModel vorname);
	Task<bool> UpdateNameAsync(VornamModel vorname);
	Task<int> DeleteNamesByKindAsync(int personId, ETextKennz textKennz);
	Task<ObservableCollection<IListItem<int>>> SearchNamesAsync(ETextKennz textKennz, string searchPattern);
	Task<(string Text, string LeadName)?> GetTextByIdAsync(int textId);
}
```

**Dependencies:**
- VornamModel
- GenFree.Data (ETextKennz, IRecordset)

---

### 3. VornamDataService (Services/VornamDataService.cs)

**Responsibility:** Legacy DataModul wrapper

**Key Pattern:**
```csharp
public async Task<List<VornamModel>> LoadNamesForPersonAsync(int personId, ETextKennz textKennz)
{
	return await Task.Run(() =>
	{
		var names = new List<VornamModel>();
		IRecordset nameTable = DataModul.DB_NameTable;
		nameTable.Index = nameof(NameIndex.NamKenn);
		nameTable.Seek("=", personId, textKennz);

		// Iterate and map each record
		while (!nameTable.EOF && !nameTable.NoMatch)
		{
			var name = MapFromDatabase(nameTable.Fields);
			if (name != null) names.Add(name);
			nameTable.MoveNext();
		}

		return names;
	});
}
```

**Safe Field Access:**
```csharp
private VornamModel MapFromDatabase(object? fields)
{
	if (fields == null) return null;
	try {
		// Reflection-based access avoids CSharp RuntimeBinder issues with dynamic
		return new VornamModel { PersonId = fields.PersNr, ... };
	}
	catch (Exception ex) { return null; }
}
```

**Dependencies:**
- IVornamDataService (implements)
- DataModul (legacy database)
- BaseLib.Helper (extension methods)

---

### 4. VornamSearchUseCase (UseCases/VornamSearchUseCase.cs)

**Responsibility:** Name workflow orchestration

**Key Methods:**

| Method | Input | Output | Purpose |
|--------|-------|--------|---------|
| `LoadNamesAsync` | personId, textKennz | ObservableCollection<VornamModel> | Load all names for person/gender |
| `SearchNamesAsync` | textKennz, pattern | IListItem<int> collection | Autocomplete dropdown search |
| `SaveBatchNamesAsync` | personId, textKennz, names | (Success, SavedCount, Error) | Save all names in transaction-like batch |
| `ValidateName` | VornamModel | (IsValid, ErrorMessage) | Business validation |
| `DeleteNamesByKindAsync` | personId, textKennz | (Success, DeletedCount, Error) | Delete all names of type |

**Transaction Pattern:**
```csharp
public async Task<(bool, int, string?)> SaveBatchNamesAsync(int personId, ETextKennz textKennz, List<VornamModel> names)
{
	// 1. Delete existing names
	await _dataService.DeleteNamesByKindAsync(personId, textKennz);

	// 2. Save new names
	int savedCount = 0;
	foreach (var name in names)
	{
		if (name.IsValid())
		{
			var lineNumber = await _dataService.SaveNameAsync(name);
			if (lineNumber > 0) savedCount++;
		}
	}

	return (true, savedCount, null);
}
```

**Dependencies:**
- IVornamDataService
- VornamModel (validation)
- IModul1 (legacy integration: Modul1.Person_ReadNames, Modul1.Ancesters_GetPersonData)

---

### 5. VornamViewModel (ViewModels/VornamViewModel.cs)

**Responsibility:** Thin UI state & command binding

**Observable Properties:**
```csharp
[ObservableProperty] public partial ObservableCollection<VornamModel> CurrentNames { get; set; }
[ObservableProperty] public partial string SearchPattern { get; set; }
[ObservableProperty] public partial bool IsLoading { get; set; }
```

**Relay Commands:**
```csharp
[RelayCommand] public async Task SearchNamesOnTextChanged(string searchText) => ...
[RelayCommand] public async Task SelectFromSearchResults(IListItem<int> selectedItem) => ...
[RelayCommand] public async Task SaveAllNamesAsync() => ...
[RelayCommand] public void CancelEdit() => ...
```

**Event Handlers (Legacy Compat):**
```csharp
public void Form_Load(object eventSender, EventArgs eventArgs) { ... }
```

**Dependencies:**
- IVornamDataService (injected)
- VornamSearchUseCase (injected)
- Vornam View (legacy WinForms, accessed via property)
- IModul1 (legacy integration)

---

## Data Flow Example: Save All Names

```
User clicks "Save" button
		 ↓
VornamViewModel.SaveAllNamesAsync()
  1. Collects VornamModel entries from form fields
  2. Validates each name (VornamModel.IsValid())
  3. Calls use case:
	 UseCase.SaveBatchNamesAsync(personId, textKennz, names)
		 ↓
	 VornamSearchUseCase.SaveBatchNamesAsync
	 1. Calls IVornamDataService.DeleteNamesByKindAsync()
	 2. For each name, calls IVornamDataService.SaveNameAsync()
	 3. Returns (Success, SavedCount, Error)
		 ↓
	 VornamDataService.DeleteNamesByKindAsync/SaveNameAsync
	 Each wraps legacy DataModul.DB_NameTable in Task.Run()
		 ↓
	 Legacy database update
		 ↓
  4. ViewModel updates:
	 - Refreshes Modul1.Person_ReadNames
	 - Recomputes ancestor data
	 - Shows duplicates if configured
	 - Closes form
```

## Testing Strategy

### Unit Tests (VornamModel)

```csharp
[TestMethod]
public void VornamModel_IsValid_ReturnsFalse_For_EmptyName()
{
	var model = new VornamModel { PersonId = 1, PrimaryName = "" };
	Assert.IsFalse(model.IsValid());
}

[TestMethod]
public void VornamModel_GenerateDisplayText_IncludesMarkers_ForCalledNameAndNickname()
{
	var model = new VornamModel 
	{ 
		PrimaryName = "Johann", 
		IsCalledName = true, 
		IsNickname = true,
		Synonym = "Johann the Great"
	};
	var display = model.GenerateDisplayText();
	Assert.IsTrue(display.Contains("[R★]"));
	Assert.IsTrue(display.Contains("(Johann the Great)"));
}
```

### Integration Tests (VornamSearchUseCase)

```csharp
[TestMethod]
public async Task VornamSearchUseCase_SaveBatchNamesAsync_PersistsValidNames_AndDeletesOldOnes()
{
	var useCase = new VornamSearchUseCase(_mockDataService.Object);
	var names = new List<VornamModel> { new VornamModel { ... } };

	var result = await useCase.SaveBatchNamesAsync(1, ETextKennz.F_, names);

	Assert.IsTrue(result.Success);
	_mockDataService.Verify(x => x.DeleteNamesByKindAsync(1, ETextKennz.F_), Times.Once);
	_mockDataService.Verify(x => x.SaveNameAsync(It.IsAny<VornamModel>()), Times.AtLeastOnce);
}
```

### Behavioral Tests (VornamDataService)

```csharp
[TestMethod]
public async Task VornamDataService_LoadNamesForPersonAsync_ReturnsEmptyList_IfNamesNotFound()
{
	var service = new VornamDataService();
	var result = await service.LoadNamesForPersonAsync(999999, ETextKennz.F_);

	Assert.IsNotNull(result);
	Assert.AreEqual(0, result.Count);
}
```

---

## Dependency Injection Setup

### Registration (Startup/DI Container)

```csharp
// Services
serviceCollection.AddScoped<IVornamDataService, VornamDataService>();

// Use Cases
serviceCollection.AddScoped<VornamSearchUseCase>();

// ViewModels
serviceCollection.AddScoped<VornamViewModel>();
```

### Constructor Injection

```csharp
public VornamViewModel(IVornamDataService dataService, VornamSearchUseCase searchUseCase)
{
	_dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
	_searchUseCase = searchUseCase ?? throw new ArgumentNullException(nameof(searchUseCase));
}
```

---

## Migration Path from Legacy Code

### Step 1: Replace Synchronous Calls with Async
```csharp
// Old
void Befehl_Click(...) { DataModul.DB_NameTable.delete(); ... }

// New
[RelayCommand] public async Task DeleteAllNamesAsync() { ... }
```

### Step 2: Extract Business Logic to UseCase
```csharp
// Old code inline in ViewModel
if (name.Length < 2) throw error;

// New – UseCase.ValidateName()
var (isValid, error) = _searchUseCase.ValidateName(name);
```

### Step 3: Inject Services Instead of Static Access
```csharp
// Old
var data = DataModul.DB_NameTable;

// New
var data = _dataService.LoadNamesForPersonAsync(...);
```

### Step 4: Replace goto/while Loops with Clear Control Flow
```csharp
// Old (from legacy VB)
IL_00f5:
  if (condition) goto end;
  // ... complex logic
  goto IL_00f5;

// New (modern C#)
public async Task SaveAllNamesAsync()
{
	var names = CollectNames();
	await _searchUseCase.SaveBatchNamesAsync(personId, textKennz, names);
	RefreshUI();
}
```

---

## Best Practices Applied

1. **Single Responsibility:** Each class has one reason to change
   - VornamModel: validation/display logic
   - VornamDataService: database mapping
   - VornamSearchUseCase: business workflows
   - VornamViewModel: UI state & binding

2. **Dependency Inversion:** Inject IVornamDataService, not DataModul directly

3. **Async/Await Over Blocking:** All I/O wrapped in Task.Run()

4. **Null Safety:** Defensive checks, null-conditional operators (?.)

5. **Clear Naming:** Methods named for intent (SearchNamesOnTextChanged vs. Text1_Changed)

6. **Testability:** Each layer mockable independently

---

## Known Limitations & Future Work

1. **Legacy Coupling:** IModul1 and Modul1 remain global – planned for future refactor
2. **Complex Master-Detail:** Name aliases/lead names stored in non-normalized way (Modul1.Ubg, Modul1.UbgT state) – consider refactor to explicit ViewModel state
3. **Static MainProject.Forms.Vornam References:** Tight coupling in Vornam_Textzeig – consider extracting to injectable service
4. **Transaction Semantics:** SaveBatchNamesAsync mimics transaction but is not truly atomic – may need wrapping in real database transaction
5. **Error Handling:** Current logging via Debug.WriteLine – consider ILogger injection

---

## Summary

The refactored **VornamViewModel** architecture separates concerns into a clean, testable, maintainable stack:
- **Domain (VornamModel):** Data + basic validation
- **Data (IVornamDataService + VornamDataService):** Persistence abstraction
- **Logic (VornamSearchUseCase):** Workflows + business rules
- **UI (VornamViewModel):** MVVM binding + commands

The design maintains backward compatibility with legacy DataModul and Modul1 while enabling incremental modernization of the genealogy application.
