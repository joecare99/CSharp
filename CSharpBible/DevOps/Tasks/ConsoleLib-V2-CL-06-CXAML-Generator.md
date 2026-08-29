# CL-06: CXAML-Codegenerator implementieren

**Status:** Abgeschlossen

**Umfang:** Deterministische C#-Factory-Erzeugung, CXAML-Escaping,
Schema-/Runtime-Diagnostik und ungültige Eingaben.

**Details:** Der Generator validiert vor der Ausgabe und verwendet dieselben
Laufzeitregeln wie der Loader. Eingebettetes Markup wird als kompatibles,
escaped C#-Literal erzeugt.

**Abhängigkeiten:** CL-05.

**Validierung:** Tests für gültige, ungültige und malformed CXAML-Dokumente.
