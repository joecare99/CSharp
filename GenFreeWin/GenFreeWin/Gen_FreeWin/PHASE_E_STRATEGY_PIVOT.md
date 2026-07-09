# PHASE_E_STRATEGY_PIVOT.md

**Date:** Phase E.2 Initiation
**Context:** Handler extraction approach tested and refined

---

## What Happened

During Phase E.1, an external `ListFillHandler` class was prototyped to extract search branches from `Listfuell()`. Build testing revealed **integration friction**:

- `DataModul.Event`, `DataModul.Person` are concrete module properties (not interface)
- `Option[EOutCfg]` is a `BoolProxy` – adapter layer required to access from external class
- Legacy code tightly coupled to global state prevents clean DI pattern

---

## Decision: In-ViewModel Extraction Strategy

Instead of external handler, extract search-mode logic as **private helper methods within the ViewModel**:

### Rationale

1. **Zero adapter cost** – Direct access to `this.DataModul`, `this.Modul1`, `this.Option`, loop variables
2. **Control-flow preservation** – `goto` targets remain reachable
3. **Backwards compatible** – No refactoring of external callers needed
4. **Incremental wins** – Each extracted method immediately reduces line count
5. **Testability roadmap** – Private helpers → public methods → separate service in Phase F

### Implementation Pattern

```csharp
// BEFORE (all inline in Listfuell)
if (ComboBox2.Text == Modul1.IText[EUserText.t312])
{
	Listleer();
	// ... 56 lines of death/burial search logic ...
	View.Cursor = Cursors.Default;
	List1_Items.Add(new("Ende der Liste"));
	goto end_IL_0001_2;
}

// AFTER (extracted to private helper)
if (ComboBox2.Text == Modul1.IText[EUserText.t312])
{
	ExecuteDeathSearch(dateText, female2Checked, male2Checked);
	goto end_IL_0001_2;
}

// ... rest of method remains unchanged ...

private void ExecuteDeathSearch(string dateText, bool female2Checked, bool male2Checked)
{
	// Extract lines 2938–2993 here
	// Direct access to: DataModul, Modul1, Option, List1_Items, etc.
}
```

---

## Phase E.2 Next Steps

1. **Extract Death/Burial search → `ExecuteDeathSearch()`** (t312, ~56 lines)
2. **Extract Birth/Baptism search → `ExecuteBirthSearch()`** (t311, ~59 lines)
3. **Extract Ancestor search → `ExecuteAncestorSearch()`** (t309, ~137 lines)
4. **Extract Descendant block → `ExecuteDescendantSearch()`** (IL_14cb, ~162 lines)
5. **Extract other branches → smaller helpers** (remaining t-codes)
6. **Measure ViewModel size reduction**
7. **Plan Phase F:** Convert private helpers → public testable methods/services

---

## Estimated Outcome

- **Starting size:** ~7410 lines in `NamenSuchViewModel.cs`
- **After extracting 5–7 major search branches:** ~7000–7050 lines
- **Line reduction:** ~350–400 lines (5%)
- **Combined with Phase D models & handlers:** Total refactored methods/classes visible

---

## Phase F (Future) – Service Layer Conversion

Once in-ViewModel helpers are proven stable:
- Convert private helpers → public methods
- Move to dedicated `SearchExecutor` or separate handler class
- Add DI for `DataModul`, `Modul1` references
- Enable unit testing

---

## Key Insight

Extracting from large legacy monoliths requires **pragmatism over architecture**.

Phase E achieves **visibility & line reduction** with minimal friction.
Phase F achieves **clean architecture & testability** once the monolith is smaller.

This two-phase approach spreads risk and validates each step.

---

_Strategy pivot approved for Phase E.2 execution._
