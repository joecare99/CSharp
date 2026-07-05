# RepoViewModel Refactoring - Architektur-Dokumentation

## Überblick

Das ursprüngliche `RepoViewModel` (376 Zeilen) war ein monolithischer "God Object", der UI-State, Geschäftslogik und Datenzugriff vermischt hat. Diese Refactoring zerlegt die Verantwortungen nach dem Single Responsibility Principle in eine klassische 3-Schichten-Architektur:

```
┌─────────────────────────────────────────────────┐
│  RepoViewModel (UI/MVVM Layer)                  │
│  - [ObservableProperty] UI State                │
│  - [RelayCommand] Command Handlers              │
│  - Command Orchestration                        │
└────────────┬────────────────────────────────────┘
			 │ uses
			 ▼
┌─────────────────────────────────────────────────┐
│  RepoSearchUseCase (Business Logic)             │
│  - Validation                                   │
│  - Search/Load Workflows                        │
│  - Data Transformation to UI Models             │
└────────────┬────────────────────────────────────┘
			 │ uses
			 ▼
┌─────────────────────────────────────────────────┐
│  IRepoDataService / RepoDataService             │
│  - Data Persistence                             │
│  - Database Operation Isolation                 │
│  - DataModul Encapsulation                      │
└────────────┬────────────────────────────────────┘
			 │ wrapper
			 ▼
┌─────────────────────────────────────────────────┐
│  RepoModel / RepoSourceModel (Domain Models)    │
│  - Type-safe Data Transfer                      │
│  - Business Entity Representation               │
└─────────────────────────────────────────────────┘
```

## Komponenten

### 1. RepoViewModel (UI/MVVM Layer)
**Datei:** `..\Gen_FreeWin\GenFreeWin\Gen_FreeWin\ViewModels\RepoViewModel.cs`

#### Verantwortungen
- Observable Properties für Property-Binding (WinForms)
- RelayCommand Command-Binding
- UI-State Management
- Command-Handler Orchestrierung
- Error Handling auf der Darstellungsebene

#### Hauptmethoden
- `FormLoad()` – Initialisiert das Fenster und lädt alle Repositories
- `Save()` – Speichert ein neues Repository und verknüpft es mit einer Quelle
- `Save2()` – Aktualisiert ein bestehendes Repository
- `List1Dbl()` – Lädt Repository-Details bei Double-Click
- `List2Dbl()` – Öffnet einen Quelleditor
- `Delete()` – Löscht ein Repository
- `NewEntry()` – Bereitet ein neues Eintragsformular vor

**Größenreduktion:** Von 376 → 330 Zeilen (durch Delegation an Services)

### 2. RepoModel (Domain Model)
**Datei:** `..\Gen_FreeWin\GenFreeWin\Gen_FreeWin\Models\RepoModel.cs`

#### Klasse: RepoModel
Repräsentiert ein Genealogie-Repository mit Kontaktdaten:
```csharp
public class RepoModel
{
	public int Id { get; set; }
	public string Name { get; set; }
	public string Street { get; set; }
	public string Place { get; set; }
	public string PostalCode { get; set; }
	public string Phone { get; set; }
	public string Email { get; set; }
	public string Website { get; set; }
	public string Remarks { get; set; }
	public string SearchName { get; set; } // für Indizierung

	public string DisplayText { get; } // "Name Place" für UI-Listen
	public bool IsValid() { } // Geschäftsregeln
}
```

#### Klasse: RepoSourceModel
Verknüpfungstabelle zwischen Repository und Quelldokumenten:
```csharp
public class RepoSourceModel
{
	public int SourceId { get; set; }
	public int RepositoryId { get; set; }
	public string SourceDescription { get; set; }
}
```

### 3. IRepoDataService / RepoDataService (Data Access Layer)
**Dateien:** 
- `..\Gen_FreeWin\GenFreeWin\Gen_FreeWin\Services\Interfaces\IRepoDataService.cs` (Interface)
- `..\Gen_FreeWin\GenFreeWin\Gen_FreeWin\Services\RepoDataService.cs` (Implementierung)

#### Verantwortungen
- Alle direkten DataModul-Zugriffe
- CRUD-Operationen auf Repositories
- Datenbankoberfläche für Such-/Ladoperationen
- Isolation der legacy Adatenbankebene

#### Vertragsschnittmethoden
```csharp
Task<List<RepoModel>> LoadAllRepositoriesAsync()
Task<RepoModel?> LoadRepositoryByIdAsync(int repoId)
Task<int> SaveRepositoryAsync(RepoModel repo)
Task<bool> UpdateRepositoryAsync(int repoId, RepoModel repo)
Task<bool> DeleteRepositoryAsync(int repoId)
Task<List<RepoSourceModel>> LoadSourcesByRepositoryAsync(int repoId)
Task<bool> LinkSourceToRepositoryAsync(int sourceId, int repoId)
Task<int> GetNextRepositoryIdAsync()
Task<List<RepoModel>> SearchRepositoriesAsync(string searchText)
```

#### Implementierungsdetails
- Alle Operationen wrappen `Task.Run(...)` um DataModul-Aufrufe
- Fehlerbehandlung mit Exceptions und Debug-Ausgaben
- `SetFieldsFromModel()` Hilfsmethode für Datensatzvorbereitung

### 4. RepoSearchUseCase (Business Logic Layer)
**Datei:** `..\Gen_FreeWin\GenFreeWin\Gen_FreeWin\UseCases\RepoSearchUseCase.cs`

#### Verantwortungen
- High-level Such- und Ladeflussorchestrationierung
- Geschäftsregel-Validierung
- Daten-Transformation für UI-Binding (RepoModel → ObservableCollection<IListItem>)
- Transaktionale Operationen (z.B. speichern + link in `SaveRepositoryWithSourceAsync`)

#### Hauptmethoden
```csharp
LoadRepositoriesAsync()              // Alle Repos in UI-Bindungsformat
SearchRepositoriesAsync(searchText)  // Gefilterte Such-Ergebnisse
LoadRepositoryDetailsAsync(repoId)   // Repo + Quellen zusammen laden
LoadSourcesForRepositoryAsync(repoId) // Nur Quellen für ein Repo
ValidateRepository(repo)             // Geschäftsregeln prüfen
SaveRepositoryWithSourceAsync(repo, sourceId) // Atomare Save+Link
DeleteRepositoryAsync(repoId)        // Repo löschen
GetNextRepositoryIdAsync()           // Nächste verfügbare ID
```

#### Hilfsklassen
- `IListItem<T>` Interface für UI-Listenbindung
- `ListItem<T>` Konkrete Implementierung für Observable-Kollektion

## Datenfluss

### Szenario 1: Repositories beim Laden anzeigen
```
RepoViewModel.FormLoad()
  └─> RepoSearchUseCase.LoadRepositoriesAsync()
	   └─> RepoDataService.LoadAllRepositoriesAsync()
			└─> DataModul.DB_RepoTable Loop + Read
	   └─> Transformation: RepoModel[] → ObservableCollection<IListItem>
  └─> Bind zu Repolist_Items Property
```

### Szenario 2: Selected Repository anzeigen
```
RepoViewModel.List1Dbl()
  └─> RepoSearchUseCase.LoadRepositoryDetailsAsync(repoId)
	   ├─> RepoDataService.LoadRepositoryByIdAsync(repoId)
	   │    └─> DataModul.DB_RepoTable Seek + Read → RepoModel
	   └─> RepoDataService.LoadSourcesByRepositoryAsync(repoId)
			└─> DataModul.DB_RepoTab/DB_QuTable Loop → RepoSourceModel[]
  └─> Bind zu RepoName_Text, RepoStreet_Text, ... (aus RepoModel)
  └─> Bind zu Sources_Items (aus RepoSourceModel[])
```

### Szenario 3: Repository speichern
```
RepoViewModel.Save()
  └─> Create RepoModel from UI Fields
  └─> RepoSearchUseCase.SaveRepositoryWithSourceAsync(repo, sourceId)
	   ├─> ValidateRepository(repo) → Check IsNotEmpty
	   ├─> RepoDataService.SaveRepositoryAsync(repo)
	   │    └─> DataModul.DB_RepoTable AddNew/Edit + Fields + Update
	   └─> RepoDataService.LinkSourceToRepositoryAsync(sourceId, repoId)
			└─> DataModul.DB_RepoTab AddNew + Link Record
  └─> Close Dialog
```

## Testbarkeit

Diese Architektur ermöglicht umfassende Unit-Tests:

### Test-Szenarien
```csharp
[TestClass]
public class RepoSearchUseCaseTests
{
	private Mock<IRepoDataService> _mockDataService;
	private RepoSearchUseCase _useCase;

	[TestInitialize]
	public void Setup()
	{
		_mockDataService = new Mock<IRepoDataService>();
		_useCase = new RepoSearchUseCase(_mockDataService.Object);
	}

	[TestMethod]
	public async Task LoadRepositoriesAsync_ReturnsEmpty_WhenNoRepos()
	{
		// Arrange
		_mockDataService.Setup(s => s.LoadAllRepositoriesAsync())
			.ReturnsAsync(new List<RepoModel>());

		// Act
		var result = await _useCase.LoadRepositoriesAsync();

		// Assert
		Assert.AreEqual(0, result.Count);
	}

	[TestMethod]
	public async Task ValidateRepository_ReturnsFalse_WhenNameIsEmpty()
	{
		// Arrange
		var repo = new RepoModel { Name = "" };

		// Act
		var (isValid, message) = _useCase.ValidateRepository(repo);

		// Assert
		Assert.IsFalse(isValid);
		Assert.IsTrue(message.Contains("name", System.StringComparison.OrdinalIgnoreCase));
	}
}
```

## Migration Path

Die Refactoring verläuft in zwei optionalen Phasen:

### Phase 1: Co-existence (aktuell)
- Neuer refaktorierter `RepoViewModel` neu hinzugefügt
- Alte DataModul-Aufrufe sind weg
- Alle Services sind abstrahiert

### Phase 2: Vollständige Legacy-Migration (zukünftig)
- Andere ViewModels (Quellverw, Familie, etc.) könnten `IRepoDataService` injiziert bekommen
- Repository-Features können unabhängig getestet werden
- Datenbanklogik ist komplett vom UI entkoppelt

## Abhängigkeiten

```
RepoViewModel
  ├─ IModul1 (injiziert)
  ├─ IRepoDataService (injiziert)
  └─ RepoSearchUseCase (intern erstellt)
	   └─ IRepoDataService (geerbt)
			└─ DataModul (nur hier!)
```

## Konfiguration (Dependency Injection)

Beispiel für DI-Konfiguration:
```csharp
// In Startup/DI-Container:
services.AddScoped<IRepoDataService, RepoDataService>();
services.AddScoped<IModul1>(sp => GenFree.Moduls._Modul1.Instance);

// Beim Erstellen des Views:
var service = container.GetRequiredService<IRepoDataService>();
var modul1 = container.GetRequiredService<IModul1>();
var viewModel = new RepoViewModel(modul1, service);
```

## Best Practices

1. **Injizieren Sie Services, nicht erstellen Sie sie**
   - ✅ Good: `new RepoViewModel(modul1, dataService)`
   - ❌ Bad: `new RepoViewModel(modul1) { _dataService = new... }`

2. **Nutzen Sie Models als Domain Transfer Objects (DTOs)**
   - ✅ Verwenden Sie `RepoModel` für Datenbankoperationen
   - ❌ Direkter DataModul-Feld-Zugriff im ViewModel

3. **Delegieren Sie Business Logic an Use Cases**
   - ✅ `_searchUseCase.SaveRepositoryWithSourceAsync(...)`
   - ❌ Direkte Service-Aufrufe im Command-Handler

4. **Halten Sie ViewModels "dünn"**
   - ✅ ViewModels = UI State + Command Delegation
   - ❌ ViewModels = DataModul Access + Geschäftslogik

## Zustände und Fehlerbehandlung

Alle async Operationen folgen diesem Muster:
```csharp
try
{
	var result = await _searchUseCase.OperationAsync(...);
	// Update UI State
}
catch (Exception ex)
{
	System.Diagnostics.Debug.WriteLine($"Operation error: {ex.Message}");
	// Optionale UI-Fehlerbehandlung
}
```

Benutzer-Fehler (Validierung) werden im ViewModel handled:
```csharp
if (RepoName_Text.Trim() == "")
{
	Interaction.MsgBox("Field required", ...);
	return;
}
```
