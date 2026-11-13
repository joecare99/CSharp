using System;

namespace AA98_AvlnCodeStudio.Base.Testing.Models;

/// <summary>
/// Represents one result reported by a test runner for a particular run.
/// </summary>
public sealed record TestCaseResult(
    string TestName,
    string Outcome,
    TimeSpan? Duration,
    string? TestClassName,
    string? QualifiedTestName,
    string? TestId,
    string? ExecutionId);
