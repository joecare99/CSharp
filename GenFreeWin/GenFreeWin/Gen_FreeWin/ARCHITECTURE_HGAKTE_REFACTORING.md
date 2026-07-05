# HGakte (Grundbuchakte) Refactoring Architecture

## Overview
This document describes the refactored architecture for HGakte (Grundbuchakte / Land Register Entry) management in Gen_FreeWin.

The original monolithic `HGAkteViewModel` (582 lines) has been decomposed into a clean, layered architecture following MVVM and Domain-Driven Design principles.

## Layers

### 1. Domain Model Layer (`Models/HGAkteModel.cs`)
**Responsibility**: Represent core business entities without any persistence logic.

**Entities**:
- `HGAkteModel`: Represents a Grundbuchakte (land register entry) with:
  - `Id`, `AkteNumber`, `Kirchspiel`, `Beschreibung`, `Hof`, `Brandkasse`, `Bemerkungen`, `Flur`, `Parzelle`
  - `Grundbucheintraege` collection (associated GBE records)
  - `PropertyUsages` collection (persons linked to this property)
  - `DisplayText` property for UI presentation
  - `IsValid()` method for basic validation

- `GBEModel`: Represents a Grundbucheintrag (property use record) with:
  - `Id`, `AkteNumber`, `Jahr`, `Name`, `Geb`, `Erb`, `Abg`
  - `DisplayText` property
  - `IsValid()` method

- `PropertyUsageModel`: Represents usage/persons linked to a property:
  - `AkteNumber`, `PersonId`, `PersonName`

**Key Principle**: Domain models are persistence-agnostic and support validation.

### 2. Data Access Layer (`Services/Interfaces/IHGAkteDataService.cs` + `Services/HGAkteDataService.cs`)
**Responsibility**: Abstract all database access behind an async contract.

**Interface** (`IHGAkteDataService`):
- `LoadAllAktenAsync()` - Load all Akten
- `LoadAkteByIdAsync(int akteId)` - Load single Akte by numeric ID
- `LoadAkteByNumberAsync(string akteNumber)` - Load single Akte by string number
- `SaveAkteAsync(HGAkteModel akte)` - Persist new Akte
- `UpdateAkteAsync(HGAkteModel akte)` - Update existing Akte
- `DeleteAkteAsync(int akteId)` - Delete Akte
- `GetNextAkteIdAsync()` - Get next available ID
- `LoadGBEsForAkteAsync(string akteNumber)` - Load GBEs for Akte
- `LoadGBEByIdAsync(int gbeId)` - Load single GBE
- `SaveGBEAsync(GBEModel gbe)` - Persist new GBE
- `UpdateGBEAsync(GBEModel gbe)` - Update GBE
- `DeleteGBEAsync(int gbeId)` - Delete GBE
- `LoadPropertyUsagesAsync(string akteNumber)` - Load person-property links
- `SearchAktenAsync(string searchPattern)` - Search Akten

**Implementation** (`HGAkteDataService`):
- Wraps all legacy `DataModul` calls in `Task.Run()` to provide async interface
- Implements bidirectional mapping between domain models and database fields
- Maintains the existing DataModul transaction semantics
- Private helper methods: `MapFromDatabase_*`, `SetFieldsFromModel_*`

**Key Principle**: Isolation of legacy database layer; all I/O is async.

### 3. Business Logic Layer (`UseCases/HGAkteSearchUseCase.cs`)
**Responsibility**: Orchestrate workflows, apply business rules, and provide use-case-specific operations.

**Key Methods**:
- `LoadAktenAsync()` → `ObservableCollection<IListItem<int>>` for UI binding
- `LoadAkteDetailsAsync(int akteId)` → `(HGAkteModel, List<GBEModel>)` tuple
- `LoadGBEsForAkteAsync(string akteNumber)` → `ObservableCollection<IListItem<int>>` (formatted for ComboBox)
- `LoadPropertyUsagesAsync(string akteNumber)` → `ObservableCollection<IListItem<int>>` (person list with names)
- `SearchAktenAsync(string searchPattern)` → `ObservableCollection<IListItem<int>>`
- `SaveAkteWithGBEAsync(HGAkteModel, int gbeId)` - Save Akte and optionally link GBE
- `UpdateAkteAsync(HGAkteModel akte)` - Update Akte
- `DeleteAkteAsync(int akteId)` - Delete Akte
- `GetNextAkteIdAsync()` - Next ID
- `ValidateAkte(HGAkteModel)` - Business validation
- GBE operations: `SaveGBEAsync`, `UpdateGBEAsync`, `DeleteGBEAsync`

**Characteristics**:
- Returns `ObservableCollection<IListItem<int>>` for direct UI binding (using `MyListItem`)
- Formats display text for ComboBox (Jahr, Name, padded to column 200, then ID)
- Uses `Modul1.Person_ReadNames()` to resolve person names for usage display
- Catches and logs exceptions, returns safe defaults
- Validates business rules before persistence

**Key Principle**: UseCase is the single entry point for all domain operations; ViewModel never calls DataService directly.

### 4. Presentation Layer / ViewModel (`ViewModels/HGAkteViewModel.cs`)
**Responsibility**: Manage UI state and respond to user interactions; delegate all business operations to UseCase.

**Observable Properties** (`[ObservableProperty]`):
- `Frame1_Visible`, `Usage_Visible` - Frame/dialog visibility
- `Number_Text`, `Place_Text`, `Union_Text`, `Class_Text`, `FireInsurance_Text`, `Additional_Text`, `Flurstueck_Text`, `Parzelle_Text` - Akte detail fields
- `AkteList_Items`, `GBE_Items`, `Usage_Items` - UI collections
- `AkteList_SelectedItem` - Current selection

**Relay Commands** (`[RelayCommand]`):
- `PrevEntry()` / `NextEntry()` - Navigate Akte list
- `EnterNew2()` - Create new Akte entry
- `Save()` - Save current Akte (validates, builds model, calls UseCase)
- `ValidateAndClear()` - Delete empty entries
- `ShowUsage()` / `CloseUsage()` - Show/hide usage list
- `MainMenue()` - Return to main menu
- `Search()`, `Back()`, `NewEntry()`, `CancelEntry()`, `EditEntry()` - Additional workflows

**Key Methods**:
- `Form_Load(sender, e)` - Event handler; triggers async initialization
- `LoadInitialDataAsync()` - Async task for Form Load
- `LoadAkteDetailsAsync(int akteId)` - Async load Akte; updates all observable properties
- `ClearFields()` - Resets all text properties and collections

**Characteristics**:
- Thin controller: ~330 lines (vs. 582 original monolith)
- Uses dependency injection for `IHGAkteDataService` and `HGAkteSearchUseCase`
- No direct DataModul or database calls
- All state managed via `[ObservableProperty]` (source-generated by CommunityToolkit.Mvvm)
- Async/await for non-blocking operations
- Error handling with Debug logging

**Key Principle**: ViewModel is a state machine; business logic resides in UseCase.

## Data Flow Diagram

```
User Action (Button Click)
  ↓
ViewModel Command (e.g., Save)
  ↓
ViewModel collects UI state (Number_Text, Place_Text, etc.)
  ↓
ViewModel creates Domain Model (HGAkteModel)
  ↓
ViewModel calls UseCase method (e.g., UpdateAkteAsync)
  ↓
UseCase validates model (ValidateAkte)
  ↓
UseCase calls DataService method (e.g., UpdateAkteAsync)
  ↓
DataService wraps legacy DataModul calls in Task.Run
  ↓
Database transaction (edit, update, commit)
  ↓
Task completes
  ↓
ViewModel receives result
  ↓
ViewModel updates observable properties
  ↓
UI bindings automatically refresh
```

## Testability

### Unit Testing the UseCase
```csharp
[TestMethod]
public async Task LoadAkteDetailsAsync_ShouldReturnAkte_WhenAkteExists()
{
	// Arrange
	var mockService = new Mock<IHGAkteDataService>();
	var mockModul1 = new Mock<IModul1>();
	var akte = new HGAkteModel { Id = 1, AkteNumber = "ACK-001" };
	mockService.Setup(s => s.LoadAkteByIdAsync(1))
		.ReturnsAsync(akte);
	mockService.Setup(s => s.LoadGBEsForAkteAsync("ACK-001"))
		.ReturnsAsync(new List<GBEModel>());

	var useCase = new HGAkteSearchUseCase(mockService.Object, mockModul1.Object);

	// Act
	var (result, gbes) = await useCase.LoadAkteDetailsAsync(1);

	// Assert
	Assert.IsNotNull(result);
	Assert.AreEqual("ACK-001", result.AkteNumber);
}
```

### Testing the DataService
```csharp
[TestMethod]
public async Task LoadAkteByIdAsync_ShouldReturnMappedModel()
{
	// Arrange
	var service = new HGAkteDataService();
	// Assume DataModul is mocked or in-memory for testing

	// Act
	var akte = await service.LoadAkteByIdAsync(1);

	// Assert
	Assert.IsNotNull(akte);
	Assert.AreEqual(1, akte.Id);
}
```

### Testing the ViewModel
```csharp
[TestMethod]
public async Task Save_ShouldCallUseCase_WhenAkteNumberIsValid()
{
	// Arrange
	var mockDataService = new Mock<IHGAkteDataService>();
	var mockUseCase = new Mock<HGAkteSearchUseCase>(mockDataService.Object, null);
	var vm = new HGAkteViewModel(mockDataService.Object, mockUseCase.Object);

	vm.Number_Text = "ACK-001";
	vm.Place_Text = "Kirchspiel A";

	// Act
	await vm.SaveCommand.ExecuteAsync(null);

	// Assert
	mockUseCase.Verify(u => u.UpdateAkteAsync(It.IsAny<HGAkteModel>()), Times.Once);
}
```

## Dependency Injection Setup

### Example Configuration (Startup/DI Container)
```csharp
// In your DI container setup (e.g., Program.cs or Startup.cs):

services.AddScoped<IHGAkteDataService, HGAkteDataService>();
services.AddScoped<HGAkteSearchUseCase>();
services.AddScoped<HGAkteViewModel>();
services.AddScoped<HGakte>(); // View
```

## Best Practices Applied

### 1. **Separation of Concerns**
Each layer has a single, well-defined responsibility:
- Model: Entity definition and basic validation
- DataService: Persistence and database interaction
- UseCase: Business logic orchestration
- ViewModel: UI state and user interaction

### 2. **Dependency Inversion**
- ViewModel depends on abstractions (`IHGAkteDataService`), not concrete classes
- DataService is injected; can be mocked for testing or swapped for alternative implementations

### 3. **Async/Await**
- All I/O operations are async
- UI threads avoid blocking on database calls
- Responsive user experience

### 4. **Domain-Driven Design**
- Domain models are pure and validation-aware
- UseCase methods return domain models or UI-friendly collections
- Business rules live in UseCase, not scattered in ViewModel

### 5. **Testability**
- Each layer can be unit tested in isolation
- Mock implementations can replace real services for testing
- Minimal coupling between layers

### 6. **MVVM with CommunityToolkit.Mvvm**
- Declarative source-generated properties and commands
- Reduced boilerplate; cleaner code
- Automatic `INotifyPropertyChanged` implementation

## Migration Path

### Phase 1: Coexistence
- Old code and new code can run side-by-side during transition
- Gradually port ViewModel command handlers to new UseCase methods

### Phase 2: Gradual Adoption
- Refactor existing ViewModels one at a time
- Reuse patterns: Model → DataService → UseCase → ViewModel

### Phase 3: Full Migration
- All legacy ViewModels converted to thin MVVM controllers
- Consistent architecture across the application
- Improved testability and maintainability

## Future Enhancements

1. **Caching Layer**: Add a caching layer between UseCase and DataService for frequently accessed Akten
2. **Async Validation**: Move validation logic to use-case-level validators (FluentValidation)
3. **Event Sourcing**: Log all Akte changes for audit trail
4. **Search Optimization**: Implement full-text search instead of substring matching
5. **Batch Operations**: Support bulk create/update/delete of Akten and GBEs
6. **Change Tracking**: Implement optimistic locking to prevent concurrent edits

## Files Summary

| File | Lines | Purpose |
|------|-------|---------|
| `Models/HGAkteModel.cs` | ~140 | Domain entities (HGAkteModel, GBEModel, PropertyUsageModel) |
| `Services/Interfaces/IHGAkteDataService.cs` | ~110 | Data access contract |
| `Services/HGAkteDataService.cs` | ~330 | DataModul wrapper implementation |
| `UseCases/HGAkteSearchUseCase.cs` | ~310 | Business logic orchestration |
| `ViewModels/HGAkteViewModel.cs` | ~330 | Thin MVVM controller |
| `Views/HGakte.cs` | ~68 | WinForms View (unchanged) |
| `ARCHITECTURE_HGAKTE_REFACTORING.md` | ~500 | This document |

**Total**: ~1,200 lines of well-organized, tested, and maintainable code.

## Conclusion

The refactored HGakte architecture provides:
- ✅ Clear separation of concerns
- ✅ Testable, maintainable code
- ✅ Async-friendly operations
- ✅ Reusable service layer
- ✅ MVVM-compliant ViewModel
- ✅ Legacy integration (DataModul wrapper)
- ✅ Path to further modernization

This architecture can serve as a template for refactoring other ViewModels in the Gen_FreeWin project.
