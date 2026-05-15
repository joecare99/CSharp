# Feature: Verwaltet Systemwert-Definitionen und Metadaten

## Beschreibung

Dieses Feature beschreibt `CSystemValueDef` und `CSystemValueDefExt` als Metadatenmodell für Systemwerte. Es legt fest, wie Beschreibungen, Datentypen, Stationen und Array-Informationen verwaltet werden.

## Sichtbare technische Bausteine

- `Core\Core\System\Values\CSystemValueDef.cs`
- `Core\Core\System\Values\CSystemValueDefExt.cs`
- `Core\Core\System\CStation.cs`
- `Core\Core\System\CSubStation.cs`

## Fachlicher Nutzen

- Systemwerte besitzen definierte Typen und Stationszuordnungen
- Array-basierte Werte werden über Indexgrenzen modelliert
- Bekannte Value-Definitions können als zentrale Konstanten adressiert werden
- Metadaten werden für UI, SQL und Kommunikationslogik wiederverwendet

## Beobachtete Abläufe

- `CSystemValueDef` hält Beschreibung, Typ, Station und Arraygrenzen.
- `CSystemValueDefExt` ergänzt IDs, statische Registries und viele vordefinierte Value-Definitionen.
- `ArraySize` und `IsArray` leiten sich aus den Indexgrenzen ab.
- Benannte Properties wie `TelTime`, `Now` oder `IsOnline` repräsentieren häufige Systemwerte.

## Offene Fragen

- Ob die umfangreiche Liste statischer Properties künftig besser als Konfiguration oder Enum modelliert werden sollte
- Ob die Initialisierung der Definitions-Registry vollständig dokumentiert oder abstrahiert werden sollte
