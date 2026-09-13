# G-Component-Quality

## Status
Passed

## Required evidence
- net8.0 build and MSTest result.
- Available net9.0/net10.0 build result.
- Success, boundary, error, and regression tests.
- NSubstitute use for external dependencies.
- Nullable and documentation review.
- English SVN commit revision/message.
- Current and next task/backlog/feature status.

## Decision
Approved for B-Config-Core. T-004 links the complete test, build, host
registration, warning-policy, and status-handover evidence. The gate remains
applicable to subsequent component backlogs.

## B-Code-Navigation evidence
T-007 confirms the gate for B-Code-Navigation: the editor fixture passed 16/16
tests, both shared contract test projects passed 9/9 tests, and both production
libraries built for net8.0, net9.0, and net10.0. The neutral contract has no
Avalonia or AvaloniaEdit dependency, and the consumer integration rules are
documented in T-007.

## B-Property-Editor evidence
T-010 confirms the gate for B-Property-Editor. The neutral contract suite
passed 10/10 on net8.0, net9.0, and net10.0. The dedicated
`Property.Editor.Avalonia.Tests` suite passed 9/9 on each framework, including
scalar conversion, nullable values, Boolean and enum editing, invalid-value
preservation, read-only behavior, ordering, and an NSubstitute fixture that
depends only on `IPropertyItem`. CodeStudio host integration passed 112/112 on
net8.0/net9.0 and 126/126 on net10.0. The UI owns Avalonia templates and typed
display conversion; the neutral component remains independent of UI, products,
planning, ConsoleLib, and OS concerns.
