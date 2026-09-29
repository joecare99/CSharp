# BaseLib ObjectComparison API Contract

## Scope

This document specifies the external BaseLib prerequisite required by the optional VB legacy compatibility rules. The implementation belongs to the BaseLib workspace and is intentionally not modified from this workspace.

## Public API

```csharp
namespace BaseLib.Helper;

public static class ObjectComparison
{
	public static bool AreEqual(
		object? left,
		object? right,
		StringComparison stringComparison = StringComparison.Ordinal);

	public static int Compare(
		object? left,
		object? right,
		StringComparison stringComparison = StringComparison.Ordinal);
}
```

## Required Behavior

- `AreEqual` returns `true` for two `null` or `DBNull` values, and `false` when only one operand is null-like.
- `Compare` orders null-like values before non-null-like values and returns zero for two null-like values.
- When both operands are strings, both methods use the supplied `StringComparison` value.
- When both operands are numeric primitive values, both methods compare their numeric values without lossy string conversion.
- When operands share a compatible `IComparable` implementation, `Compare` delegates to it and normalizes the result to negative, zero, or positive.
- For values without a compatible comparison contract, `Compare` throws `ArgumentException` and `AreEqual` returns `object.Equals(left, right)`.
- `DBNull.Value` is treated consistently with null-like values; it is not converted to the text "DBNull".

## Documentation and Tests

The BaseLib implementation must include English XML documentation and MSTest coverage for null, `DBNull`, ordinal and case-insensitive string comparisons, mixed numeric primitives, compatible `IComparable` values, equality fallback, and unsupported comparisons.

## Transpiler Dependency

`baselib-compatibility-rules.json` emits calls to this API and adds `BaseLib.Helper` as a required using. Consumers must reference a BaseLib version that provides this API before enabling the BaseLib compatibility profile.
