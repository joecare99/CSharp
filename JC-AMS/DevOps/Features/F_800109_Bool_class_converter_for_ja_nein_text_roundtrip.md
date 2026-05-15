# Feature: Konvertiert Bool-Werte in Ja/Nein-Text und zurück

## Beschreibung

Dieses Feature beschreibt `CBoolClassConverter` als kleine Bool-Konverterklasse für deutschsprachige Ja/Nein-Darstellung.

## Sichtbare technische Bausteine

- `Core\Core\Converter\CBoolClassConverter.cs`
- `Core.Tests\Core\Converter\CBoolClassConverterTests.cs`
- `System.ComponentModel.BooleanConverter`

## Fachlicher Nutzen

- Bool-Werte können in UI-Kontexten als Ja/Nein angezeigt werden
- Rückkonvertierung bleibt für einfache Formulare und Designer-Umgebungen möglich
- Der Konverter passt zu deutschsprachigen Legacy-Oberflächen

## Beobachtete Abläufe

- `ConvertTo(...)` mappt `true` auf `Ja` und `false` auf `Nein`.
- `ConvertFrom(...)` interpretiert nur den Text `Ja` als wahr.
- Die Tests prüfen beide Richtungen mit typischen Werten.

## Offene Fragen

- Ob die harte Sprachbindung an `Ja`/`Nein` noch gewünscht ist
- Ob der Konverter mit weiteren Sprachvarianten erweitert werden sollte
