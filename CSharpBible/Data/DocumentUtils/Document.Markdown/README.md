# Document.Markdown

`Document.Markdown` ist ein Markdown-Implementierungsprojekt auf Basis von `Document.Base`.
Es stellt die Markdown-spezifischen Dokumenttypen, den Renderer sowie Lade-/Speicherhilfen bereit.

## Zweck

- Markdown-Dokumente erzeugen
- Markdown-Inhalt rendern
- Markdown-Dokumente laden und speichern
- die gemeinsame Dokumentabstraktion aus `Document.Base` nutzen

## Bestandteile

- `MarkdownDocument` als Einstiegspunkt für Markdown-Dokumente
- `MarkdownSection`, `MarkdownParagraph`, `MarkdownHeadline`, `MarkdownTOC`, `MarkdownSpan` und weitere Modelltypen
- `MarkdownRenderer` für die Ausgabe als Markdown-Text
- `MarkdownDocumentSerializer` als Umwandlungsschicht
- `MarkdownDocumentIO` für Datei-/Stream-Zugriff

## Teststand

Das zugehörige Testprojekt ist:

- `Document.MarkdownTests`

Aktueller Stand:

- Build erfolgreich
- Coverage für `Document.Markdown` vollständig
- Tests liegen namespace-basiert in Unterordnern

## Hinweise

- Die öffentliche Contract-Schicht bleibt in `Document.Base` stabil.
- Parent-Tracking wird intern in den konkreten Markdown-Knoten geführt.
- Die Markdown-Logik ist bewusst in Renderer, Serializer und IO getrennt.

## Relevante Projekte

- `Data\DocumentUtils\Document.Base`
- `Data\DocumentUtils\Document.Markdown`
- `Data\DocumentUtils\Document.MarkdownTests`
