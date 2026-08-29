# CL-14: Form-Navigationswidgets implementieren

**Status:** Abgeschlossen

**Umfang:** CheckBox, ComboBox, ProgressBar, StatusBar und TabControl mit
zustandsabhängigem POSIX-Rendering, Padding, Clipping und Navigation.

**Details:** `PosixFormRenderer` bildet Auswahl-, Fortschritts-, Status- und
Tabzustände über den gemeinsamen Renderer-Vertrag ab.

**Abhängigkeiten:** CL-01, CL-10, CL-12.

**Validierung:** Renderer- und Interaktionstests für alle fünf Widgettypen.
