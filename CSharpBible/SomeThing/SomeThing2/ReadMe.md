# SomeThing2 – Komprimierte Labyrinth-Variante

Dies ist die stark verdichtete (obfuskierte) Ein-Datei-Version des Maze-Generators aus `SomeThing`.

## Merkmale
- Fast alles in eine einzige Code-Sequenz gepackt.
- Minimierte Bezeichner (`Z,u,O,D,B,C,l0,lQ,E,Q,h,w,P,A,S,L,a,l,ll,Il`).
- Logik entspricht funktional der kommentierten Version `SomeThing2a`.
- Nutzung bitweiser Operationen und arithmetischer Tricks zur Minimierung von Literalgröße.

## Ablauf in Kurzform
1. Initialisiert Gitter `E` (Speicherung von Wandbits + Besuchsflag mittels hoher Bitmaske `Z`).
2. Start- und Endzelle werden markiert.
3. Tiefen-/Breiten-gemischte Expansion durch zufällige Nachbarn (gesammelt in `Il`).
4. Backtracking über `ll` wenn Sackgasse.
5. Abschließendes Rendern mittels Funktion `Q(int x)` (Segmentausgabe aus codierter Bitmaske `B`).

## Rendering
`B = 0x597b` enthält 2-Bit Fragmente. `Q` extrahiert je zwei Fragmente pro Aufruf (horizontales Doppel-Segment) und baut die ASCII-Grafik auf.

## Vergleich zu `SomeThing2a`
| Aspekt | SomeThing2 | SomeThing2a |
|--------|------------|-------------|
| Lesbarkeit | Sehr gering | Hoch (Kommentare, sprechende Namen) |
| Änderbarkeit | Fehleranfällig | Robust |
| Lernwert | Zeigt Extreme von Verdichtung | Erklärt Algorithmus |

## Ausführung
```
dotnet run --project SomeThing2
```

## Empfehlung
Nutze `SomeThing2a` zum Verstehen; `SomeThing2` nur als Beispiel für Codegolf / Verdichtung / Obfuskation.

