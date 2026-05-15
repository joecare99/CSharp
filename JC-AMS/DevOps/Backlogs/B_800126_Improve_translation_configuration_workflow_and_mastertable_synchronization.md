# Backlog: Übersetzungs-Konfigurationsworkflow verbessern

## Beschreibung

Die Übersetzungsoberfläche enthält Suche, Vergleich, Import, Export und Synchronisation in einer zentralen administrativen Ansicht. Daraus ergibt sich ein Backlog für Verbesserungen an Pflege, Nachvollziehbarkeit und Trennung der Verantwortlichkeiten.

## Ableitung aus dem Code

- Die Oberfläche greift direkt auf Datenbanktabellen und globale Registry-Daten zu.
- Synchronisationslogik, UI-Logik und Persistenz sind eng miteinander verbunden.
- Der Vergleich zwischen lokal und Master ist hilfreich, aber fachlich komplex und wartungsintensiv.

## Mögliche Verbesserungsrichtungen

- Vergleichs- und Synchronisationslogik fachlich von der UI lösen
- Fehlermeldungen und Statushinweise vereinheitlichen
- Import/Export robust gegen ungültige oder unvollständige Übersetzungsdateien machen
- Synchronisation der Message-Tabellen stärker kapseln
- Auswertung der Änderungen besser protokollieren

## Ziel

Die Pflege von Übersetzungen soll nachvollziehbarer, testbarer und weniger fehleranfällig werden.
