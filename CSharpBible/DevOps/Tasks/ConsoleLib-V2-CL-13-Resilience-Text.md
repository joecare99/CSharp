# CL-13: Diagnostik und Textlayout härten

**Status:** Abgeschlossen

**Umfang:** UnicodeTextLayoutService, InMemoryRenderContext,
CXAML-Diagnostik, sichtbare Binding-/Renderfehler und TextBox-Caret-Zellen.

**Details:** Fehler werden nicht als stille Erfolgspfade verschluckt.
Textlayout berücksichtigt Unicode-Zellbreiten und der Caret wird in der
Terminaldarstellung eindeutig repräsentiert.

**Abhängigkeiten:** CL-12.

**Validierung:** Core-, CXAML- und Rendering-Regressionstests.
