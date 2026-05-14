# Ollama DI Usage

## Minimal Example
```csharp
using Microsoft.Extensions.DependencyInjection;
using Ollama.Client;
using Ollama.Extensions.DependencyInjection;

ServiceCollection services = new();
services.AddOllamaClient(new Uri("http://localhost:11434/"));
services.AddOllamaChatClient("qwen3.5:4b");

using ServiceProvider serviceProvider = services.BuildServiceProvider();
OllamaChatClient chatClient = serviceProvider.GetRequiredService<OllamaChatClient>();
```

## Notes
- Register the root client first.
- Register feature clients afterwards for the model you want to bind.
- The current milestone provides a simple default registration path.
