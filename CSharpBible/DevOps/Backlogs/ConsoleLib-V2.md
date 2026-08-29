# ConsoleLib V2 Backlog

## Zweck

Dieses Backlog exportiert die Metaaufgaben des ConsoleLib-V2-Plans in
Scrum-/DevOps-Form. Die umsetzbaren Einzelaufgaben liegen in
`DevOps/Tasks/ConsoleLib-V2-*.md`.

## Statusübersicht

| Metaaufgabe | Enthaltene Tasks | Status |
| --- | --- | --- |
| V2-Fundament und Interaktion | CL-01, CL-02, CL-10, CL-12, CL-13 | Abgeschlossen |
| POSIX-Terminal und Rendering | CL-03, CL-04, CL-11, CL-14, CL-15 | Abgeschlossen |
| CXAML-Laufzeit und Tooling | CL-05, CL-06, CL-07, CL-08 | Abgeschlossen |
| Release und Qualität | CL-09 | Abgeschlossen |
| Abschlusskorrekturen | CL-16, CL-17, CL-18, CL-19 | **Blockiert** |

## Metaaufgabe: V2-Fundament und Interaktion

**Umfang:** Provider-neutrale Verträge, Fokus- und Tastatursteuerung,
Textbearbeitung, Layoutcontainer sowie robuste Render-/Anwendungsgrundlagen.

**Status:** Abgeschlossen.

**Details:** Die Grundlage umfasst immutable Input- und Capability-Verträge,
FocusManager, Textauswahl und Wortnavigation, Grid/DockPanel/StackPanel,
Dispatcher/Scheduler/Clock, Renderkontext, Unicode-Textlayout und explizite
Fehlerdiagnostik.

## Metaaufgabe: POSIX-Terminal und Rendering

**Umfang:** ANSI/VT-Transport, Raw-Mode-Lifecycle, Keyboard-/Mouse-Eingabe,
Collection-/Form-Rendering sowie backendbezogene Regressionstests.

**Status:** Abgeschlossen.

**Details:** Der POSIX-Stack unterstützt UTF-8 ohne BOM, Cancellation,
Resize, SGR-Mouse-Demultiplexing, Tree/Tile/Form-Widgets und gezielte
Coverage-Härtung. Die ExtendedConsole-Oberfläche bleibt separat.

## Metaaufgabe: CXAML-Laufzeit und Tooling

**Umfang:** CXAML-Loader, Codegenerator, Designer und parallele
Referenzanwendungen.

**Status:** Abgeschlossen.

**Details:** CXAML validiert Struktur und Werte, erzeugt deterministische
C#-Factories, bietet Live-Preview/Inspector und wird in vier unabhängigen
Beispielprojekten verwendet, ohne imperative Anwendungen zu ersetzen.

## Metaaufgabe: Release und Qualität

**Umfang:** Testprojektaufteilung, Coverage-Erfassung, Dokumentation,
Migrations- und Betriebsleitfäden.

**Status:** Abgeschlossen.

**Details:** Der frühere dokumentierte Gesamtstand von 164 Tests ist veraltet.
In der aktuellen Arbeitskopie bestanden alle fünf Split-Projekte mit 132 + 17
+ 24 + 6 + 3 = **182 Tests**. Für alle fünf Läufe wurden frische Cobertura-
Reports erzeugt. Der Designer-Lauf hatte dabei einen nicht-fatalen
`MSB3026`-Dateisperren-Retry.

## Metaaufgabe: Abschlusskorrekturen

**Umfang:** Die ursprünglichen Akzeptanzkriterien für funktionale
CXAML-Referenzanwendungen, interaktive Designer-Preview und vollständige
Tastatursteuerung erfüllen.

**Status:** Blockiert.

****Details:** CL-16 und CL-17 sind technisch abgeschlossen: vier
Referenzanwendungen bauen einzeln, vier gemeinsame Beispieltests und vier
anwendungsspezifische Integrationstests bestehen. Die gerenderte Avalonia-
Preview, stabile Mappings, Auswahl, Rückschreiben ins CXAML und der
dedizierte Headless-Nachweis sind durch neun Designer-Tests abgedeckt.
CL-18 ist durch 132 aktuelle Core-Tests belegt. CL-19 bleibt wegen der
geschützten Benutzeränderung und der offenen fachlichen Detective-
Suggestion-Auswahl blockiert.

**Abhängigkeiten:** CL-19 hängt von der fachlichen Detective-Suggestion-
Auswahl und einer ausdrücklich freigegebenen, sauberen Arbeitskopie ab. Der
technische Vier-App-, Headless- und SVN-Nachweis liegt in Revision 1773/1774
vor.

## Abnahmekriterien

- Jede Einzelaufgabe besitzt eine eigene Status- und Detaildatei.
- Abhängigkeiten sind in den Task-Dateien nachvollziehbar.
- Test- und Coverage-Ergebnisse sind dokumentiert und als aktuell oder
  historisch gekennzeichnet.
- Die fünf Split-Testprojekte laufen einzeln und sequenziell ohne Fehler und
  ohne Warnungen.
- Vier CXAML-Referenzanwendungen bauen unabhängig.
- Vier referenzierte CXAML-Anwendungen besitzen eigene Integrationstests.
- Gerenderte Designer-Preview-, Mapping- und Headless-Tests bestehen.
- Nutzeränderungen außerhalb dieses Plans bleiben unberührt.
- Kein Release wird als abgeschlossen markiert, solange ein Kriterium oder
  eine Abhängigkeit blockiert ist.