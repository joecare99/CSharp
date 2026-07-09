// ***********************************************************************
// Assembly         : GenFreeWin
// Author           : Mir
// Created          : 2025
// ***********************************************************************
// <copyright file="RELAYCOMMAND_MIGRATION_GUIDE.md" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary>Guide for migrating NamenSuchViewModel events to RelayCommands</summary>
// ***********************************************************************

# RelayCommand Migration Guide for NamenSuchViewModel

## Overview
This guide documents the pattern for converting VB.NET-style event handlers 
to modern CommunityToolkit.Mvvm [RelayCommand] pattern in NamenSuchViewModel.

## Pattern: Old Event → New RelayCommand + [Obsolete] Fallback

### Example 1: Checkbox State Changed Event

**OLD EVENT HANDLER (VB Pattern):**
```csharp
[Obsolete]
public void Check2_CheckStateChanged(object eventSender, EventArgs eventArgs)
{
	if (eventSender == View.chbFamOnly)
	{
		Male2_Checked = false;
		Female2_Checked = false;
		Male_Checked = false;
		Females_Checked = false;
	}
	if (eventSender != View.chbSelection)
	{
		if (FamOnly_Checked)
		{
			Male2_Visible = false;
			Female2_Visible = false;
			Male_Visible = true;
			Females_Visible = true;
		}
		else
		{
			Male_Visible = false;
			Females_Visible = false;
			Male2_Visible = true;
			Female2_Visible = true;
		}
		_ = ComboBox1.Focus();
	}
}
```

**NEW RELAY COMMAND (Modern MVVM):**
```csharp
[RelayCommand]
private void FamOnlyToggled()
{
	// Clear cross-filters when family-only is toggled
	if (FamOnly_Checked)
	{
		Male2_Checked = false;
		Female2_Checked = false;
		Male_Checked = false;
		Females_Checked = false;
		Male2_Visible = false;
		Female2_Visible = false;
		Male_Visible = true;
		Females_Visible = true;
	}
	else
	{
		Male_Visible = false;
		Females_Visible = false;
		Male2_Visible = true;
		Female2_Visible = true;
	}
	ClearResults();
}

// DEPRECATE OLD: Forward to command
[Obsolete("Use FamOnlyToggledCommand via RelayCommand binding", false)]
public void Check2_CheckStateChanged(object eventSender, EventArgs eventArgs)
{
	try
	{
		FamOnlyToggled();
	}
	catch (Exception ex)
	{
		System.Diagnostics.Debug.WriteLine($"Check2_CheckStateChanged compat error: {ex.Message}");
	}
}
```

**VIEW BINDING (WinForms + CommandBindingAttribute):**
```csharp
// In Namensuch.cs (View code-behind):
[CommandBinding(nameof(INamenSuchViewModel.FamOnlyToggledCommand))]
private void chbFamOnly_CheckedChanged(object sender, EventArgs e)
{
	// Automatically delegates to ViewModel command via attribute
}

// OR manual wiring in constructor:
public Namensuch(IVornamViewModel viewModel)
{
	chbFamOnly.Click += (s, e) => 
		viewModel.FamOnlyToggledCommand?.Execute(null);
}
```

---

### Example 2: Gender Filter Checkboxes

**OLD EVENT HANDLERS:**
```csharp
[RelayCommand]
private void CheckMale()
{
	Females_Checked = false;
	Listleer();  // Clear results
}

[RelayCommand]
private void CheckFemale()
{
	Male_Checked = false;
	Listleer();
}

[RelayCommand]
private void CheckMale2()
{
	Female2_Checked = false;
	Listleer();
}

[RelayCommand]
private void CheckFemale2()
{
	Male2_Checked = false;
	Listleer();
}
```

**THESE ARE ALREADY GOOD RELAY COMMANDS!**
Only action needed: Rename `Listleer()` → `ClearResults()` (better C# naming)

---

### Example 3: Text Input Changed

**OLD EVENT HANDLER:**
```csharp
[Obsolete]
private void Text1_TextChanged(object eventSender, EventArgs eventArgs)
{
	// Clear results on every keystroke
	Listleer();
}
```

**NEW RELAY COMMAND:**
```csharp
[RelayCommand]
private void SearchTextChanged(string newText)
{
	SearchPattern = newText ?? "";
	ClearResults();
}

[Obsolete("Use SearchTextChangedCommand", false)]
private void Text1_TextChanged(object eventSender, EventArgs eventArgs)
{
	SearchTextChanged(Text1_Text);
}
```

**VIEW BINDING:**
```csharp
// In Namensuch.cs:
private void textBox1_TextChanged(object sender, EventArgs e)
{
	_viewModel.SearchTextChanged(textBox1.Text);
}
```

---

### Example 4: List Item Selection (DoubleClick)

**OLD EVENT HANDLER:**
```csharp
[Obsolete]
private void List1_DoubleClick(object eventSender, EventArgs eventArgs)
{
	// Process selected item
	if (ListBox1_SelectedItem != null)
	{
		HandleListItemSelected(ListBox1_SelectedItem.IntValue);
	}
}
```

**NEW RELAY COMMAND:**
```csharp
[RelayCommand]
private void SelectListItem(ListItem<int> item)
{
	if (item != null)
	{
		HandleListItemSelected(item.IntValue);
	}
}

[Obsolete("Use SelectListItemCommand", false)]
private void List1_DoubleClick(object eventSender, EventArgs eventArgs)
{
	SelectListItem(ListBox1_SelectedItem);
}
```

**VIEW BINDING:**
```csharp
// In Namensuch.cs:
private void listBox1_DoubleClick(object sender, EventArgs e)
{
	if (listBox1.SelectedItem is ListItem<int> selected)
	{
		_viewModel.SelectListItemCommand?.Execute(selected);
	}
}
```

---

## Logging: All Existing RelayCommands in NamenSuchViewModel

### ✅ Already Modern (Just rename for clarity):
1. `[RelayCommand] FamOnlyChecked()` → rename to `FamOnlyToggled()`
2. `[RelayCommand] CheckMale()` → OK
3. `[RelayCommand] CheckFemale()` → OK
4. `[RelayCommand] CheckMale2()` → OK
5. `[RelayCommand] CheckFemale2()` → OK

### 🔄 To Migrate (Event → Command):
1. `Check2_CheckStateChanged()` → `FamOnlyToggled()` + `[RelayCommand]`
2. `chbOmitSpouse_CheckStateChanged()` → `OmitSpouseToggled()` + `[RelayCommand]`
3. `Text1_TextChanged()` → `SearchTextChanged()` + `[RelayCommand]`
4. `ComboBox2_TextChanged()` → `ComboBoxTextChanged()` + `[RelayCommand]`
5. `Combo1_KeyPress()` → `SearchKeyPressed()` + `[RelayCommand]` (with KeyEventArgs param)
6. `List1_DoubleClick()` → `SelectListItem()` + `[RelayCommand]`
7. `ListBox1_Click()` → `ListBoxItemClicked()` + `[RelayCommand]`
8. `Label5_DoubleClick()` → `LabelDoubleClicked()` + `[RelayCommand]`

### 🔄 To Rename/Refactor (Business Logic):
1. `Listleer()` → `ClearResults()` (better naming)
2. `StartSearch()` → Already refactored to `ExecuteSearchAsync()`
3. `PersonSheet()` → Keep name, add `[RelayCommand]`
4. `FamilySheet()` → Keep name, add `[RelayCommand]`
5. `PrintList()` → Keep name, add `[RelayCommand]`

---

## Command Registration Pattern

### In NamenSuchViewModel (Constructor):
```csharp
public NamenSuchViewModel(
	INameSearchService searchService,
	ISearchResultMapper resultMapper)
{
	_searchService = searchService;
	_resultMapper = resultMapper;
	_stateAdapter = new SearchStateAdapter(this);

	// RelayCommands are auto-generated by CommunityToolkit.Mvvm
	// No manual registration needed

	// Initialize collections...
}
```

### In INamenSuchViewModel Interface (Add):
```csharp
// Commands accessible for binding
IAsyncRelayCommand ExecuteSearchCommand { get; }
IRelayCommand FamOnlyToggledCommand { get; }
IRelayCommand CheckMaleCommand { get; }
IRelayCommand SearchTextChangedCommand { get; }
// etc.
```

---

## Testing Checklist

After migration, verify:
- [ ] All [RelayCommand] decorated methods are public
- [ ] Command names end with "Command" in interface (auto-generated by toolkit)
- [ ] Old [Obsolete] event-handlers still work (backward compat)
- [ ] New commands bind correctly via CommandBindingAttribute
- [ ] No Console/Debug errors when clicking UI controls
- [ ] Search functionality works after refactor
- [ ] Filter/checkbox toggles update UI state correctly

---

## VB.NET API Migration (Bonus Reference)

| VB.NET | Modern C# | Helper |
|--------|-----------|--------|
| `Strings.Asc(char)` | `(int)char` | `VBCompatibilityHelper.AsciiValue()` |
| `Strings.Chr(int)` | `(char)int` | `VBCompatibilityHelper.CharFromCode()` |
| `Information.Err()` | Try/catch or Result<T> | Wrap in Result pattern |
| `FileSystem.FileOpen()` | `File.Open()` / `StreamWriter` | Use System.IO |
| `Format(num, "0.00")` | `num.ToString("0.00")` | `VBCompatibilityHelper.Format()` |

---

## Summary

**Key Changes:**
1. Convert event-handlers marked [Obsolete] → [RelayCommand] methods
2. Keep old event-handlers as deprecated forwarding shims (backward compat)
3. Update INamenSuchViewModel interface with command properties
4. Use CommandBindingAttribute in View for declarative binding
5. Use VBCompatibilityHelper for legacy VB.NET function replacements

**Result:** NamenSuchViewModel becomes MVVM-compliant, UI-agnostic, and testable.
