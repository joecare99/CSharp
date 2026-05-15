# Feature: Dokumentiert die Double-/Float-Konvertierungs-Test-Suite für Fließkommawerte

## Beschreibung

Dieses Feature beschreibt `SAsDoubleXtntnTests` als Test-Suite für die Double- und Float-Konvertierung aus verschiedenen Objektformen.

## Sichtbare technische Bausteine

- `Core.Tests\Core\Extensions\SAsDoubleXtntnTests.cs`
- `Core\Core\Extensions\SAsDoubleXtntn.cs`

## Fachlicher Nutzen

- Fließkomma-Konvertierungen bleiben reproduzierbar
- Kultur- und Byte-Array-Verhalten werden dokumentiert
- Grenzwerte wie NaN, Infinity und Max/Min bleiben testbar

## Beobachtete Testinhalte

- `AsDouble(...)` wird über mehrere API-Pfade geprüft.
- `AsFloat(...)` wird mit analogen Datenreihen getestet.
- Die Suite enthält spezielle Byte-Array-Fälle und wissenschaftliche Notationen.
- Kommentare markieren bewusst offene bzw. umstrittene Randfälle.

## Offene Fragen

- Ob die Kulturabhängigkeit der Dezimaltrennzeichen in allen Umgebungen stabil genug ist
- Ob `NaN` und `Infinity` für alle Datenquellen gleich behandelt werden sollen
