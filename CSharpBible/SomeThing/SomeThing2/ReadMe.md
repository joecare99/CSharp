# SomeThing2 � Komprimierte Labyrinth-Variante

Dies ist die stark verdichtete (obfuskierte) Ein-Datei-Version des Maze-Generators aus `SomeThing`.

## Merkmale
- Fast alles in eine einzige Code-Sequenz gepackt.
- Minimierte Bezeichner (`Z,u,O,D,B,C,l0,lQ,E,Q,h,w,P,A,S,L,a,l,ll,Il`).
- Logik entspricht funktional der kommentierten Version `SomeThing2a`.
- Nutzung bitweiser Operationen und arithmetischer Tricks zur Minimierung von Literalgr��e.

## Ablauf in Kurzform
1. Initialisiert Gitter `E` (Speicherung von Wandbits + Besuchsflag mittels hoher Bitmaske `Z`).
2. Start- und Endzelle werden markiert.
3. Tiefen-/Breiten-gemischte Expansion durch zuf�llige Nachbarn (gesammelt in `Il`).
4. Backtracking �ber `ll` wenn Sackgasse.
5. Abschlie�endes Rendern mittels Funktion `Q(int x)` (Segmentausgabe aus codierter Bitmaske `B`).

## Rendering
`B = 0x597b` enth�lt 2-Bit Fragmente. `Q` extrahiert je zwei Fragmente pro Aufruf (horizontales Doppel-Segment) und baut die ASCII-Grafik auf.

## Vergleich zu `SomeThing2a`
| Aspekt | SomeThing2 | SomeThing2a |
|--------|------------|-------------|
| Lesbarkeit | Sehr gering | Hoch (Kommentare, sprechende Namen) |
| �nderbarkeit | Fehleranf�llig | Robust |
| Lernwert | Zeigt Extreme von Verdichtung | Erkl�rt Algorithmus |

## Ausf�hrung
```
dotnet run --project SomeThing2
```

## Empfehlung
Nutze `SomeThing2a` zum Verstehen; `SomeThing2` nur als Beispiel f�r Codegolf / Verdichtung / Obfuskation.

