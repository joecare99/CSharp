# CL-04: POSIX-Rendering und Mouse-Eingabe implementieren

**Status:** Abgeschlossen

**Umfang:** ANSI/VT-Ausgabe, Keyboard-/SGR-Mouse-Demultiplexing und
fragmentierte Eingabesequenzen.

**Details:** Gemischte Eingabelesungen verlieren weder Tastatur- noch
Mausereignisse. SGR-Sequenzen dürfen über mehrere Reads verteilt sein;
Rendering und Host-Verarbeitung bleiben getrennt testbar.

**Abhängigkeiten:** CL-03.

**Validierung:** POSIX-Parser-, Host- und Frame-Renderer-Regressionstests.
