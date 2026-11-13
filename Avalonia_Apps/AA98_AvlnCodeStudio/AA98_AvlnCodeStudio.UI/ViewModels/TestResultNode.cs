using System.Collections.Generic;

namespace AA98_AvlnCodeStudio.UI.ViewModels;

public sealed record TestResultNode(string Label, IReadOnlyList<TestResultNode> Children);
