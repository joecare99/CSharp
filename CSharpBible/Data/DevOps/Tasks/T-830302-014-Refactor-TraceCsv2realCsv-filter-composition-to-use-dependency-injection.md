# Task T-830302-014 - Refactor TraceCsv2realCsv filter composition to use dependency injection

## Status

Done

## Parent

- Backlog Item `BI-830332` - `Create pluggable input filters for initial source formats`

## Goal

Move `TraceCsv2realCsv` filter and selector creation into a Dependency Injection composition root so the converter no longer hard-codes filter construction inside the conversion service.

## Scope

- Register the shared input filters through `Microsoft.Extensions.DependencyInjection`
- Register the input-filter selector and CSV output filter through the same container
- Resolve `TraceCsvConversionService` and `TraceCsv2realCsvHelpTextProvider` from the container
- Remove direct `new ...Filter()` construction from `TraceCsvConversionService`

## Implementation Notes

- Keep `Program` as the console composition root
- Register each `IAnalyzableInputFilter` individually so the filter list remains explicit and deterministic
- Preserve the existing default `CsvOutputFilter` behavior while resolving it through `IOutputFilter`
- Keep conversion logic constructor-injected for testability and future extension

## Validation

- `TraceCsv2realCsv\TraceCsv2realCsv.csproj` builds successfully after the DI refactor
- File-level diagnostics are clean for `Program.cs` and `TraceCsvConversionService.cs`
- Older target framework support warnings can be ignored when newer supported targets also build successfully

## Dependencies

- `T-830353` - `Specify input filter interface and selection strategy`
- `T-830302-012` - `Integrate TraceCsv2realCsv with shared TraceAnalysis intake filters`

## Done Criteria

- `TraceCsv2realCsv` uses a DI container to construct filter-related services
- `TraceCsvConversionService` no longer creates filters directly
- Registered filters remain explicit and deterministic
- Converter behavior remains unchanged apart from the internal composition model
