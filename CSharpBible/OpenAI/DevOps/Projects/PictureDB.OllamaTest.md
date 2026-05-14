# Project: PictureDB.OllamaTest

## Status
In Progress

## Purpose
`PictureDB.OllamaTest` is a small console sandbox for testing image-based Ollama vision requests.

## Current Responsibilities
- Read a single image from disk.
- Normalize the image before sending it to Ollama.
- Convert the image to base64.
- Send the image and prompt through `Ollama.Client`.
- Stream the response to the console.

## Image Normalization Notes
- Uses `System.Drawing.Common` to avoid external image package dependency.
- Resizes very small images to reduce hallucination caused by tiny source images.
- Flattens transparency onto a white background before encoding.

## Current CLI Behavior
- Accepts `ImagePath`, optional `ModelName`, and optional `Prompt`.
- Sends the encoded image through `Ollama.Client` as a vision-enabled chat message.
- Streams the model response directly to the console.

## Notable Types
- `Program`

## Related Planning Items
- [Epic: AI-Driven Picture Database (PictureDB)](../Epics/Epic-PictureDatabase.md)
- [Feature: Ollama Vision Prototyping and Sandbox](../Features/Feat-03-VisionPrototyping.md)
- [Backlog Item: Integrate Ollama.Client into PictureDB.OllamaTest](../BacklogItems/PBI-03-OllamaTestIntegrateClient.md)