# AA98-Bl001 - CodeQL Top Errors Review

## Goal
Review and reduce the findings from the latest CodeQL run without broadening the scope beyond the highest-confidence issues.

## Scope
The analysis is limited to the concrete top errors from `DevOps/CodeQL/Results/run-20260816-210337/top-errors-20.json` and their code paths.

## Findings Summary

### 1) High-confidence runtime issue
- `AvlnSamples/Avln_RenderDemo/Pages/CustomSkiaPage.cs`
  - `SKShader.CreateSweepGradient(new SKPoint((int)Bounds.Width / 2, (int)Bounds.Height / 2), ...)`
  - The conversion to `int` truncates fractional pixels and can distort the gradient center. This is a real precision loss and should be replaced with floating-point math that preserves the bounds.

### 2) Numeric-analysis warnings in calculator logic
- `AA05_CommandParCalc/AA05_CommandParCalc/Models/CalculatorModel.cs`
  - `Accumulator != 0d` triggers `cs/equality-on-floats` and should be reviewed for a semantic zero check rather than a raw float equality.
  - The switch fall-through `_ => (Func<double, double>?)null` is an unnecessary upcast; the null literal can be typed implicitly.
  - `Memory ?? 0d` and similar patterns may be intentional but should be reviewed because CodeQL flags some of them as constant/null checks after normalization.

### 3) Lower-confidence/secondary findings
- `AA06_ValueConverter2/AA06_Converters4/View/Controls/DynamicPlotCanvas.cs`
  - Repeated `cs/equality-on-floats` warnings likely come from zero checks in view/viewport math. These are valid to review but are lower priority unless the logic turns out to be unstable.
- `AA05_CommandParCalc/AA05_CommandParCalcTests/ViewModels/ViewModelBaseTests.cs`
  - Test-only float comparisons; they are not production defects and should be reviewed only after production fixes.
- `DevOps/Web.config`
  - Missing `X-Frame-Options` is a security/config issue, not a C# logic defect.

## Planned Changes
1. Fix the Skia gradient center so it keeps floating-point values and does not truncate the bounds.
2. Review the calculator state checks and replace ambiguous float comparisons with a deliberate zero tolerance or explicit non-zero logic.
3. Remove the explicit null upcast in the operation switch.
4. Decide whether to keep the UI plotting and test comparisons as-is or normalize them with epsilon-based checks.
5. Evaluate the web-header issue as a separate security configuration task.

## Acceptance Criteria
- The `CustomSkiaPage` gradient center is computed without integer truncation.
- The calculator model no longer emits the obvious static-analysis warnings caused by explicit nullable upcasts and ambiguous float checks.
- Remaining float warnings are either intentionally documented or explicitly justified as non-bugs.
- The web `X-Frame-Options` issue is tracked separately if not addressed in the same code change.
