# Feature: Bietet WinAPI-Interop für Ini-Profile und Nachrichtenversand

## Beschreibung

Dieses Feature beschreibt `SWinAPI` als kleine Interop-Hilfsklasse für Win32-Funktionen rund um Ini-Dateien und Fensternachrichten.

## Sichtbare technische Bausteine

- `Core\Core\SWinAPI.cs`
- `kernel32.dll`
- `user32.dll`

## Fachlicher Nutzen

- Ini-Dateien können ohne eigene Parser direkt gelesen und beschrieben werden
- Fensternachrichten lassen sich über das Win32-API senden
- Die Klasse kapselt zentrale P/Invoke-Signaturen an einer Stelle

## Beobachtete Abläufe

- `GetPrivateProfileString(...)` liest Werte aus Ini-Dateien.
- `WritePrivateProfileString(...)` schreibt Werte in Ini-Dateien.
- `SendMessage(...)` stellt den direkten Zugriff auf das Win32-Message-API bereit.

## Offene Fragen

- Ob die Interop-Signaturen an moderneres .NET-API-Design angepasst werden sollten
- Ob zusätzliche Win32-Hilfen in dieselbe Klasse gehören oder separat dokumentiert werden sollten
