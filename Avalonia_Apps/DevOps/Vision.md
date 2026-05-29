# 🚀 Projektübersicht: AA98_AvlnCodeStudio

## 🎯 Vision
AA98_AvlnCodeStudio soll schrittweise als plattformunabhängige C#-Entwicklungsumgebung mit Avalonia UI entstehen. Das langfristige Ziel ist ein fokussierter Visual-Studio-Ersatz für die .NET- und Mono-Welt, der lokale und entfernte KI-gestützte Entwicklungsunterstützung integrieren kann. Die Lösung soll von Anfang an modular, erweiterbar und für inkrementelle Ausbaustufen ausgelegt sein.

Jeder Umsetzungsschritt soll in sich benutzbar sein und gleichzeitig das Gesamtframework dem Zielbild näherbringen. Statt eines großen Rohbaus soll das System in kleinen, abgeschlossenen und direkt nutzbaren Inkrementen wachsen.

## 👥 Zielgruppen
- Einzelentwickler
- Kleine Teams
- Entwickler mit lokaler KI-Nutzung
- Entwickler mit Remote-KI-Nutzung, beispielsweise Copilot-ähnlichen Diensten

## 🛠️ Technologie-Stack
| Komponente | Technologie | Zweck |
|-|-|-|
| Frontend/UI | Avalonia UI | Plattformübergreifende Benutzeroberfläche |
| Backend/Logik | C# (.NET) | Kernlogik der IDE und Integrationsschichten |
| Architektur | MVVM | Saubere, testbare Trennung von UI und Logik |
| Kommunikation mit KI | HTTP / gRPC / später erweiterbar | Verbindung zu lokalen oder entfernten KI-Diensten |
| Zielplattform | Linux-first | Primäre Zielplattform bei offener Architektur für weitere Plattformen |

## 📦 Erstes benutzbares Inkrement / MVP
Das erste benutzbare Inkrement soll einen komponentenbasierten Editor bereitstellen für:
- `C#`
- `.axaml`
- `.md`

Optional kann `json` früh mitgedacht werden, ist aber nicht zwingend Teil des ersten Inkrements.

Das erste Inkrement soll insbesondere Folgendes leisten:
- Dateien öffnen, bearbeiten und speichern
- Syntax-Highlighting für die primären unterstützten Dateitypen
- eine aus Komponenten aufgebaute Architektur bereitstellen
- eine benutzeranpassbare Oberfläche mit Docking- und Layout-Grundlagen besitzen
- die spätere KI-Integration architektonisch vorbereiten, aber noch nicht voraussetzen

## 🧩 Komponentenmodell
Das Framework soll aus lose gekoppelten Komponenten aufgebaut werden. Jede Komponente soll eigenständig nutzbare Funktionalität in das Gesamtsystem einbringen können.

Komponenten können insbesondere beitragen:
- Commands für Menü, Toolbar oder Tab-Leiste
- UI-Elemente, insbesondere dockbare UserControls oder vergleichbare Oberflächenmodule
- Konfigurationswerte und konfigurierbare Optionen
- spätere Integrationspunkte für editorbezogene Aktionen, Toolfenster und Provider

Das Komponentenmodell ist ein Kernbestandteil der Gesamtarchitektur und soll frühe Erweiterbarkeit sicherstellen.

## 🚫 Nicht-Ziele der frühen Schritte
Die ersten Inkremente sollen bewusst noch kein vollständiger Visual-Studio-Ersatz sein.

Ausdrücklich nicht Ziel der ersten Ausbaustufen ist:
- vollständige Abdeckung aller Visual-Studio-Funktionen
- breite Sprachunterstützung jenseits des initialen Fokus
- tiefe KI-Automatisierung ohne vorherige transparente Benutzerkontrolle

Der anfängliche fachliche Fokus liegt auf:
- `C#`
- `.axaml`
- `.md`
- `.resx`
- optional später `json`

## 🏗️ Architekturleitlinien
- modular und komponentenbasiert
- MVVM für UI-nahe Anwendungsstruktur
- Dependency Injection für lose Kopplung und Testbarkeit
- klare Trennung zwischen UI, Anwendungslogik und Infrastruktur/Providern
- Erweiterbarkeit für lokale und entfernte KI-Anbieter
- Linux-first, ohne die Architektur unnötig auf nur eine Plattform zu beschränken
- vorbereitete Integrationspunkte für standardisierte externe Tool- und KI-Protokolle
- starke Lokalisierbarkeit von Anfang an, insbesondere durch ressourcenbasierte UI- und Benutzerttexte

## 🔁 Inkrementelle Umsetzungsstrategie
Die Lösung soll als Framework Stück für Stück wachsen.

Für jedes Inkrement gilt:
- es liefert einen in sich benutzbaren Zustand
- es schafft klaren Mehrwert für den Anwender
- es erweitert die bestehende Architektur gezielt statt sie zu ersetzen
- es bereitet den nächsten sinnvollen Ausbauschritt vor

Bevorzugt werden vertikale, direkt nutzbare Ausbaustufen statt großer technischer Vorarbeiten ohne unmittelbaren Nutzen.

## ✅ Qualitätsattribute
- testbar
- modular erweiterbar
- stabile Grundfunktionen vor Feature-Breite
- responsive Benutzeroberfläche
- benutzeranpassbare Oberfläche
- speicherbares Layout
- Docking als zentraler Teil des UI-Konzepts
- klare Verantwortlichkeiten zwischen UI, Logik und Integrationsschichten
- lokalisierbar und sprachneutral in UI-nahen Textdefinitionen

## 🌍 Lokalisierbarkeit und Ressourcen
- Benutzer- und UI-Texte sollen von Anfang an aus Ressourcen kommen statt direkt im Code oder XAML hartcodiert zu werden.
- Die Architektur soll Ressourcen als regulären, erweiterbaren Bestandteil des Frameworks behandeln.
- Komponenten sollen mittelfristig eigene lokalisierbare Ressourcen beitragen können.
- Mittelfristig soll das Framework auch Unterstützung für das Anzeigen, Bearbeiten und Pflegen von Ressourcendateien bieten.
- Frühe Inkremente sollen bereits so gestaltet werden, dass spätere Mehrsprachigkeit keine grundlegende Umstrukturierung der UI- oder Konfigurationslogik erfordert.

## 🔐 Sicherheits- und Datenschutzgrundsätze
Sicherheits- und Datenschutzentscheidungen sollen bewusst in der Hand des Benutzers liegen.

Grundprinzipien:
- Opt-in statt automatischer Datenweitergabe
- der Benutzer kontrolliert, welche Informationen an externe Provider gesendet werden
- lokale und entfernte KI-Nutzung sollen transparent unterscheidbar sein
- die Oberfläche und Konfiguration sollen sowohl für einfache Anwender als auch für erfahrene Profi-Entwickler verständlich nutzbar sein

Für die Steuerung der Datenfreigabe sollen mehrere Stufen vorgesehen werden:
1. Einfache Stufe mit explizit ausgelösten Aktionen
2. Fortgeschrittene Stufe mit Freigaben pro Datei, Auswahl oder Aktion
3. Expertenstufe mit feingranularen Regeln für Kontext, Inhalte und Provider-Verhalten

## 🤖 KI- und Provider-Strategie
Die frühe Architektur soll KI-Integration vorbereiten, ohne sie im ersten benutzbaren Inkrement bereits zwingend vollständig umzusetzen.

Geplante Ausbaustufen:
1. Architektur für lokale und entfernte KI-Provider vorbereiten
2. Chat-Komponente mit lokaler KI als früher nutzbarer Ausbau
3. Aktionen auf Selektion oder Dateikontext für KI-gestützte Unterstützung
4. Erweiterte Provider- und Protokollintegration

Früh mitzudenken ist insbesondere die mögliche Unterstützung standardisierter Integrationsansätze wie MCP, damit das Framework offen für spätere Tool- und Modellanbindungen bleibt.

## 📏 Erfolgskriterien für frühe Inkremente
Ein frühes Inkrement ist erfolgreich, wenn es:
- als Anwendung startbar und nutzbar ist
- Dateien der Kernformate öffnen, bearbeiten und speichern kann
- Syntax-Highlighting für die initialen Formate bereitstellt
- Komponentenregistrierung für Commands, UI-Elemente und Konfiguration ermöglicht
- eine benutzeranpassbare Oberfläche mit Docking-Grundlagen unterstützt oder vorbereitet
- die KI-Integration architektonisch sauber vorbereitet, ohne sie bereits zu erzwingen
- UI- und Benutzerttexte so platziert, dass eine spätere Lokalisierung ohne grundlegenden Umbau möglich bleibt

## 🧭 Langfristiges Zielbild
Langfristig soll AA98_AvlnCodeStudio ein fokussierter, moderner und erweiterbarer Entwicklungsarbeitsplatz für die .NET- und Mono-Welt werden. Der Schwerpunkt liegt dabei auf einer modularen Architektur, einer anpassbaren Oberfläche, klarer Benutzerkontrolle über KI-Integration sowie einer schrittweisen Entwicklung zu einem praxistauglichen Visual-Studio-Ersatz im definierten Zielbereich.

## Epics und Features
Die untergeordneten Dokumente in diesem Bereich konkretisieren die Vision schrittweise und dienen der strukturierten Ausarbeitung, Planung und Weiterentwicklung der fachlichen und technischen Umsetzung. Sie werden iterativ ergänzt, verfeinert und angepasst, um Anforderungen, Arbeitspakete und Fortschritt nachvollziehbar zu dokumentieren.Features sind unter .\Features\*.md definiert und bilden die konkreten Umsetzungsschritte für die einzelnen Epics ab. Sie werden iterativ erweitert und angepasst, um den Fortschritt der Entwicklung zu steuern und sicherzustellen, dass jedes Inkrement einen klaren Mehrwert bietet.