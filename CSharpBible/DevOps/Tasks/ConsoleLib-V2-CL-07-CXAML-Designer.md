# CL-07: CXAML-Designer implementieren

**Status:** Abgeschlossen

**Umfang:** Live-Preview, Inspector, Property-Editing, Diagnostik und
Avalonia-Bindings.

**Details:** Der Designer materialisiert Preview-Controls über den Runtime-
Loader. Width/Height werden als UI-Aliase auf die interne Größenrepräsentation
abgebildet; Änderungen werden auf das Live-Control angewandt.

**Abhängigkeiten:** CL-04, CL-05, CL-10, CL-11, CL-14.

**Validierung:** Eigenes `ConsoleLib.Cxaml.DesignerTests`-Projekt mit 3 Tests.
