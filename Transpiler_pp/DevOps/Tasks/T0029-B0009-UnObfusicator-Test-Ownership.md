# T0029-B0009 - UnObfusicator test ownership cleanup

## Status

Completed.

## Objective

Keep `VBUnObfusicatorTests` focused on UnObfusicator-specific behavior and avoid repeating tests for shared parser, tokenizer, code-block, and base-library implementations that belong to the `TranspilerLib.*.Tests` projects.

## Removed duplicate tests

- `VBUnObfusicatorTests\Data\TokenDataTests.cs` duplicated `TranspilerLibTests\Data\TokenDataTests.cs`.
- `VBUnObfusicatorTests\Models\ParentedItemsListTests.cs` duplicated `TranspilerLibTests\Models\ParentedItemsListTests.cs`.
- `VBUnObfusicatorTests\Models\Scanner\CSTokenHandlerTests.cs` repeated tokenizer-state and token-emission behavior covered more thoroughly by `TranspilerLib.CSharp.Tests\Models\Scanner\CSTokenHandlerTests.cs`.
- `VBUnObfusicatorTests\Models\Scanner\CCodeBlockTests.cs` repeated block movement, deletion, and reference-index tests from `TranspilerLib.CSharp.Tests\Models\Scanner\CCodeBlockTests.cs`.
- `VBUnObfusicatorTests\Models\Scanner\CSCodeTests.cs` repeated C# tokenization, parsing, code emission, label reordering, and label-removal tests from `TranspilerLib.CSharp.Tests\Models\Scanner\CSCodeTests.cs`. The shared suite has the same categories with expanded fixtures and additional cases.
- `VBUnObfusicatorTests\TestData\TestDataClass.cs` existed only to supply the removed scanner/block tests and is no longer needed.

The cleanup removed 177 test cases from the UnObfusicator test project. The remaining 25 tests exercise WPF startup/views, CLI options, and `CodeUnObFusViewModel` workflows, which are frontend-specific and are not duplicated by the library suites.

## Validation

- `VBUnObfusicatorTests` Release, `net8.0-windows`: **25/25 passed**.
- `TranspilerLibTests` Release: **12/12 passed** on net8.0, net9.0, and net10.0.
- Complete shared C# test suite: **374/374 passed** on net8.0, net9.0, and net10.0.
- `CSCodeTests.TokenizeTest2` now verifies three tokens in source order (`int`, `i`, `;`). Fixing it exposed that the enumerable `CSCode.Tokenize()` implementation ignored the result of `Stack.Reverse()` and yielded emitted tokens backwards; it now yields each handler's tokens in emission order.
