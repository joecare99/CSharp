# CL-19: Release-Readiness erneut bewerten

**Status:** Blockiert

**Stand:** 2026-08-28, SVN-Snapshot Revision 1777. Die CL-16/CL-17-
Änderungen sind versioniert; die verschobene, benutzerverwaltete
`ConsoleFrameworkTests.cs` bleibt bewusst als fehlend markiert.

**Umfang:** Frühere Abschlussstatus korrigieren, CXAML-/Designer-/Keyboard-
Verträge dokumentieren, produktionsbezogene Coverage erfassen und alle
geteilten Testprojekte sequenziell validieren.

**Aktueller Befund:**

- **CL-16 – Abgeschlossen:** Vier CXAML-Referenzanwendungen bauen einzeln
  erfolgreich; vier separate anwendungsspezifische Integrationstests und
  vier gemeinsame Beispieltests decken alle Anwendungspfade ab.
- **CL-17 – Abgeschlossen:** Gerenderte Avalonia-Preview, stabile Mapping-
  IDs, Preview-/Inspector-/Quelltext-Auswahl und neun Designer-Tests,
  einschließlich dediziertem Avalonia-Headless-Hosting, sind vorhanden.
- **CL-18 – Abgeschlossen:** Der aktuelle Core-Testlauf besteht mit 132 Tests.

**Validierungscheckliste:**

- [x] Vier unabhängige CXAML-Anwendungsbuilds: 0 Warnungen, 0 Fehler.
- [x] `ConsoleLib.CoreTests`: 133 bestanden.
- [x] `ConsoleLib.Cxaml.DesignerTests`: 12 bestanden, einschließlich Headless-
  Hosting.
- [x] `ConsoleLib.Cxaml.ExamplesTests`: 4 bestanden.
- [x] CXAML-Attribute `Tag`, `Accelerator`, `Shadow`, `BorderStyle`,
  `BorderColor` und `HLBackColor` in Loader und Referenzmarkups validiert.
- [x] Accelerator-Auslösung auf KeyDown begrenzt; KeyUp erzeugt keine
  Doppelaktion.
- [x] Designer-Datei-I/O und Panel-Root-Preview einschließlich Canvas-
  Positionierung, Root-Layoutfläche und skalierter Console-Zellkoordinaten
  validiert.
- [x] Vier anwendungsspezifische CXAML-Testprojekte: je 1 Test bestanden.
- [x] Gezielter SVN-Snapshot r1777 erstellt.

**Blocker:**

Die Releasefreigabe bleibt wegen der ausdrücklich geschützten,
benutzerverwalteten Verschiebung von `ConsoleFrameworkTests.cs` und der
weiterhin offenen fachlichen Detective-Suggestion-Auswahl zurückgestellt.
Eine Releasefreigabe wird erst nach der fachlichen Suggestion-Auswahl und
einer ausdrücklich freigegebenen Arbeitskopie-Basis behauptet.
