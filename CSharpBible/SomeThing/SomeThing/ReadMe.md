# SomeThing – Labyrinth / Maze Generator (Obfuscated vs. Klar)

Dieses Projekt enthält eine generative Konsolen-Ausgabe eines kleinen ASCII-Labyrinths in drei Varianten:
1. `SomeThing` – halb kryptische, leicht umbenannte Version mit Direktiven.
2. `SomeThing2` – stark verdichtete, fast einzeilige Fassung (Codegolf / Obfuskation).
3. `SomeThing2a` – aufgeräumte und kommentierte Fassung mit denselben Algorithmen.

Alle drei erzeugen dasselbe Grundprinzip: Ein perfektes Labyrinth (ohne Zyklen) durch Tiefen-/Breiten-Mix mittels Randomized Backtracking.

## Kern-Idee des Algorithmus
- Spielfeldgröße: Quadratisches Gitter (`CellPerDimension x CellPerDimension`).
- Jede Zelle hat bis zu 4 Wände (Bit-Codierung mit Richtungsbits 0..3).
- Start- und Ziel-/Endknoten werden markiert; das Labyrinth wird durch zufällige Wahl unbesuchter Nachbarn erweitert.
- Datenstruktur: FIFO / Liste dient als Rücksprungstruktur (Backtracking-Light).
- Abbruch: Wenn keine offenen Expansionsrichtungen mehr vorhanden sind.

## Wichtige Konstanten (Variante `SomeThing`)
- `CellPerDimension` = 39 (aus Rechen-Trick `(0x13 << 1) + 1`).
- `One` = 1 (Bit-Trick für generische Ableitung).
- `Eight` = 8 (hier als Masken-/Layoutbasis für Ausgabe benutzt).
- Wand-/Richtungsbits: `(One << dir)`.
- Besuchsmarkierung: Höheres Bit (`T2k = 1<<11`) wird mitgeführt.

## Ablauf (vereinfacht)
1. Initialisiere Array `Labyrinth` mit 0.
2. Markiere Start (Zelle 0) und Ziel (letzte Zelle) speziell.
3. Iteriere:
   - Sammle alle gültigen, noch nicht verbundenen Nachbarn.
   - Falls vorhanden: Wähle zufällig eine Richtung, öffne Wand in aktueller und Gegenwand in Nachbarzelle, pushe Nachbar in FIFO.
   - Falls nicht: Pop aus FIFO zum Zurückspringen.
4. Nach Abschluss: Ausgabe des Gitters als ASCII mit Stück-Renderer `WOut` / `PrintHouse`.

## Ausgabeprinzip
Die Variable `TextCode / housePattern` encodiert ein 2?Bit Muster für Segmentfragmente (" ", "_", "|") und wird positionsabhängig geshiftet.
Jedes aufgerufene Rendering verarbeitet zwei horizontale Halb-Zellen und erzeugt so Linien + Wände.

## Unterschiede der Varianten
| Datei | Stil | Zweck |
|-------|------|-------|
| `SomeThing/Program.cs` | Semantisch leicht lesbar, aber noch kryptische Namen | Ausgangsbasis / Demo mit Direktiven (`#define ShowGen`) für Live-Generierung |
| `SomeThing2/Program.cs` | Stark komprimiert, wenig Zeilenumbrüche | Codegolf / Minimaldarstellung |
| `SomeThing2a/Program.cs` | Kommentiert, sprechende Variablen, zusätzliche Lesbarkeit | Didaktische Variante |

## Direktive `ShowGen`
Wenn aktiv, wird die Labyrinth-Generierung animiert: Jede gesetzte Zelle wird mit einem 2?Zeichen-Fragment aktualisiert (`SetCursorPosition`, kleine Pause via `Thread.Sleep`).

## Klasse `Rnd`
Eigene Wrapper-Klasse um `System.Random` mit injizierbarer Fabrik (`GetRnd` Delegate) – erleichtert Testbarkeit / Determinismus in Unit-Tests.

## Build & Run
```
dotnet run --project SomeThing
# oder
dotnet run --project SomeThing2
# oder
dotnet run --project SomeThing2a
```

## Erweiterungsideen
- Seed-Übergabe über Argumente für reproduzierbare Generation.
- Export als Unicode (Box Drawing) statt ASCII.
- Pfadlösung (Start?Ziel) farblich hervorheben.
- Alternative Algorithmen: Prim, Kruskal, Recursive Division.
- Optional: Interaktive Spieler-Steuerung durch das Labyrinth.

## Lizenz / Nutzung
Demonstrationscode für Lern- und Experimentierzwecke.
