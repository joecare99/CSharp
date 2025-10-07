# TestNamespaces

Compact console project exploring namespace organization, resolution, and possibly aliasing/conflict scenarios.

## Target Framework
net9.0 (modern baseline showcasing latest compiler behaviors around namespaces).

## Possible Topics
- File-scoped vs block-scoped namespaces
- Nested namespace design
- Using directives (global vs local)
- Name collision resolution patterns

## Build
`dotnet build TestNamespaces/TestNamespaces.csproj`

## Rationale
Acts as an isolated sandbox free from unrelated dependencies, enabling focused namespace experimentation.
