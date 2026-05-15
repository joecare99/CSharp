# Feature: Synchronisiert Systemwerte aus Telegrammen und Datenquellen

## Beschreibung

Dieses Feature beschreibt die Verarbeitung von Systemwerten aus Telegrammen, Datenbank und Kommunikationsschicht. Die Logik hält Systemwerte aktuell, verknüpft sie mit Telegrammdefinitionen und speist sie in fachliche Folgeprozesse ein.

## Sichtbare technische Bausteine

- `Core\Core\CommSystem\SSystemValuesHelpers.cs`: Lesen, Schreiben und Synchronisieren von Systemwerten
- `Core\Core\System\Values\CSystemValueExt.cs`: erweiterte Systemwerte mit Telegramm- und Datenbankbezug
- `Core\Core\CommSystem\Telegram`: Telegrammtypen und Feldzuordnung
- `Message\Message\CSyncMessageHistory.cs`: nutzt Systemwerte für Meldungshistorien
- `Message\Message\CSyncMessageMessage.cs`: legt fehlende Systemwerte aus Meldungsdefinitionen an

## Fachlicher Nutzen

- Telegramme können direkt in aktuelle Anlagenwerte überführt werden
- Systemwerte bleiben zwischen Kommunikations-, Datenbank- und Fachlogik konsistent
- Meldungs- und Historienlogik kann auf denselben Datenbestand zugreifen
- Neue Werte werden bei Bedarf automatisch angelegt

## Beobachtete Abläufe

- Telegramme erzeugen oder aktualisieren Einträge in `System_Value`.
- Werte werden über `CSystemValueExt` typisiert in ihre Zielwerte übersetzt.
- Ein interner RowVersion-Mechanismus begrenzt unnötige Folgelesevorgänge.
- Systemwerte dienen als Basis für Meldungen, Historien und Prisma-Feedback.

## Offene Fragen

- Welche Telegrammdefinitionen produktiv am wichtigsten sind
- Welche Systemwerte nur lesend und welche schreibend genutzt werden
- Wie stark die Synchronisation auf einzelne Stationen begrenzt ist
