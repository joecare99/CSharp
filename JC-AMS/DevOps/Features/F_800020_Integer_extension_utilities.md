# Feature: Stellt Integer-Erweiterungen bereit

## Beschreibung

Dieses Feature beschreibt die Erweiterungsmethoden zur Konvertierung von Werten und Texten in Integer- und Long-Werte sowie zur Prüfung auf Ganzzahlen.

## Sichtbare technische Bausteine

- `Core\Core\Extensions\SAsIntXtntn.cs`
- `Core.Tests\Core\Extensions\SAsIntXtntnTests.cs`

## Fachlicher Nutzen

- Unterschiedliche Eingabetypen lassen sich robust in Ganzzahlen überführen
- Textwerte können auf Ganzzahlform geprüft werden
- Die Konvertierung liefert definierte Defaultwerte statt Ausnahmen
- Tests dokumentieren die Grenzfälle und Rundungs-/Konvertierungsregeln

## Beobachtete Abläufe

- `AsInt32()` und `AsInt64()` versuchen die Konvertierung über `Convert` bzw. Parsen.
- Ungültige Eingaben liefern `0` oder `0L`.
- `IsInt()` prüft Textwerte gegen ein Ganzzahlmuster.
- Die Tests zeigen die aktuelle Behandlung von `null`, `DBNull`, Fließkommawerten und Extremwerten.

## Offene Fragen

- Ob die String-Konvertierung bei lokalen Dezimal- oder Exponentialformaten erweitert werden soll
- Ob die Rückgabe von `0` als Default überall fachlich gewünscht ist
- Ob ähnliche Hilfen für weitere Zahltypen notwendig sind
