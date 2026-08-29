# CL-03: POSIX-Terminaltransport implementieren

**Status:** Abgeschlossen

**Umfang:** Stream-basierter POSIX-Transport, Raw-Mode-Lifecycle, UTF-8,
Cancellation, Resize und asynchrones Host-Pumping.

**Details:** Raw mode wird injizierbar betreten und wiederhergestellt;
Terminalausgabe wird ohne BOM geschrieben. `RunAsync` beendet sich
cancellation-sicher und erhält Eingabefehler sichtbar.

**Abhängigkeiten:** CL-02.

**Validierung:** Transport- und Host-Integrationstests einschließlich
Lifecycle und UTF-8-Ausgabe.
