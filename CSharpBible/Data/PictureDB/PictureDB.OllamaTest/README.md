# PictureDB Ollama Test

Kleines Testprojekt, das die `ollama`-CLI aufruft, um das lokale Modell mit einem Prompt zu befragen.

Verwendung:

```
dotnet run --project PictureDB.OllamaTest -- <model> <prompt>

Beispiel:

dotnet run --project PictureDB.OllamaTest -- llava-alpha "Beschreibe dieses Bild kurz"
```

Hinweis: `ollama` muss installiert und in PATH verfügbar sein.