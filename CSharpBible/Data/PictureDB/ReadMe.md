# 🖼️ Architekturplan: C#-Programm mit lokalem LLM zur Bildkategorisierung, Bewertung und Sortierung

## 1. Grundlegende Programmlogik (C#)
- **Projektstruktur**: Konsolenanwendung oder WPF/WinForms für GUI  
- **Module**:
  - `ImageLoader`: lädt Bilder aus lokalen Ordnern
  - `ImageProcessor`: bereitet Bilder für das LLM vor (z. B. Konvertierung in Base64 oder Feature-Extraktion)
  - `LLMInterface`: Schnittstelle zum lokalen LLM
  - `Categorizer`: ruft das LLM auf, um Kategorien zu bestimmen
  - `Evaluator`: ruft das LLM auf, um Bewertungen (Scores) zu erzeugen
  - `Sorter`: sortiert Bilder nach Bewertung oder Kategorie
  - `UI/Output`: zeigt Ergebnisse an oder speichert sie in einer Datenbank/Datei

---

## 2. Lokales LLM
- **Optionen**:
  - Ollama (lokale LLMs wie LLaVA, Mistral, etc.)
  - GPT4All oder LM Studio
  - Hugging Face Modelle (z. B. BLIP, CLIP für Bildbeschreibung/Kategorisierung)
- **Anforderungen**:
  - Modell mit Multimodalität (Text + Bild), z. B. CLIP oder LLaVA
  - Lokale API oder Bibliothek, die von C# aus angesprochen werden kann (REST, gRPC oder Prozessaufruf)

---

## 3. Bildverarbeitung
- **Bibliotheken**:
  - `System.Drawing` oder `ImageSharp` für Bildmanipulation
  - Konvertierung in ein Format, das das LLM versteht (z. B. Base64‑String oder Pfadübergabe)
- **Vorverarbeitung**:
  - Skalierung, Normalisierung
  - Metadaten extrahieren (Dateiname, Größe, Format)

---

## 4. Kommunikation mit dem LLM
- **API Layer**:
  - REST‑Client (`HttpClient`) oder gRPC
  - Eingabe: Bilddaten + Prompt (z. B. „Kategorisiere dieses Bild nach Thema“)
  - Ausgabe: Textantwort (Kategorie, Score, Ranking)
- **Prompt Engineering**:
  - Beispiel:
    ```text
    Analysiere das Bild und gib eine Kategorie (z. B. Natur, Architektur, Person) 
    sowie eine Bewertung von 1-10 für Qualität.
    ```

---

## 5. Bewertung & Sortierung
- **Evaluator**:
  - Nimmt Score vom LLM entgegen
  - Speichert Ergebnisse in einer Datenstruktur (z. B. `List<ImageResult>`)
- **Sorter**:
  - Sortiert nach Kategorie oder Score
  - Optional: Filterfunktionen (z. B. nur Bilder > Score 7)

---

## 6. Persistenz & Ausgabe
- **Optionen**:
  - Speicherung in JSON/CSV
  - Datenbank (SQLite, LiteDB)
  - GUI‑Darstellung (WPF/WinForms)
- **Funktionen**:
  - Export der Ergebnisse
  - Anzeige sortierter Bilder

---

## 7. Erweiterungen
- Batch‑Verarbeitung ganzer Ordner
- Training eigener Kategorien
- Kombination mit klassischen ML‑Bibliotheken (z. B. TensorFlow.NET, ML.NET)
- Parallelisierung für Performance

---

## 📋 Zusammenfassung der benötigten Komponenten
- **C# Projektstruktur** (Loader, Processor, Categorizer, Evaluator, Sorter, UI)
- **Lokales LLM** (z. B. Ollama, GPT4All, Hugging Face Modell)
- **Bildverarbeitungsbibliothek** (ImageSharp, System.Drawing)
- **Kommunikationslayer** (REST/gRPC Client)
- **Persistenz** (JSON/CSV oder Datenbank)
- **UI/Output** (Konsole, GUI oder Weboberfläche)
