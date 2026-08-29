# CL-05: CXAML-Loader implementieren

**Status:** Abgeschlossen

**Umfang:** Strukturvalidierung, Control-Erzeugung, Positionierung,
skalare Attribute und explizite Parsefehler.

**Details:** Unterstützt werden unter anderem Text, Größe, X/Y, Sichtbarkeit,
Aktivierung und Farben. Leere Dokumente, mehrere Wurzeln, unbekannte Elemente
und fehlerhaftes XML werden mit diagnostischen Details abgewiesen.

**Abhängigkeiten:** CL-01, CL-10, CL-11, CL-13, CL-14.

**Validierung:** Loader- und Fehlerszenario-Tests mit erhaltener InnerException.
