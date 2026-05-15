# Feature: Dokumentiert die Logging-Test-Suite und Zielverhalten

## Beschreibung

Dieses Feature beschreibt `SLoggingTests` als Test-Suite für die Logging-Infrastruktur. Die Tests zeigen insbesondere das Routing an Startup-Log, Debug- und Fehlerpfade sowie mehrere noch offene Platzhalter.

## Sichtbare technische Bausteine

- `Core.Tests\Core\Logging\SLoggingTests.cs`
- `Core\Core\Logging\SLogging.cs`
- `Core\Core\System\CState.cs`

## Fachlicher Nutzen

- Logging-Ausgaben sind über Tests nachvollziehbar
- Startup- und Journalize-Verhalten bleiben reproduzierbar
- Die aktuell offenen Teststellen markieren weitere technische Schulden

## Beobachtete Testinhalte

- Ein eigener `DoWriteLog`-Stub sammelt erwartete Logaufrufe in `DebugResult`.
- `JournalizeTest`, `JournalizeTest0_1` und `JournalizeTest1` prüfen konkrete Logausgaben.
- Mehrere Tests wie `CreateProtocolTest`, `DebugPrintTest` oder `DeleteStartupLogTest` sind noch als `Assert.Fail()` markiert.

## Offene Fragen

- Welche Logging-Funktionen als nächstes mit echten Assertions ergänzt werden sollen
- Ob die erwarteten String-Literale für Logausgaben langfristig stabil bleiben
