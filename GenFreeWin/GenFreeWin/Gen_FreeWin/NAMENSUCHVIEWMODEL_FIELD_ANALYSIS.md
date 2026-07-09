# NamenSuchViewModel Field Analysis - Scope Classification

## SCOPE-BASIERTE KLASSIFIZIERUNG

### 🔴 **DATEN-MODELL (PersonSearchData)** - 25 Felder
**Legacy VB-style Datenspeicherung - SOLLTE in separates Model**

```
Person-Identifikatoren:
- private int An;                    // Aktennummer?
- private short ID;                  // Person ID
- private int PersSp;                // Person-Spezifikation?
- private string Namen;              // Name
- private string Kennzt;             // Kennzeich(n)et

Familie/Event:
- private short BemSch;              // Bemerkung-Schrift?
- private short LfNR;                // Lfd. Nummer
- private int Fambehk;               // Familie-Bearbeitung?
- private short Beruf;               // Beruf

Arrays (mehrfach Werte):
- private string[] KontSP;           // Kontakt-Sammelpunkte (50)
- private string[] KontSP1;          // Kontakt-Sammelpunkte2 (50)
- private int[] Vorn;                // Vornamen
- private string[] Ruf;              // Rufnamen

Zähler/Iteratoren:
- private int I1;                    // Iterator 1
- private int A;                     // Counter A
- private int Z;                     // Counter Z
- private int U;                     // Counter U

Druckoptionen/Output:
- private IList<string> asOption;    // Array mit Option-Strings (korr. EOutCfg)
- private BoolProxy<EOutCfg, string> Option;  // Option-Proxy
- private int privaus;               // Privacy-Output-Modus

Datum/Zeit:
- private DateTime Datu;             // Datum (Geburt?)

Public Legacy:
- public bool EreiRf;                // ?
- public bool Scheid;                // Scheidung?

Verwendung:
- private int Modul1_priv;           // Modul1 privat?
- private (string, ETextKennz) Modul1_Bezeichnu;  // Modul1 Bezeichnung

FAZIT: **~25 Felder für PersonSearchData-Model**
```

---

### 🟡 **UI-STATE-MODELL (SearchUIState)** - 18 Felder
**Checkboxen, Sichtbarkeit, Enable-States - SOLLTE in separates Model**

```
Filter-Checkboxen (bereits [ObservableProperty]):
- public partial bool Male_Checked;
- public partial bool Females_Checked;
- public partial bool FamOnly_Checked;
- public partial bool Selection_Checked;
- public partial bool Male2_Checked;
- public partial bool Female2_Checked;
- public partial bool OmitSpouse_Checked;

Suchtext:
- public partial string Text1_Text;
- public partial string Text2_Text;

Suchparamter:
- public partial int PersNr;         // Person-Nummer
- public partial int FamNr;          // Familie-Nummer

Sichtbarkeitsstatus:
- public bool Male_Visible;
- public bool Females_Visible;
- public bool FamOnly_Visible;
- public bool Male2_Visible;
- public bool Female2_Visible;
- public bool OmitSpouse_Visible;
- public bool Text2_Visible;

Enable-Status:
- public bool OmitSpouse_Enabled;
- public bool Male_Enabled;
- public bool Female_Enabled;
- public bool FamOnly_Enabled;

UI-Navigation:
- public bool StartSearch_Visible;
- public bool Ready_Visible;
- public bool Label9_Visible;
- private bool Label4_Visible;
- private bool Label10_Visible;
- private bool ComboBox1_Visible;
- private bool PersonSheet_Visible;
- private bool FamilySheet_Visible;
- private string Label3_Text;
- private bool xDeathMark;

FAZIT: **~25 Felder für SearchUIState-Model**
```

---

### 🟢 **ABSTRAKTIONS-LAYER (Services/Adapters)** - Bereits gemacht
**Phase B/C hat bereits extrahiert:**
- INameSearchService (_searchService)
- ISearchResultMapper (_resultMapper)
- SearchStateAdapter (_stateAdapter)
- SearchCriteria, SearchResult, FilterOptions Models

---

### 🔵 **COLLECTIONS** - 8 ObservableCollections
**Für UI-Binding - BLEIBEN im ViewModel aber als Properties:**
```
- List1_Items
- List2_Items
- List3_Items
- List4_Items
- List5_Items
- List7_Items
- ListBox1_Items
- ComboBox1_Items
- Label1_Text
- Label5_Text
- Label7_Text
- Label8_Text
```

---

### 🟣 **SERVICE/INFRA-LAYER** - ~5 Properties
**Legacy Abhängigkeiten:**
```
- IModul1 Modul1;
- IGenPersistence Persistence
- IInteraction Interaction
- [Obsolete] IProjectData ProjectData
- [Obsolete] IVBInformation Information
- [Obsolete] IStrings Strings
- static EventHandler IdleEvent;
```

---

### 🟠 **VIEW-PROPERTIES (Legacy)** - ~10 Properties
**WinForms CodeBehind Zugriff:**
```
- [Obsolete] Namensuch View
- FraNameSrchSelection fraNameSrchSelection1
- [Obsolete] PictureBox PictureBox1
- [Obsolete] ComboBox ComboBox1
- [Obsolete] ComboBox ComboBox2
- [Obsolete] GroupBox Frame3
- [Obsolete] ListBox ListBox1
- [Obsolete] ListBox List1
- [Obsolete] Cursor Cursor
```

---

## REFERENZ: Phase B/C BESTEHENDE EXTRAKTION

```csharp
// Bereits vorhanden:
- SearchCriteria model (search input)
- SearchResult model (search output)
- FilterOptions model (filter state snapshot)
- INameSearchService (search service)
- NameSearchService (search implementation)
- ISearchResultMapper (result formatting)
- SearchResultMapper (display text generation)
- SearchStateAdapter (ViewModel↔Models bridge)
```

---

## EMPFEHLUNG - EXTRACTION-PLAN

### **PersonSearchData Model** (extract ~25 Felder)
```csharp
public class PersonSearchData
{
	public int AccountNumber { get; set; }           // An
	public int PersonId { get; set; }                // ID
	public string PersonName { get; set; }           // Namen
	public string Identifier { get; set; }           // Kennzt
	public short RemarkScript { get; set; }          // BemSch
	public short SequenceNumber { get; set; }        // LfNR
	public int FamilyProcessing { get; set; }        // Fambehk
	public short Occupation { get; set; }            // Beruf

	public string[] ContactPoints { get; set; }      // KontSP
	public string[] ContactPoints2 { get; set; }     // KontSP1
	public int[] FirstNames { get; set; }            // Vorn
	public string[] CallNames { get; set; }          // Ruf

	public DateTime DateOfBirth { get; set; }        // Datu
	public int PrivacyMode { get; set; }             // privaus
	public List<string> OutputOptions { get; set; }  // asOption

	public bool IsRelatedEvent { get; set; }         // EreiRf
	public bool IsDivorced { get; set; }             // Scheid
}
```

### **SearchUIState Model** (extract ~25 Felder)
```csharp
public class SearchUIState
{
	// Filter Checkboxes
	public bool MaleChecked { get; set; }
	public bool FemaleChecked { get; set; }
	public bool FamilyOnlyChecked { get; set; }
	// ... weitere

	// Search Input
	public string SearchText { get; set; }           // Text1_Text
	public string AdditionalText { get; set; }       // Text2_Text
	public int PersonNumber { get; set; }            // PersNr
	public int FamilyNumber { get; set; }            // FamNr

	// Visibility State
	public bool MaleVisible { get; set; }
	public bool FemaleVisible { get; set; }
	// ... weitere visibility properties

	// Enable State
	public bool MaleEnabled { get; set; }
	// ... weitere enable properties
}
```

### **Result: NamenSuchViewModel wird zur Service-Facade**
```csharp
public partial class NamenSuchViewModel : BaseViewModelCT, INamenSuchViewModel
{
	// DI
	private INameSearchService _searchService;
	private ISearchResultMapper _resultMapper;
	private SearchStateAdapter _stateAdapter;

	// Data Models
	private PersonSearchData _personData;
	private SearchUIState _uiState;

	// Collections (UI-Binding)
	public ObservableCollection<ListItem<...>> List1_Items { get; }
	// ... weitere collections

	// Service/Command Handlers
	public IAsyncRelayCommand ExecuteSearchCommand { get; }
	public IRelayCommand OmitSpouseToggledCommand { get; }
	// ... weitere commands

	// Minimal legacy support
	public IModul1 Modul1 { get; }
	[Obsolete] Namensuch View { get; }
}
```

---

## IMPACT ANALYSIS

| Aspekt | Vorher | Nachher | Effekt |
|--------|--------|---------|--------|
| **NamenSuchViewModel Größe** | 7600 Zeilen | ~1500 Zeilen | ✅ -80% |
| **Felder im ViewModel** | 50+ Felder | <20 Felder | ✅ Clarity |
| **Testability** | Schwer | Leicht (Models testbar) | ✅ Besser |
| **Reusability** | ❌ Keine | ✅ Models overallreusable | ✅ Besser |
| **Legacy Compat** | ✅ Ja | ✅ Noch ja | ✅ Preserved |
| **IDE Intellisense** | ❌ Überwältigend | ✅ Klar | ✅ Besser |

