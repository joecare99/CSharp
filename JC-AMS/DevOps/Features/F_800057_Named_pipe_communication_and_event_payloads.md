# Feature: Bereitstellt Named-Pipe-Kommunikation und Event-Payloads

## Beschreibung

Dieses Feature beschreibt die Named-Pipe-Kommunikationsbausteine im Core-Bereich. Die aktuell lesbaren Artefakte deuten auf Pipe-basierte Datenträger, Eventdaten und Hilfsklassen für Leseereignisse hin.

## Sichtbare technische Bausteine

- `Core\Core\Communication\CNamedPipe.cs`
- `Core\Core\Communication\CNamedPipeReadEventArgs.cs`
- `Core\Core\Communication\NamedPipes\NamedPipeReadEventHandler.cs`
- `Manager\Manager\FormNamedPipeTest.cs`

## Fachlicher Nutzen

- Named-Pipe-Schnittstellen können als interne Kommunikationskanäle verwendet werden
- Leseereignisse sind über EventArgs strukturierbar
- Die Komponenten unterstützen Test- und Diagnoseoberflächen

## Beobachtete Abläufe

- Die Dateiinstanzen waren im aktuellen Lesedurchlauf nicht auslesbar, daher ist der konkrete Implementierungsumfang noch unklar.
- Die Projektstruktur zeigt jedoch einen dedizierten Named-Pipe-Zweig mit EventHandler und Testform.

## Offene Fragen

- Wie die konkrete Pipe-Implementierung intern aufgebaut ist
- Welche Lese-/Schreibereignisse und Protokolle unterstützt werden
