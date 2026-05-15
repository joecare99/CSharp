# Backlog Item: Test coverage for logging and core utilities

## Ziel

Die vorhandenen Core- und Logging-Hilfen sollen durch nachvollziehbare Tests abgesichert werden, damit Änderungen an Hilfsfunktionen und Protokollierung sicherer werden.

## Inhalt

- Tests für Logging-Verhalten
- Tests für Hilfs- und Erweiterungsmethoden
- Stabilisierung von aktuellen Testfällen mit Platzhaltern
- Nachvollziehbare Erwartungswerte und Randbedingungen

## Beobachtungen aus der Solution

- `Core.Tests\Core\Logging\SLoggingTests.cs` enthält bereits Testgerüste
- Mehrere Tests sind noch mit `Assert.Fail()` markiert
- Die Testbasis liegt bereits für diverse Core-Bereiche vor

## Akzeptanzkriterien

- Bestehende Testgerüste werden sinnvoll ergänzt
- Kernhilfen werden gegen Regressionen abgesichert
- Logging-Verhalten ist reproduzierbar prüfbar
- Testausfälle durch Platzhalter werden schrittweise reduziert
