# CL-02: Textbearbeitung und Clipboard erweitern

**Status:** Abgeschlossen

**Umfang:** Wortnavigation, Wortselektion, Ctrl-Backspace/Ctrl-Delete,
Multiline-Grenzen und Clipboard-Verträge.

**Details:** TextBox unterstützt Ctrl-Left/Ctrl-Right und Shift-basierte
Wortbereiche, ohne Cursor- oder Auswahlgrenzen zu verletzen. Clipboard bleibt
über einen abstrakten Dienst austauschbar.

**Abhängigkeiten:** CL-01.

**Validierung:** Core-Regressionstests für ein- und mehrzeilige Eingaben.
