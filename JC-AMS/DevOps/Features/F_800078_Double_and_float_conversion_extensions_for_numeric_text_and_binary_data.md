# Feature: Bietet Double- und Float-Konvertierungs-Extensions für numerische und binäre Daten

## Beschreibung

Dieses Feature beschreibt `SAsDoubleXtntn` als Konvertierungsschicht für Double- und Float-Werte aus Objekten, Strings und Byte-Arrays.

## Sichtbare technische Bausteine

- `Core\Core\Extensions\SAsDoubleXtntn.cs`
- `Core.Tests\Core\Extensions\SAsDoubleXtntnTests.cs`
- `System.Globalization`

## Fachlicher Nutzen

- Numerische Texte werden zuverlässig in Fließkommazahlen umgewandelt
- Byte-Arrays können als binäre Repräsentation für `double` und `float` genutzt werden
- Die Konvertierung kann an die aktuelle Kultur angepasst werden

## Beobachtete Abläufe

- `AsDouble(...)` behandelt `double`, `float`, `long`, `int`, `byte[]` und `string`.
- Dezimaltrennzeichen werden an die aktuelle Kultur angepasst.
- `AsFloat(...)` nutzt entweder `BitConverter` oder die Double-Konvertierung.
- Die Tests decken Grenzwerte, NaN, Infinity und Byte-Array-Interpretation ab.

## Offene Fragen

- Ob die kulturabhängige Ersetzung von Punkt und Komma an allen Stellen gewünscht ist
- Ob die Byte-Array-Konvertierung für kurze Arrays stärker abgesichert werden sollte
