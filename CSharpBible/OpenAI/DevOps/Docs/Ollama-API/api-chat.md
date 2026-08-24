# Chat API

Generate the next assistant message in a conversation using Ollama's native chat API.

- **Endpoint:** `POST /api/chat`
- **Default base URL:** `http://localhost:11434`
- **Content type:** `application/json`
- **Response formats:** JSON for non-streaming requests, newline-delimited JSON (NDJSON) for streaming requests

## Basic request

The `model` and `messages` properties are required. Messages are sent in conversation order.

```bash
curl http://localhost:11434/api/chat \
  -H "Content-Type: application/json" \
  -d '{
	"model": "gemma4",
	"messages": [
	  {
		"role": "user",
		"content": "Why is the sky blue?"
	  }
	],
	"stream": false
  }'
```

A response contains the generated message and completion metadata:

```json
{
  "model": "gemma4",
  "created_at": "2025-10-17T23:14:07.414671Z",
  "message": {
	"role": "assistant",
	"content": "The sky appears blue because air molecules scatter blue light more strongly than other visible wavelengths."
  },
  "done": true,
  "done_reason": "stop",
  "total_duration": 174560334,
  "load_duration": 101397084,
  "prompt_eval_count": 11,
  "prompt_eval_duration": 13074791,
  "eval_count": 18,
  "eval_duration": 52479709
}
```

## Conversation history

Provide previous messages to maintain context. The supported roles are `system`, `user`, `assistant`, and `tool`.

```json
{
  "model": "gemma4",
  "messages": [
	{
	  "role": "system",
	  "content": "Answer briefly and clearly."
	},
	{
	  "role": "user",
	  "content": "What is a black hole?"
	},
	{
	  "role": "assistant",
	  "content": "A black hole is a region of space with gravity so strong that nothing, not even light, can escape."
	},
	{
	  "role": "user",
	  "content": "How is one formed?"
	}
  ],
  "stream": false
}
```

## Streaming

Streaming is enabled by default. Set `stream` to `false` when a single complete response is preferred. With streaming enabled, Ollama returns one JSON object per line until the final object has `done: true`.

```bash
curl http://localhost:11434/api/chat -d '{
  "model": "gemma4",
  "messages": [
	{ "role": "user", "content": "Write three short facts about Mars." }
  ],
  "stream": true
}'
```

Typical stream events look like this:

```json
{"model":"gemma4","message":{"role":"assistant","content":"Mars"},"done":false}
{"model":"gemma4","message":{"role":"assistant","content":" is a planet."},"done":false}
{"model":"gemma4","message":{"role":"assistant","content":""},"done":true,"done_reason":"stop"}
```

Applications should process each line independently and stop after the event with `done: true`. A stream event can contain an empty `content` while still carrying metadata or thinking output.

## Thinking output

Thinking-capable models can return a separate reasoning trace in `message.thinking`. Enable it with `think`:

```bash
curl http://localhost:11434/api/chat -d '{
  "model": "gpt-oss",
  "messages": [
	{ "role": "user", "content": "What is 1+1?" }
  ],
  "think": "low"
}'
```

`think` accepts:

- `true` or `false`
- `"low"`
- `"medium"`
- `"high"`
- `"max"`

The response keeps thinking and answer text separate:

```json
{
  "model": "gpt-oss",
  "message": {
	"role": "assistant",
	"thinking": "The question asks for a basic addition.",
	"content": "2"
  },
  "done": true
}
```

The selected model must support thinking. If the model does not support it, no `thinking` property is returned.

## Structured outputs

Use `format: "json"` for valid JSON output. A JSON Schema object can be supplied when the response must follow a specific structure.

```bash
curl http://localhost:11434/api/chat \
  -H "Content-Type: application/json" \
  -d '{
	"model": "gemma4",
	"messages": [
	  {
		"role": "user",
		"content": "Give the populations of the United States and Canada."
	  }
	],
	"stream": false,
	"format": {
	  "type": "object",
	  "properties": {
		"countries": {
		  "type": "array",
		  "items": {
			"type": "object",
			"properties": {
			  "country": { "type": "string" },
			  "population": { "type": "integer" }
			},
			"required": ["country", "population"]
		  }
		}
	  },
	  "required": ["countries"]
	}
  }'
```

## Tool calling

Tools describe functions that the model may request. Ollama returns requested calls in `message.tool_calls`. The application executes the function and sends the result back as a message with role `tool`.

```bash
curl http://localhost:11434/api/chat \
  -H "Content-Type: application/json" \
  -d '{
	"model": "qwen3",
	"messages": [
	  { "role": "user", "content": "What is the weather in Paris?" }
	],
	"stream": false,
	"tools": [
	  {
		"type": "function",
		"function": {
		  "name": "get_current_weather",
		  "description": "Get the current weather for a location",
		  "parameters": {
			"type": "object",
			"properties": {
			  "location": {
				"type": "string",
				"description": "The city and country"
			  },
			  "unit": {
				"type": "string",
				"enum": ["celsius", "fahrenheit"]
			  }
			},
			"required": ["location", "unit"]
		  }
		}
	  }
	]
  }'
```

A tool call has this general shape:

```json
{
  "message": {
	"role": "assistant",
	"content": "",
	"tool_calls": [
	  {
		"function": {
		  "name": "get_current_weather",
		  "arguments": {
			"location": "Paris",
			"unit": "celsius"
		  }
		}
	  }
	]
  },
  "done": true
}
```

After executing the function, send its result in a follow-up request:

```json
{
  "model": "qwen3",
  "messages": [
	{ "role": "user", "content": "What is the weather in Paris?" },
	{
	  "role": "assistant",
	  "content": "",
	  "tool_calls": [
		{
		  "function": {
			"name": "get_current_weather",
			"arguments": { "location": "Paris", "unit": "celsius" }
		  }
		}
	  ]
	},
	{
	  "role": "tool",
	  "content": "18°C and cloudy"
	}
  ]
}
```

## Images

Vision-capable models accept base64-encoded images in the message's `images` array.

```bash
curl http://localhost:11434/api/chat \
  -H "Content-Type: application/json" \
  -d '{
	"model": "gemma4",
	"messages": [
	  {
		"role": "user",
		"content": "What is in this picture?",
		"images": ["<base64-encoded-image>"]
	  }
	],
	"stream": false
  }'
```

Use a model with vision support. Ollama's native chat API expects inline base64 image data; image URLs are not accepted by this endpoint.

## Request properties

| Property | Type | Description |
|---|---|---|
| `model` | string | Model name, for example `gemma4` or `qwen3`. Required. |
| `messages` | array | Conversation messages. Required. |
| `tools` | array | Optional function definitions. |
| `format` | string or object | `json` or a JSON Schema. |
| `options` | object | Runtime generation options such as `temperature`, `top_p`, `seed`, and `num_predict`. |
| `stream` | boolean | Stream NDJSON events. Defaults to `true`. |
| `think` | boolean or string | Enable thinking or select `low`, `medium`, `high`, or `max`. |
| `keep_alive` | string or number | How long the model remains loaded, for example `5m` or `0`. |
| `logprobs` | boolean | Return token log probabilities when supported. |
| `top_logprobs` | integer | Number of alternatives returned for each token when log probabilities are enabled. |

## Response properties

| Property | Type | Description |
|---|---|---|
| `model` | string | Model used for generation. |
| `created_at` | string | ISO 8601 creation timestamp. |
| `message.role` | string | Role of the generated message, normally `assistant`. |
| `message.content` | string | Generated answer text. |
| `message.thinking` | string | Separate thinking text when enabled and supported. |
| `message.tool_calls` | array | Function calls requested by the model. |
| `message.images` | array | Optional base64-encoded images. |
| `done` | boolean | Indicates the final response event. |
| `done_reason` | string | Why generation ended, for example `stop`. |
| `total_duration` | integer | Total generation time in nanoseconds. |
| `load_duration` | integer | Model load time in nanoseconds. |
| `prompt_eval_count` | integer | Number of prompt tokens. |
| `prompt_eval_duration` | integer | Prompt evaluation time in nanoseconds. |
| `eval_count` | integer | Number of generated tokens. |
| `eval_duration` | integer | Generation time in nanoseconds. |

## Related documentation

- [Generate API](api--generate.md)
- [Streaming](api--streaming.md)
- [Thinking capability](../capabilities/thinking.md)
- [Tool calling capability](../capabilities/tool-calling.md)
- [Official Ollama API documentation](https://docs.ollama.com/api/chat)
