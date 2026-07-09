# Lizenz-Dialog Refactoring - Architectural Documentation

## Overview

Die License-Dialog wurde von einer monolithischen Klasse in eine layered-Architektur mit klarer Separation of Concerns aufgeteilt. Dadurch sind die einzelnen Komponenten testbar, wartbar und wiederverwendbar.

## Architecture Layers

### 1. **View Layer** (`Lizenz.cs`)
**Verantwortung**: UI-Präsentation, Event-Delegation, Benutzereingaben
- **Nur UI-spezifische Logik**
  - Textfeld-Validierung (KeyPress, KeyUp Events)
  - Tab-Order und Focus-Navigation
  - Kontrollelemente-Handling (ControlArray)
  - Lokalisierung der Beschriftungen (Label-Texte)
- **Delegiert Geschäftslogik an ViewModel**
  - Sammelt Eingaben aus UI-Feldern
  - Ruft `_viewModel.VerifyLicense()` auf
  - Behandelt Benutzer-Abbruch

**Key Files**:
- `..\Gen_FreeWin\GenFreeWin\Gen_FreeWin\Views\Lizenz.cs`
- `..\Gen_FreeWin\GenFreeWin\Gen_FreeWin\Views\Lizenz.Designer.cs`

**Wichtige Änderungen**:
- Instanzen von `_viewModel` und `_persistenceService` hinzugefügt
- Geschäftslogik aus `_Command1_Click` entfernt
- Fokus auf Event-Delegation und UI-Handling

### 2. **ViewModel Layer** (`LicenseViewModel.cs`)
**Verantwortung**: Geschäftslogik, State-Management, Orchestrierung
- **Implementiert INotifyPropertyChanged** für Datenbindung
- **State-Verwaltung**
  - `ProductId`, `ManufacturerId`, `LicenseKey` Properties
  - `AttemptCounter` für Verifizierungsversuche
  - `IsVerified` Flag für erfolgreiche Bestätigung
- **Business-Methoden**
  - `VerifyLicense()` - Koordiniert Validierung und Persistierung
  - `AreAllFieldsComplete()` - Prüft Eingabevollständigkeit
  - `ResetForRetry()` - Zurücksetzen für neuen Versuch
- **Fehlerbehandlung und User-Feedback**
  - Zeigt Retry-Dialoge über `IInteraction`
  - Bricht Anwendung bei zu vielen Versuchen ab
  - Loggt Fehler bei Persistierung

**Key File**: `..\Gen_FreeWin\GenFreeWin\Gen_FreeWin\ViewModels\LicenseViewModel.cs`

**Dependencies**:
```csharp
public LicenseViewModel(IModul1 modul1, IInteraction interaction, IProjectData projectData, IStrings strings)
```

### 3. **Model Layer** (`LicenseData.cs`)
**Verantwortung**: Datenstruktur, Validierungslogik
- **Properties**
  - `ProductId` (10-stellig)
  - `ManufacturerId` (10-stellig)
  - `LicenseKey` (min. 5-stellig)
  - `SerialNumber` (calculated: `ProductId-GB-ManufacturerId-LicenseKey`)
- **Validierungsmethoden**
  - `IsValid()` - Vollständige Formatvalidierung
  - `ValidateChecksum()` - Checksummen-Verifizierung
	- Summe aller Ziffern von ManufacturerId (10 Zeichen)
	- Plus erstes Ziffer von LicenseKey
	- Geteilt durch letztes Ziffer von LicenseKey
	- Vergleich mit Ziffern 3-4 von LicenseKey

**Key File**: `..\Gen_FreeWin\GenFreeWin\Gen_FreeWin\Models\LicenseData.cs`

**Beispiel-Validierung**:
```csharp
var license = new LicenseData
{
	ProductId = "Q123456789",
	ManufacturerId = "1234567890", 
	LicenseKey = "0012345"
};

if (license.IsValid()) 
{
	// Lizenz ist gültig
}
```

### 4. **Data Access Layer** (`LicensePersistenceService.cs`)
**Verantwortung**: Persistierung, Datenzugriff, System-State
- **Persistierungs-Operationen**
  - `SaveLicenseSerialNumber()` → IDF.Dat
  - `LoadLicenseSerialNumber()` ← IDF.Dat
  - `LoadOwnerInformation()` ← adress.dat
- **System-State Management**
  - `ActivateLicense()` - Setzt System.xDemo = false
  - `DeactivateLicense()` - Setzt System.xDemo = true
- **Hilfsmethoden**
  - `IsLicenseRegistered()` - Prüft Lizenz-Status

**Key Files**:
- `..\Gen_FreeWin\GenFreeWin\Gen_FreeWin\Services\LicensePersistenceService.cs`
- `..\Gen_FreeWin\GenFreeWin\Gen_FreeWin\Services\ILicensePersistenceService.cs`

**Usage**:
```csharp
var service = new LicensePersistenceService(modul1);
service.SaveLicenseSerialNumber("Q1234567890-GB-1234567890-00123456");
service.ActivateLicense();
```

## Data Flow

```
User Input (TextBoxes)
	↓
View._Command1_Click()
	↓
_viewModel.ProductId = AText1[0].Text (etc.)
	↓
_viewModel.VerifyLicense()
	├→ Create LicenseData Model
	├→ Call licenseData.IsValid()
	│  ├→ Check product ID starts with 'Q'
	│  ├→ Validate lengths
	│  └→ ValidateChecksum()
	├→ If valid:
	│  ├→ PersistLicense()
	│  │  ├→ _persistenceService.SaveLicenseSerialNumber()
	│  │  ├→ _persistenceService.ActivateLicense()
	│  │  └→ Update Menue.Default (legacy)
	│  └→ Return true
	└→ If invalid:
	   ├→ Show retry dialog via IInteraction
	   ├→ Check AttemptCounter
	   └→ Return false
	↓
View.Close()
```

## Integration mit WFSystem.Data

Zukünftige Integration mit **TextBindingAttribute** aus `WFSystem.Data`:

```csharp
public partial class Lizenz : Form
{
	[TextBinding(nameof(LicenseViewModel.ProductId))]
	private TextBox _productIdControl;

	private void InitializeBindings()
	{
		TextBindingAttribute.Commit(this, _viewModel);
	}
}
```

## Testing-Strategie

### Unit Tests für LicenseData
```csharp
var license = new LicenseData 
{ 
	ProductId = "Q123456789", 
	ManufacturerId = "1234567890", 
	LicenseKey = "00123456" 
};
Assert.IsTrue(license.IsValid());
```

### Unit Tests für LicenseViewModel
```csharp
var modul1 = mockModul1.Object;
var vm = new LicenseViewModel(modul1, mockInteraction, mockProjectData, mockStrings);
vm.ProductId = "Q123456789";
vm.ManufacturerId = "1234567890";
vm.LicenseKey = "00123456";
Assert.IsTrue(vm.VerifyLicense());
```

### Integration Tests
- View-ViewModel-Interaction testen
- Persistierungs-Calls verifizieren
- Fehlerbehandlung validieren

## Abhängigkeiten

- **GenFree.Interfaces.Sys**: IModul1, IProjectData, IStrings, IInteraction
- **MVVM_BaseLib**: NotifyPropertyChangedBase für INotifyPropertyChanged
- **System.Windows.Forms**: UI-Framework

## Rückwärts-Kompatibilität

⚠️ **Hinweis**: Die aktuelle Implementierung behält den statischen `Menue.Default`-Zugriff bei, um Rückwärts-Kompatibilität zu wahren. Dies sollte in einer zukünftigen Refactoring entfernt werden und stattdessen über DI injiziert werden.

## Nächste Schritte

1. **TextBinding-Integration**: Vollständige Bindung mit WFSystem.Data-Attributen
2. **Validierungs-UI Feedback**: Toast/Banner für Fehler statt MessageBox
3. **Async Support**: Verzögerte Persistierungs-Operationen
4. **Dependency Injection**: Registrierung in DI-Container
5. **Unit Test Coverage**: > 90% Coverage für alle Layers
