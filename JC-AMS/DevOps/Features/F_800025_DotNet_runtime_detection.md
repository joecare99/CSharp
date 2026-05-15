# Feature: Erkennt die .NET-Runtime-Version

## Beschreibung

Dieses Feature beschreibt die Hilfen zur Erkennung und Prüfung der installierten .NET-Runtime-Version. Die Lösung nutzt diese Informationen für Plattform- und Startlogik.

## Sichtbare technische Bausteine

- `Core\Core\SDotNetUtil.cs`
- `Core.Tests\Core\SDotNetUtilTests.cs`

## Fachlicher Nutzen

- Die Laufzeitumgebung kann vor dem Start geprüft werden
- Versionsabhängige Entscheidungen lassen sich zentral treffen
- Start- und Kompatibilitätsprobleme werden früher erkannt
- Tests dokumentieren die erwartete Mindest- und Nebenversionenlogik

## Beobachtete Abläufe

- `CheckDotNETVersion()` prüft eine Zielversion gegen installierte Framework-Werte.
- `GetDotNETVersion()` liefert Version und Service-Pack-Informationen aus der Umgebung.
- Die Tests zeigen, dass 3.5 und 4.0 explizit als Prüffälle genutzt werden.

## Offene Fragen

- Ob diese Logik nur für Legacy-Systeme oder auch für neue Startpfade relevant ist
- Welche Mindestversionen im gesamten Projekt wirklich gefordert werden
- Ob die Laufzeiterkennung für .NET 6+ analog erweitert werden soll
