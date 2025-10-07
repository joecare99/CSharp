# CallAllExamples

Launcher console applications (framework & modern variants) that sequentially execute demonstration routines from other sample projects.

## Projects
- CallAllExamples_net.csproj (net6.0; net7.0; net8.0)
- CallAllExamples.csproj (net462; net472; net48; net481)

## Purpose
Provide one-click / one-command aggregation to:
- Exercise reflection examples
- Run collection & LINQ demos
- Invoke async/task samples
- Validate plugin or dynamic behaviors indirectly

## Design
References both DynamicSample and TestStatements (matching appropriate TFMs) to unify sample invocation.

## Build
Modern: `dotnet build CallAllExamples/CallAllExamples_net.csproj`
Legacy: `dotnet build CallAllExamples/CallAllExamples.csproj`

## Run
`dotnet run --project CallAllExamples/CallAllExamples_net.csproj`

## Extension Ideas
- Add CLI arguments to filter which example groups to run
- Emit structured logs (JSON) for automated validation
