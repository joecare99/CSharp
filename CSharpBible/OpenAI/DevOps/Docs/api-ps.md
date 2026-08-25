# List Running Models

The `GET /api/ps` endpoint returns the models that are currently loaded in Ollama memory.

## Endpoint

```http
GET http://localhost:11434/api/ps
```

- **Base URL:** `http://localhost:11434`
- **Authentication:** None
- **Operation ID:** `ps`
- **Response format:** JSON

## Example Request

```bash
curl http://localhost:11434/api/ps
```

## Successful Response

A successful request returns HTTP status `200 OK`.

```json
{
  "models": [
	{
	  "name": "gemma4",
	  "model": "gemma4",
	  "size": 6591830464,
	  "digest": "c6eb396dbd5992bbe3f5cdb947e8bbc0ee413d7c17e2beaae69f5d569cf982eb",
	  "details": {
		"parent_model": "",
		"format": "gguf",
		"family": "gemma4",
		"families": [
		  "gemma4"
		],
		"parameter_size": "8.0B",
		"quantization_level": "Q4_K_M"
	  },
	  "expires_at": "2025-10-17T16:47:07.93355-07:00",
	  "size_vram": 5333539264,
	  "context_length": 4096
	}
  ]
}
```

## Response Structure

The response is an object with a `models` property. The property contains an array of models currently running in Ollama.

### Top-Level Properties

| Property | Type | Description |
|---|---|---|
| `models` | Array | Currently running models. |

### Model Properties

| Property | Type | Description |
|---|---|---|
| `name` | String | Name of the running model. |
| `model` | String | Name of the running model. |
| `size` | Integer | Model size in bytes. |
| `digest` | String | SHA-256 digest of the model. |
| `details` | Object | Additional model metadata, such as its format and family. |
| `expires_at` | String | Timestamp indicating when the model will be unloaded. |
| `size_vram` | Integer | VRAM usage in bytes. |
| `context_length` | Integer | Context length configured for the running model. |

### `details` Properties

The `details` object contains model metadata. The OpenAPI definition describes it as an object and does not define a required property set. The example contains the following fields:

| Property | Type | Description |
|---|---|---|
| `parent_model` | String | Parent model name, if applicable. |
| `format` | String | Model format, for example `gguf`. |
| `family` | String | Primary model family. |
| `families` | Array of strings | Model families associated with the model. |
| `parameter_size` | String | Approximate number of model parameters, for example `8.0B`. |
| `quantization_level` | String | Quantization level, for example `Q4_K_M`. |

## Behavior

- The endpoint reports models that are currently loaded into memory.
- `size` describes the model size in bytes.
- `size_vram` describes the amount of VRAM used by the running model in bytes.
- `expires_at` indicates when Ollama plans to unload the model.
- The API definition does not specify request parameters or authentication requirements.
