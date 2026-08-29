# CL-16: Funktionale CXAML-Referenzanwendungen

**Status:** Abgeschlossen

**Umfang:** Sichere CXAML-Binding-Kontexte, benannte Controls,
Command-/Text-/Listenbindungen sowie die Migration der realen Views von
Calc32, Leonardo, DetectiveGame und Ollama.

****Details:** `CxamlLoadContext` und `CxamlLoadResult` bilden die sichere
Grundlage: das Binding referenziert ausschließlich einen explizit gelieferten
DataContext; Namen werden deterministisch auf materialisierte Controls
abgebildet. Calc32Cons, Leonardo.ST, DetectiveGame.Console und
Ollama.CodingAgent.Console laden jetzt ihre realen CXAML-Strukturen über
anwendungsspezifische Adapter und starten über DI-Composition-Roots. Die
imperativen Anwendungen bleiben unverändert.

**Abhängigkeiten:** CL-01, CL-05, CL-08.

**Validierung:** Vier unabhängige CXAML-Anwendungsbuilds und vier
CXAML-Integrationstests bestanden. Zusätzlich bestehen die vier
CXAML-Binding-Regressionstests. Der gemeinsame Beispieltest verwendet
`net8.0-windows`, damit die referenzierten Windows-Targets korrekt geprüft
werden.
