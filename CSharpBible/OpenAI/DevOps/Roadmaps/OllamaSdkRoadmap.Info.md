# Ollama SDK Roadmap

## Vision
Build a reusable Ollama-oriented SDK with an API shape inspired by Azure.AI libraries, including streaming, tool use, skills, dependency injection, and sample applications.

## Product Goal
Provide a layered implementation so low-level Ollama protocol work stays isolated from higher-level client APIs and orchestration features.

## Guiding Principles
- Keep one clear responsibility per project.
- Stabilize protocol and models before adding orchestration.
- Add tests at every layer before building the next layer on top.
- Prefer Azure.AI-like naming and usage patterns only after the Ollama behavior is understood well.
- Keep transport, contracts, tools, and skill orchestration separate.

## Suggested Project Backlog

### 1. Ollama.Protocol
**Purpose**
- Raw HTTP access to Ollama endpoints.
- Request and response DTOs for `/api/tags`, `/api/generate`, `/api/chat`, `/api/embed`, and model management endpoints if needed.

**Key scope**
- Streaming line reader for NDJSON responses.
- Error mapping for HTTP and Ollama payload errors.
- Serialization helpers.

**Done when**
- A console sample can list models and stream a basic generate/chat response.

### 2. Ollama.Protocol.Tests
**Purpose**
- Protect protocol parsing and streaming behavior.

**Key scope**
- JSON serialization tests.
- NDJSON chunk parsing tests.
- Error payload parsing tests.
- Cancellation and timeout behavior tests.

**Done when**
- Protocol contracts are regression-safe.

### 3. Ollama.Client
**Purpose**
- First public client API, analogous to an Azure.AI-style service client.

**Key scope**
- `OllamaClient`
- `GetChatClient(model)`
- `GetGenerateClient(model)` or one unified text-generation client
- `CompleteChatAsync`, `CompleteChatStreamingAsync`
- Shared options and pipeline configuration

**Done when**
- Calling code no longer knows Ollama endpoint details.

### 4. Ollama.Client.Tests
**Purpose**
- Verify public client behavior without depending on a live Ollama instance.

**Key scope**
- Mocked transport tests.
- Model selection tests.
- Streaming aggregation tests.
- Public exception behavior tests.

**Done when**
- The client layer is refactor-safe.

### 5. Ollama.Tools
**Purpose**
- Tool schema and tool-call execution model.

**Key scope**
- Tool definitions.
- Tool result envelope.
- Function argument parsing.
- Tool registry abstraction.
- Model-to-tool-call loop orchestration.

**Done when**
- A model can request a tool, the host resolves it, and the conversation continues automatically.

### 6. Ollama.Tools.Tests
**Purpose**
- Validate tool routing and tool-call safety.

**Key scope**
- Argument validation tests.
- Unknown tool tests.
- Multi-step tool-call loop tests.
- Tool result reinjection tests.

**Done when**
- Tool execution is predictable and test-covered.

### 7. Ollama.Skills
**Purpose**
- Higher-level reusable capabilities composed from prompts, tools, and policies.

**Key scope**
- Skill abstraction.
- Skill context.
- Prompt template support.
- Skill registry.
- Optional memory/context injection.

**Done when**
- A skill can be invoked as a reusable unit rather than writing raw prompts every time.

### 8. Ollama.Skills.Tests
**Purpose**
- Protect orchestration behavior for reusable skills.

**Key scope**
- Skill resolution tests.
- Prompt composition tests.
- Tool-enabled skill tests.
- Failure-path tests.

**Done when**
- Skills behave consistently across different models and prompts.

### 9. Ollama.Extensions.DependencyInjection
**Purpose**
- Make the SDK easy to consume in applications.

**Key scope**
- `AddOllamaClient(...)`
- Named model registrations.
- Options binding.
- Service lifetime defaults.

**Done when**
- Console, web, and desktop applications can register the SDK in one line.

### 10. Ollama.Diagnostics
**Purpose**
- Observability and operational support.

**Key scope**
- Logging hooks.
- Request/response tracing.
- Streaming event diagnostics.
- Token and duration summaries where available.

**Done when**
- Failures and latency can be understood without manual packet inspection.

### 11. Ollama.Samples.BasicChat
**Purpose**
- Small reference app for the stable public client.

**Key scope**
- Basic prompt.
- Streaming output.
- Model selection.
- Error display.

**Done when**
- New users can learn the API in minutes.

### 12. Ollama.Samples.ToolUse
**Purpose**
- Demonstrate the tool loop end to end.

**Key scope**
- Sample tools such as time, weather stub, local file lookup, or calculator.
- Console visualization for thinking, tool call, tool result, and final answer.

**Done when**
- Tool use is understandable and reproducible.

### 13. Ollama.Samples.Skills
**Purpose**
- Demonstrate reusable skills on top of the SDK.

**Key scope**
- Summarize text skill.
- Extract structured data skill.
- Code helper skill.

**Done when**
- Skills are visible as a distinct layer above the raw client and tool system.

## Recommended Delivery Order
1. Ollama.Protocol
2. Ollama.Protocol.Tests
3. Ollama.Client
4. Ollama.Client.Tests
5. Ollama.Tools
6. Ollama.Tools.Tests
7. Ollama.Skills
8. Ollama.Skills.Tests
9. Ollama.Extensions.DependencyInjection
10. Ollama.Diagnostics
11. Samples

## API Shape Recommendation
Adopt Azure.AI-inspired shapes carefully:
- one root client for server configuration
- feature clients for chat, generation, and embeddings
- async-first methods
- streaming methods separated from buffered methods
- explicit options objects
- transport abstractions for testability

Avoid copying Azure.AI surface area too early. First confirm which Ollama concepts map cleanly and which need Ollama-specific design.

## First Milestone Recommendation
The first milestone should stop after these projects:
- Ollama.Protocol
- Ollama.Protocol.Tests
- Ollama.Client
- Ollama.Client.Tests
- Ollama.Samples.BasicChat

This gives a stable base before tool use and skills are introduced.

## Second Milestone Recommendation
Add:
- Ollama.Tools
- Ollama.Tools.Tests
- Ollama.Samples.ToolUse

## Third Milestone Recommendation
Add:
- Ollama.Skills
- Ollama.Skills.Tests
- Ollama.Extensions.DependencyInjection
- Ollama.Diagnostics
- Ollama.Samples.Skills

## Risks
- Ollama model behavior differs per model, especially for thinking and tool formats.
- Some Azure.AI concepts may not map 1:1 to Ollama.
- Tool use may require model-specific prompt conventions before a generic abstraction is possible.
- Streaming formats and metadata may evolve.

## Immediate Next Step
Create `Ollama.Protocol` and `Ollama.Protocol.Tests` first and move the current experimental HTTP code there as the initial seed implementation.
