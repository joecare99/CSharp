# Feature: Konvertiert Klassenwerte in Bool und Bool-Text

## Beschreibung

Dieses Feature beschreibt die boolesche Konvertierung und die zugehörige Textdarstellung für Klassenwerte. Es wird genutzt, um deutschsprachige Ja/Nein-Repräsentationen und allgemeine Wahrheitswerte zu verarbeiten.

## Sichtbare technische Bausteine

- `Core\Core\Converter\CBoolClassConverter.cs`
- `Core.Tests\Core\Converter\CBoolClassConverterTests.cs`

## Fachlicher Nutzen

- Bool-Werte können benutzerfreundlich als `Ja`/`Nein` dargestellt werden
- Texte können robust in Bool-Werte zurückkonvertiert werden
- UI- und Konfigurationsdaten bleiben konsistent
- Tests zeigen die aktuell erwarteten Eingaben und Ausgaben

## Beobachtete Abläufe

- `ConvertTo()` gibt für `false` den Text `Nein` und für `true` den Text `Ja` zurück.
- `ConvertFrom()` akzeptiert einfache deutschsprachige Wahrheitswörter.
- Unerwartete Texte werden als `false` interpretiert.
- Die Tests dokumentieren bewusst auch unklare Eingaben wie leere Texte und unerwartete Begriffe.

## Offene Fragen

- Ob weitere sprachliche Varianten wie `True`/`False` oder `1`/`0` unterstützt werden sollen
- Wie der Converter in UI-Bindings und Konfigurationsmasken eingesetzt wird
- Ob eine einheitlichere Bool-Darstellung im restlichen System wünschenswert ist
