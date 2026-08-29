# CL-08: CXAML-Referenzanwendungen migrieren

**Status:** Abgeschlossen

**Umfang:** Parallele CXAML-Beispiele für Calc32Cons, Leonardo.ST,
DetectiveGame.Console und Ollama.CodingAgent.Console.

**Details:** Jedes Beispiel bettet eine CXAML-View ein und besitzt eine kleine
Factory für Startup und Tests. Die bestehenden imperativen Anwendungen wurden
nicht ersetzt.

**Abhängigkeiten:** CL-04, CL-06, CL-07, CL-11, CL-14.

**Validierung:** Eigenes `ConsoleLib.Cxaml.ExamplesTests`-Projekt mit 4 Tests;
SVN-Revision 1768.
