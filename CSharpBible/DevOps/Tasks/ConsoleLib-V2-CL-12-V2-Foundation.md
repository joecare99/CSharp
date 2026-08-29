# CL-12: V2-Verträge und Testharness etablieren

**Status:** Abgeschlossen

**Umfang:** Input-/Capability-/Clipboard-Verträge, Layoutzustand,
Application-Queue, Dispatcher, Scheduler, Clock, RenderContext und
In-Memory-Host.

**Details:** Die Dienste sind instanzbezogen und testbar; Application-Lifecycle
und Disposal werden explizit behandelt. Provider-spezifische Implementierungen
bleiben außerhalb des Core-Projekts.

**Abhängigkeiten:** Keine.

**Validierung:** Core-Vertrags- und Lifecycle-Tests.
