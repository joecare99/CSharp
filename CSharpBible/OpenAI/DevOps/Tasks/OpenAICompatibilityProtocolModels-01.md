# OpenAI-Kompatibilitaet: Protocol-Modelle

Stand: 2026-08-22

## Status
Done

## Ziel
Die von Ollama dokumentierte OpenAI-Kompatibilitaet als serialisierbare DTO-Schicht in `Ollama.Protocol` bereitstellen.

## Umsetzung
- Namespace `Ollama.Protocol.Models.OpenAI` eingefuehrt.
- Chat Completions inklusive Vision-Content, Tools, Reasoning, JSON-Modus und Streaming-Optionen modelliert.
- Legacy Completions inklusive Streaming-kompatibler Choice-Struktur modelliert.
- Models-/Model-List- und Embeddings-Vertraege modelliert.
- Nicht-staatliche Responses API inklusive Function-Tools, Output-Content und Streaming-Events modelliert.
- JSON-Vertraege mit MSTest abgesichert.

## Abgrenzung
Die DTOs bilden den dokumentierten Protocol-Vertrag ab. Ein HTTP-Client fuer `/v1/*` wird in einem separaten Arbeitsschritt auf diesen Modellen aufbauen.

## Verifikation
- Tests: `Ollama.Protocol.Tests` — 87 erfolgreich, 0 fehlgeschlagen, 0 übersprungen.
- Build: `Ollama.Protocol` für net8.0, net9.0 und net10.0 erfolgreich.
