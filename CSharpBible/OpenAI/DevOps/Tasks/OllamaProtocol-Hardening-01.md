# Ollama Protocol Hardening - Review, Verification & Fixes

Stand: 2026-08-22 (laufend aktualisiert - Notizen zum Wiederaufsetzen)

## Kontext

Review von `Ollama.Protocol` + `Ollama.Client` (siehe Zusammenfassung der
Review-Findings unten). Aufgabe:
1. Protokollverhalten mit curl gegen lokalen Ollama-Server verifizieren (localhost:11434).
2. Findings belegen/entkräften.
3. P1/P2 (und wo billig P3) umsetzen.
4. Diese Notiz fortlaufend aktualisieren.

## Review-Findings (Verifikationsergebnis 2026-08-22)

| # | Finding | Verifikation |
|---|---------|--------------|
| 1 | `OllamaChatMessage.Content` ist `required string` -> STJ wirft `JsonException`, wenn `content` in Chunk fehlt (Tool-Call-Szenario) | ZU PRUEFEN (curl: Tool-Call-Stream) |
| 2 | Kein `error`-Feld auf `OllamaChatResponseChunk`/`OllamaGenerateResponseChunk` -> Mid-Stream-`{"error":...}` wird verschluckt (Message=null, Done=false) | ZU PRUEFEN (curl: Error-Envelope) |
| 3 | `EnsureSuccessStatusCode()` wirft Ollama-Error-Body weg | ZU PRUEFEN (curl: 4xx mit Body) |
| 4 | Fehlende Request-Parameter: `options`, `keep_alive`, `think`, `system`, `format`, `images` (generate) | ZU PRUEFEN (curl: Parameter akzeptiert?) |
| 5 | `think` nur in Response, nicht als Request-Schalter | wie #4 |
| 6 | Chat/Generate StreamReader = Kopien | Code-Fakt, P3 |
| 7 | `OllamaProtocolAdapter` reine Pass-through | Code-Fakt, P3 (diesmal NICHT angetastet) |
| 8 | `OllamaChatClient` setzt immer `Stream = true` | Code-Fakt, Note |

| URI-Bug (vom Review uebersehen) | `new Uri(base, "/api/tags")` ersetzt den Basispfad -> `http://host/ollama` verliert `/ollama`. Nur mit Slash am Ende korrekt. | ZU PRUEFEN (curl mit Subpath ist gegen localhost nicht moeglich; .NET-Semantik dokumentiert) |
| "Layering violation" | UNBEGRUENDET - `OpenAI\DevOps\Projects\Ollama.Protocol.md` dokumentiert exakt diese Aufgabe (HTTP + NDJSON im Protocol-Projekt) | entkraeftet |

## Umsetzungsplan

- P1a: `error`-Zeile im NDJSON-Stream erfassen (Reader wirft `OllamaProtocolException` mit Fehlermeldung).
- P1b: Endpunkt-URI normalisieren (trailing slash / korrekter Pfadbuild) in `OllamaProtocolClient`.
- P2a: `OllamaChatMessage.Content`: `required string` -> `string` (Default `""`), `required` entfernen.
- P2b: Request-Parameter ergaenzen (Chat + Generate): `Options` (neues `OllamaOptions`-Modell), `KeepAlive`, `Think`, `System`, `Format`, `Images` (generate), `Raw` (generate).
- P3a: Fehler-Body bei Nicht-2xx auslesen und in Exception-Message mitschleifen.
- P3b: StreamReader-Deduplizierung (gemeinsamer internen NDJSON-Helfer).
- OFFEN (diesmal nicht): Adapter-Redundanz, Client-Layer-Optionen-Mapping, `OllamaChatClient` Always-Stream.

## Umgebung

- SVN-Working-Copy unter `C:\Projekte\CSharp` (`.svn` vorhanden) -> neue Dateien mit `svn add`.
- Ollama 0.32.15 auf `http://localhost:11434`.
- Modelle: qwen3.5:9b (thinking,tools), qwen2.5-coder:7b (tools), gemma4:e2b (klein), u.a.
- Test-Modell (schnell): `gemma4:e2b` / Fuer Tool-Calls: `qwen2.5-coder:7b` oder `qwen3.5:9b`.

## curl-Tests (Ergebnisse)

(ausfuehren und Roh-Output hier ablegen)
