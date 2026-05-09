# OpenAI Playground

This project is a minimal starting point for learning the basic OpenAI .NET SDK workflow.

## What it shows
- read the API key from an environment variable
- create a `ChatClient`
- send a prompt to a model
- print the response to the console

## Prerequisites
- .NET 8 SDK
- an OpenAI API key

## Configuration
Set the environment variable before running the project:

### PowerShell
`$env:OPENAI_API_KEY = "your-api-key"`

The sample uses the default model `gpt-4o-mini` so that the first experiment stays simple and cost-aware.

## Run
`dotnet run --project .\OpenAIPlayground\OpenAIPlayground.csproj`

You can also pass your own prompt:

`dotnet run --project .\OpenAIPlayground\OpenAIPlayground.csproj -- "Write a short hello world explanation."`

## Learning focus
Use this sample to understand the basic request flow first:
- configuration
- client creation
- prompt input
- response output

## Next learning steps
- switch to another model
- try streaming responses
- add system prompts
- compare OpenAI and Azure OpenAI endpoints
