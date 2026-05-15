# Feature: Misst Netzwerklatenz mit Ping-Hilfen

## Beschreibung

Dieses Feature beschreibt `CPing` als Hilfsklasse zur periodischen Ping-Messung mit statistischer Auswertung.

## Sichtbare technische Bausteine

- `Core\Core\Communication\CPing.cs`
- `Core\Core\Math2\CArithmeticAverage.cs`
- `System.Net.NetworkInformation.Ping`

## Fachlicher Nutzen

- Erreichbarkeit und Antwortzeiten von Hosts können beobachtet werden
- Durchschnitt, Minimum und Maximum werden protokolliert
- Ping-Serien lassen sich zeitgesteuert ausführen

## Beobachtete Abläufe

- Der Konstruktor initialisiert Ziel, Intervall, Statistik und Timer.
- `OnPing(...)` sendet wiederholt Ping-Anfragen und aktualisiert Statistikwerte.
- Erfolgreiche Antworten werden mit Roundtrip und TTL geloggt.
- Am Ende einer Serie werden zusammenfassende Statistikwerte ausgegeben.

## Offene Fragen

- Ob die Ping-Hilfsklasse heute noch in produktiven Pfaden genutzt wird
- Ob die Timerlogik robuster auf Wiederverwendung geprüft werden sollte
