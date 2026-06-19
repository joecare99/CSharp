# Libraries.Package

Dieses Projekt erzeugt Paketartefakte für die Bibliotheken dieser Solution.

## Zweck
- baut nur Bibliotheksprojekte
- schließt Testprojekte und Console-Apps aus
- sammelt die erzeugten Bibliotheks-Assemblies in einem ZIP-Archiv
- erzeugt zusätzlich die NuGet-Pakete `CSharpBible.Libraries.Core` und `CSharpBible.Libraries.Windows`

## Verwendung
```powershell
dotnet build .\Libraries.Package\Libraries.Package.csproj -c Release -t:CreateLibrariesPackage
dotnet build .\Libraries.Package\Libraries.Package.csproj -c Release -t:CreateNuGetPackages
```

Intern wird dafür die testfreie `Libraries.Package.sln` gebaut. Danach können die Bibliotheksausgaben entweder in ein gemeinsames ZIP-Archiv unter `artifacts` geschrieben oder als `.nupkg` unter `artifacts\nuget` erzeugt werden.

## NuGet-Pakete
- `CSharpBible.Libraries.Core`
  - BaseLib
  - CommonDialogs.Abstractions
  - ConsoleDisplay
  - ConsoleLib
  - GenInterfaces
  - MathLibrary
- `CSharpBible.Libraries.Windows`
  - CommonDialogs
  - CommonDialogs_net
  - ConsoleLib_net
  - ExtendedConsole
  - MVVM_BaseLib
  - WFSystem.Windows.Data

## Enthaltene Projekte
- BaseLib
- CommonDialogs.Abstractions
- CommonDialogs
- CommonDialogs_net
- ConsoleDisplay
- ConsoleLib
- ConsoleLib_net
- ExtendedConsole
- GenInterfaces
- MathLibrary
- MVVM_BaseLib
- WFSystem.Windows.Data

## Nicht enthalten
- alle *Tests*-Projekte
- TestConsole und TestConsole_net

## Enthaltene Hilfsdateien
- `Libraries.Package.sln` enthält nur Bibliotheksprojekte
- `Libraries.Package.csproj` baut die testfreie Solution und erstellt ZIP- sowie NuGet-Artefakte
- `CSharpBible.Libraries.Core.csproj` definiert das Core-NuGet-Paket
- `CSharpBible.Libraries.Windows.csproj` definiert das Windows-NuGet-Paket

## Architekturhinweis
- `CommonDialogs.Abstractions` enthält die OS-neutralen Dialog-Verträge.
- `CommonDialogs` und `CommonDialogs_net` bleiben die Windows-spezifischen Implementierungen.
