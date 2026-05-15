# Feature: Verwaltet Übersetzungen mit Registry, Import und Export

## Beschreibung

Dieses Feature beschreibt `CTranslation` und `TTranslations` als Globalisierungs- und Übersetzungsinfrastruktur. Die Komponenten laden Übersetzungswerte, importieren sie aus einer Masterdatenbank und exportieren sie in Austauschdateien.

## Sichtbare technische Bausteine

- `Core\Core\Globalisation\CTranslation.cs`
- `Core\Core\Globalisation\TTranslations.cs`
- `Core\Core\Globalisation\ETranslTable.cs`
- `Core\Core\Globalisation\EAvailableLanguage.cs`
- `Core\Core\SControlNameHelper.cs`
- `Core\Core\SDebugHelpers.cs`

## Fachlicher Nutzen

- Übersetzungen können zentral pro Sprache verwaltet werden
- Fehlende Schlüssel werden protokolliert und sichtbar gemacht
- Masterdaten können in die lokale Übersetzungsquelle importiert werden
- Übersetzungswerte lassen sich für Austausch oder Pflege exportieren

## Beobachtete Abläufe

- `CTranslation.Load()` initialisiert die Übersetzungs-Registry und lädt Datenbankinhalte.
- `ImportFromMaster()` übernimmt Einträge aus der Masterdatenbank.
- `ExportTranslationItemsFile(...)` erzeugt eine dateibasierte Übersetzungsübersicht.
- `TTranslations.Tr(...)` liefert Texte aus der Registry und loggt fehlende Keys.
- `TTranslations.GetTT(...)` und `sTr(...)` normalisieren Eingabetexte vor der Auflösung.

## Offene Fragen

- Ob die große statische Enum-basierten Übersetzungsstruktur künftig stärker auf Ressourcendateien oder Service-APIs umgestellt werden sollte
- Ob die Master-/Lokaldatenbankbeziehung weiter entkoppelt werden sollte
