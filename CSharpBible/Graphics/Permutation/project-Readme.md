# Permutation

`Permutation` contains the `PermutatedRange` learning implementation. It
starts with an identity range and applies ring-based swaps controlled by the
`pp` parameter. The indexer exposes the generated value and returns `-1` for
the first index outside the range.

## Tests

`PermutationTests` covers the published small-range examples, the identity
case (`pp = 0`), and the out-of-range sentinel.

```powershell
dotnet test C:\Projekte\CSharp\CSharpBible\Graphics\PermutationTests\PermutationTests.csproj --configuration Debug
```

The focused `net8.0-windows` run passes all 6 tests. The `net6.0-windows` and
`net7.0-windows` hosts currently stop before test execution because their
dependency manifests cannot resolve `Microsoft.ApplicationInsights`; use the
modern target for the deterministic validation slice until that environment
issue is resolved.

The large diagnostic test remains intentionally separate from the deterministic
contract tests because it prints distribution diagnostics across many
parameter values.
