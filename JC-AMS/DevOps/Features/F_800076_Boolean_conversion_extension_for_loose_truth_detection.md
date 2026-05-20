# Feature: Bietet eine boolesche Konvertierungs-Extension mit lockerer Wahrheitslogik

## Beschreibung

Dieses Feature beschreibt `SAsBoolXtntn` als kleine Erweiterung zur booleschen Auswertung verschiedenster Objektformen.

## Sichtbare technische Bausteine

- `Core\Core\Extensions\SAsBoolXtntn.cs`
- `Core.Tests\Core\Extensions\SAsBoolXtntnTests.cs`
- `Core\Core\Extensions\SAsIntXtntn.cs`
- `Core\Core\Extensions\SAsNumericXtntn.cs`

## Fachlicher Nutzen

- Bool-Werte können aus Texten und numerischen Ersatzwerten erkannt werden
- Legacy-Daten mit inhomogenen Typen lassen sich einfacher interpretieren
- Die Konvertierungslogik bleibt an einer Stelle gebündelt

## Beobachtete Abläufe

- `null` und `DBNull` werden als `false` interpretiert.
- Echte boolesche Werte werden direkt zurückgegeben.
- Texte mit `true` oder `false` werden per Teilstring-Auswertung erkannt.
- `1` und `-1` werden als wahr interpretiert, andere Werte als falsch.
- Die Tests decken Groß-/Kleinschreibung und auffällige Sonderfälle ab.

## Offene Fragen

- Ob die lockere Wahrheitslogik fachlich überall beabsichtigt ist
- Ob deutsche Sprachmuster wie `JA` und `NEIN` ebenfalls explizit unterstützt werden sollten
