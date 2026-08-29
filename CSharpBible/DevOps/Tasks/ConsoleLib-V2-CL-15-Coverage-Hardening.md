# CL-15: Backend-Coverage härten

**Status:** Abgeschlossen

**Umfang:** Gezielte POSIX-Vertragstests für ANSI-Ausgabe, Farbcodes,
Mouse-Encoding, Transport-Lifecycle, Resize-Clamping und Cancellation.

**Details:** Zusätzlich wurden veraltete Cross-Project-Includes aus dem
aufgeteilten POSIX-Testprojekt entfernt. Die neuen Tests prüfen direkt
produktive Backendpfade statt nur Testanzahl zu erhöhen.

**Abhängigkeiten:** CL-09.

**Validierung:** Vollständige Split-Suite mit 164 Tests ohne Fehler; POSIX
Coverage 17,2 % (780/4533 Zeilen); SVN-Revision 1770.
