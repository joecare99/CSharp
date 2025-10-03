# SomeThing2a � Lesbare & Kommentierte Labyrinth-Generation

Dies ist die didaktisch aufbereitete Version des ASCII-Labyrinth-Generators. Sie basiert funktional auf der kompakten Fassung (`SomeThing2`) und der Zwischenform (`SomeThing`), bietet aber:
- Sprechende Variablennamen
- Kommentare pro Logikabschnitt
- Optionale Generations-Animation via `#define ShowGen`

## Grundprinzip
Erzeugt ein perfektes (zyklusfreies) Labyrinth der Gr��e `cellCount x cellCount` durch zuf�llige Verbindung unbesuchter Nachbarn (Randomized Depth-First mit implizitem Backtracking �ber Warteschlange / Stack-Ersatz).

## Zentrale Komponenten
| Name | Bedeutung |
|------|-----------|
| `grid` | Integer-Array f�r Zellen; Wandbits + Besuchsflag gespeichert |
| `cellSpacing` | Richtungs-Basiswert (1) zur Bitverschiebung |
| `rowOffsets / colOffsets` | Hilfsarrays zur Nachbarberechnung |
| `queue` | Speicherung bereits erreichter Zellen (Backtracking) |
| `queue2` | Tempor�re Liste m�glicher Richtungen |
| `housePattern` | Codierte 2-Bit Segmente zur ASCII-Ausgabe |
| `PrintHouse` | Rendert zwei horizontale Mini-Segmente oder Zeilenumbruch |

## Wand-/Statuskodierung
- Besuchsbit: `gridSize (1<<11)`
- Wand nach Richtung d: `(cellSpacing << d)`
- Gegenrichtung: `(d + 2) % 4`

## Algorithmus-Schritte (Schleife)
1. Aktuelle Zelle bestimmen (`currentCell`).
2. Alle vier Richtungen pr�fen:
   - Innerhalb Grenzen?
   - Noch nicht besucht?
   - Seitenversatz passt (kein Wrap)?
3. Wenn Kandidaten vorhanden:
   - Zuf�llige Richtung w�hlen
   - Wand in aktueller + Gegenwand in Nachbar setzen
   - Nachbar in `queue` schieben (Erweiterung)
4. Sonst: Backtracking � �lteste gespeicherte Zelle aus `queue` erneut bearbeiten.

## Ausgabe
Nach Fertigstellung iteriert der Code �ber alle Zellen und ruft `PrintHouse` mit passenden Parametern auf. Dadurch entsteht ein zusammengesetztes Raster aus Leerzeichen, Unterstrichen und vertikalen Strichen.

## Animation (`ShowGen`)
Ist die Direktive aktiv und die Ausgabe nicht umgeleitet, wird jede verarbeitete Zelle live dargestellt (mit Cursorpositionierung & kurzer Verz�gerung `Thread.Sleep(10)`).

## Ausf�hrung
```
dotnet run --project SomeThing2a
```

## Erweiterungen
- Pfadfinder (A* oder BFS) zur Markierung einer L�sung.
- Umstellung auf Unicode Box-Drawing ("????" etc.).
- Steuerbarer Seed �ber Programmargumente.
- Interaktive Variante (Spieler bewegt sich durchs Labyrinth).

## Testbarkeit
Die Hilfsklasse `Rnd` kapselt `Random` und erlaubt durch Setzen von `Rnd.GetRnd` deterministische Tests.

