# CommonDialogs.Abstractions

Dieses Projekt enthält die OS-neutralen Verträge für CommonDialogs.

## Zweck
- trennt plattformneutrale Dialog-Abstraktionen von den Windows-Implementierungen
- dient als Core-Paket für DI und Referenzierung ohne WPF- oder WinForms-Abhängigkeiten

## Enthalten
- `IFileDialog`
- `IOpenFileDialog`
- `IColorDialog`
- `IFontDialog`
- `DialogColor`
- `FontDialogSelection`

`DialogColor` und `FontDialogSelection` entkoppeln die Core-Verträge von `System.Drawing` und können daher auch von nicht-Windows-spezifischen UI-Schichten verwendet werden.

## Nicht enthalten
- `IPrintDialog`

`IPrintDialog` bleibt vorerst im Windows-spezifischen Bereich, da der Vertrag direkt WPF-/Printing-Typen referenziert.
