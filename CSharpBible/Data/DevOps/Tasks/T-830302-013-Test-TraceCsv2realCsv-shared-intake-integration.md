# Task T-830302-013 - Test TraceCsv2realCsv shared intake integration

## Status

Draft

## Parent

- Backlog Item `BI-830332` - `Create pluggable input filters for initial source formats`

## Goal

Verify that `TraceCsv2realCsv` can convert supported source formats through the shared TraceAnalysis intake path.

## Test Scope

- Select the expected CSV-oriented filter for `.trace.csv` input
- Convert MOVIRUN text trace input through the shared intake path
- Convert MOVIRUN XML `.trace` input through the shared intake path
- Verify deterministic output structure from the converter service

## Test Notes

- Use `MSTest`
- Prefer compact representative inputs for converter tests
- Keep assertions focused on canonical output structure and key values
- Cover selection and conversion behavior together where practical

## Done Criteria

- Converter tests cover `.csv`, `.txt`, and `.trace` input paths
- Shared selector behavior is exercised by the converter tests
- Tests pass in the TraceCsv2realCsv test project
