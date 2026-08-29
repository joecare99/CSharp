# CL-11: Tree- und Tile-Widgets implementieren

**Status:** Abgeschlossen

**Umfang:** TreeView-/TileView-Navigation, Auswahl, Expansion, Indentation,
absolute Positionierung, ANSI-Rendering und Clipping.

**Details:** `PosixCollectionRenderer` rendert beide Widgettypen mit
selection-aware styling und berücksichtigt Control-Grenzen sowie sichtbare
Inhalte.

**Abhängigkeiten:** CL-01, CL-10.

**Validierung:** Umfangreiche Navigation-, Randfall- und Renderer-Tests.
