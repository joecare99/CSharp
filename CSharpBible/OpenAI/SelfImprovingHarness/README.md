# SelfImprovingHarness

Ein selbstoptimierendes .NET-8-Harness für Ollama. Es erstellt Kandidaten in isolierten Generationsordnern, kompiliert und bewertet sie und schreibt niemals Quelldateien der stabilen Basis direkt um.

## Architektur
- `OllamaClient`: `POST /api/generate`, konfigurierbares Modell, Timeout und Retries.
- `CompilerService`: startet `dotnet build --no-restore`, sammelt Roslyn-/CLI-Fehler.
- `SelfModifier`: liest `.cs`-Dateien, verwendet einen Verbesserungs-/Reparatur-Prompt und extrahiert Markdown-Code-Fences.
- `FitnessEvaluator`: Build, Smoke-Test (`--smoke-test`) und kleine Laufzeitmessung; numerischer Score.
- `Orchestrator`: Baseline, Generations-/Repair-Schleife, Akzeptanz nur bei echter Verbesserung.
- `RunLogger`: append-only `run-log.jsonl` mit allen wichtigen Schritten.

## Bedienung
Voraussetzungen: .NET 8 SDK und optional Ollama. Im Projektordner:
```bash
dotnet restore
dotnet run -- --model=llama3.1 --generations=3 --ollama-url=http://localhost:11434
```
Die Konfiguration steht in `appsettings.json`; Command-Line-Werte überschreiben sie. Ein Ollama-Fehler beendet den Lauf nicht unkontrolliert: Die laufende Generation wird protokolliert und übersprungen.

## Sicherheit
Nur der Projektordner ist als Arbeitsbereich vorgesehen. Kandidaten landen unter `generations/`; `bin/`, `obj/` und bereits erzeugte Generationen werden nicht als Quellinput verwendet. Vor jedem akzeptierten Swap wird ein Backup in `backups/` angelegt. `state.json` ist der Version-Pointer; die stabile Basis bleibt unverändert. Für produktive Nutzung zusätzlich Prozess-/Dateisystem-Sandbox (Container, nicht privilegierter Benutzer, Ressourcenlimits und Netzwerk-Allowlist) einsetzen: Dieses Beispiel ist keine vollständige OS-Sandbox.

## Beispielablauf
1. Basis bauen und Fitness messen.
2. `gen001` durch Ollama erzeugen.
3. Kandidat bauen, Smoke-Test und Benchmark ausführen.
4. Bei Build-Fehlern bis zu `MaxRepairAttempts` Reparaturprompts senden.
5. Kandidat nur bei höherem Score akzeptieren, sonst verwerfen.
