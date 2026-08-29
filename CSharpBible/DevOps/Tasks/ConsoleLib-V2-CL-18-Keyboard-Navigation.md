# CL-18: Tastatur-, Menü- und Listennavigation vervollständigen

**Status:** Abgeschlossen

**Umfang:** Standardisierte Bedienung für Button, einzeilige TextBox, MenuBar,
ListBox und TreeView.

**Details:** Fokusierte Buttons lösen über Space und Enter ihren bestehenden
`Click()`-Pfad aus. Einzeilige TextBoxen bieten `OnEnterKey`; ohne Handler
reicht Enter an Parent und Accelerators weiter, mehrzeilige TextBoxen behalten
ihren Zeilenumbruch. F10 aktiviert/deaktiviert die Menüleiste, öffnet das
Root-Popup und stellt den vorherigen Fokus wieder her. Alt zeigt die
Accelerator-Markierung im ExtCon-Renderer. ListBox unterstützt Up/Down,
Home/End und PageUp/PageDown zusätzlich zu J/K. TreeView verarbeitet +/- für
Auf-/Zuklappen zusätzlich zu Left/Right.

**Abhängigkeiten:** CL-01.

**Validierung:** 124 Core-Tests bestanden, einschließlich 8 gezielter
Regressionstests für Button, TextBox, ListBox, TreeView und MenuBar.
