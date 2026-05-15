# Feature: Dokumentiert die Bool-Konvertierungs-Test-Suite für truthy und falsy Werte

## Beschreibung

Dieses Feature beschreibt `SAsBoolXtntnTests` als Test-Suite für die lockere Bool-Konvertierung.

## Sichtbare technische Bausteine

- `Core.Tests\Core\Extensions\SAsBoolXtntnTests.cs`
- `Core\Core\Extensions\SAsBoolXtntn.cs`

## Fachlicher Nutzen

- Wahrheitswerte aus unterschiedlichen Objektformen bleiben nachvollziehbar
- Sonderfälle und Legacy-Interpretationen werden sichtbar abgesichert
- Die Tests zeigen die bewusst lockere Semantik der Konvertierung

## Beobachtete Testinhalte

- `null` und `DBNull` werden als `false` erwartet.
- Boolesche und String-Werte wie `true`/`false` werden direkt geprüft.
- Numerische Werte wie `1` und `-1` werden als wahr interpretiert.
- Die Suite enthält auffällige Sonderfälle und Kommentare mit offenen Fragen.

## Offene Fragen

- Ob die aktuelle Wahrheitslogik für alle Fachbereiche beibehalten werden soll
- Ob weitere Lokalisierungen wie deutsche Ja/Nein-Werte ergänzt werden müssen
