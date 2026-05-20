# Feature: Definiert verfügbare Sprachen und Übersetzungstabelle als Enums

## Beschreibung

Dieses Feature beschreibt `EAvailableLanguage` und `ETranslTable` als Grundlage für sprach- und termbasierte Globalisierung im Core-Bereich.

## Sichtbare technische Bausteine

- `Core\Core\Globalisation\EAvailableLanguage.cs`
- `Core\Core\Globalisation\ETranslTable.cs`
- `Core\Core\Globalisation\TTranslations.cs`

## Fachlicher Nutzen

- Sprachen können über LCIDs eindeutig adressiert werden
- UI-Texte und fachliche Begriffe werden über feste Term-IDs referenziert
- Die Enums dienen als gemeinsame, typsichere Basis für Übersetzungszugriffe

## Beobachtete Abläufe

- `EAvailableLanguage` enthält mehrere LCID-basierte Sprachdefinitionen.
- `ETranslTable` listet zahlreiche Übersetzungstermine aus UI- und Fachkontexten.
- Die Übersetzungslogik greift über `TTranslations.Tr(...)` auf diese Werte zu.

## Offene Fragen

- Ob die Enumerationsliste vollständig genug für alle UI- und Fachtextfälle ist
- Ob die Term-IDs noch zentraler dokumentiert oder generiert werden sollten
