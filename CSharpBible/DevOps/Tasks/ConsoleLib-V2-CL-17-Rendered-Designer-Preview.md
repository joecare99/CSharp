# CL-17: Gerenderte bidirektionale Designer-Preview

**Status:** Abgeschlossen

**Umfang:** Avalonia-Preview-Adapter, stabile Zuordnung von CXAML-Elementen zu
ConsoleLib-Controls sowie Auswahlfluss Preview → Inspector → Quelltext.

**Details:** Ein Klick auf ein Preview-Control aktiviert das entsprechende
CXAML-Element und dessen Inspector-Eigenschaften. Inspector-Änderungen
aktualisieren die Preview, ungültiges Markup bleibt ohne veraltete Darstellung.

**Abhängigkeiten:** CL-16 (CXAML runtime APIs only; no application-project dependency).

**Validierung:** Unit- und Mapping-Regressionstests in
`ConsoleLib.Cxaml.DesignerTests`. Die Avalonia-Preview rendert auch ein
`Panel` als Root-Control mit stabiler Layoutfläche für absolute
Kinderpositionen. Ungelöste Design-Bindings werden als Platzhalter geladen,
damit gebundene Referenz-Views im Designer sichtbar bleiben. ConsoleLib-
Zellkoordinaten werden für Avalonia auf eine lesbare Preview-Größe skaliert;
die große Preview-Fläche ist scrollbar, damit auch untere Controls
anwählbar bleiben; der aktuelle Stand umfasst zwölf bestandene Tests.
