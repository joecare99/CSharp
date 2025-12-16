# Quine - Ein selbstreplizierendes Programm

## Was ist ein Quine?

Ein **Quine** ist ein Computerprogramm, das seinen eigenen Quellcode als Ausgabe erzeugt, ohne dabei die eigene Quelldatei zu lesen. Der Name stammt vom Philosophen und Logiker Willard Van Orman Quine.

## Wie funktioniert dieses Quine?

Das Programm verwendet eine klassische Quine-Technik mit zwei Hauptkomponenten:

### Der Aufbau

´´´csharp
var s = "var s = {0}{1}{0};{2}Console.Write(s, (char)34, s, Environment.NewLine);"; 
Console.Write(s, (char)34, s, Environment.NewLine);
´´´ 

### Erklärung der Funktionsweise

1. **Die Variable `s`**: Enthält ein Template des gesamten Programms mit Platzhaltern:
   - `{0}` - Platzhalter für das Anführungszeichen (`"`)
   - `{1}` - Platzhalter für den String `s` selbst
   - `{2}` - Platzhalter für den Zeilenumbruch

2. **`(char)34`**: Der ASCII-Code für das doppelte Anführungszeichen (`"`). Dies ist notwendig, um das "Henne-Ei-Problem" zu umgehen - wir können kein `"` direkt im String verwenden, ohne in Escape-Probleme zu geraten.

3. **`Console.Write(s, (char)34, s, Environment.NewLine)`**: 
   - Gibt den String `s` aus
   - Ersetzt `{0}` durch `"`
   - Ersetzt `{1}` durch den kompletten String `s`
   - Ersetzt `{2}` durch einen Zeilenumbruch

### Das Ergebnis

Wenn das Programm ausgeführt wird, gibt es exakt seinen eigenen Quellcode aus:

´´´csharp
var s = "var s = {0}{1}{0};{2}Console.Write(s, (char)34, s, Environment.NewLine);"; 
Console.Write(s, (char)34, s, Environment.NewLine);
´´´ 

## Ausführen

´´´bash
dotnet run
´´´

Die Ausgabe sollte identisch mit dem Inhalt von `Program.cs` sein.

