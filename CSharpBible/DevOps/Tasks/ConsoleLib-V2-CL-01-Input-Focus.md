# CL-01: Input- und Fokus-Engine definieren

**Status:** Abgeschlossen

**Umfang:** Immutable Input-Verträge, deterministische Fokusverwaltung,
Tab-/Shift-Tab-Navigation und testbare Eingangsgrenzen.

**Details:** `FocusManager` traversiert eligible Controls depth-first und
überspringt nicht sichtbare oder deaktivierte Controls. Die Verträge bleiben
provider-neutral und werden von den Host-Backends verwendet.

**Abhängigkeiten:** CL-12.

**Validierung:** MSTest-Abdeckung im Core-Testprojekt; abgeschlossen als Teil
der vollständigen ConsoleLib-Suite.
