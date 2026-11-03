using AA98_AvlnCodeStudio.Model.Workspaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace AA98_AvlnCodeStudio.Tests.Engineering;

[TestClass]
public sealed class AxamlMvvmMemberPlannerTests
{
    [TestMethod]
    public void Plan_ReusesCompatibleWritablePropertyAndCommand()
    {
        AxamlMvvmMemberPlanEntry[] plan = new AxamlMvvmMemberPlanner().Plan(
            [
                new AxamlMvvmMemberRequirement("Name", "string", IsCommand: false),
                new AxamlMvvmMemberRequirement("SaveCommand", "IRelayCommand", IsCommand: true),
            ],
            [
                new AxamlMvvmSemanticMember("Name", "string", IsCommand: false, IsWritable: true),
                new AxamlMvvmSemanticMember("SaveCommand", "IRelayCommand", IsCommand: true, IsWritable: false),
            ],
            createMissingMembers: false).ToArray();

        Assert.AreEqual(2, plan.Length);
        Assert.IsTrue(plan.All(static entry => entry.Action == AxamlMvvmMemberPlanAction.Reuse));
    }

    [TestMethod]
    public void Plan_RequiresExplicitChoiceBeforeCreatingMissingMember()
    {
        AxamlMvvmMemberPlanEntry entry = new AxamlMvvmMemberPlanner().Plan(
            [new AxamlMvvmMemberRequirement("Name", "string", IsCommand: false)],
            [],
            createMissingMembers: false).Single();

        Assert.AreEqual(AxamlMvvmMemberPlanAction.NeedsCreationChoice, entry.Action);
        Assert.IsNotNull(entry.Diagnostic);
    }

    [TestMethod]
    public void Plan_CreatesMissingMemberOnlyWhenExplicitlyRequested()
    {
        AxamlMvvmMemberPlanEntry entry = new AxamlMvvmMemberPlanner().Plan(
            [new AxamlMvvmMemberRequirement("Name", "string", IsCommand: false)],
            [],
            createMissingMembers: true).Single();

        Assert.AreEqual(AxamlMvvmMemberPlanAction.Create, entry.Action);
        Assert.IsNull(entry.Diagnostic);
    }

    [TestMethod]
    public void Plan_RejectsTypeNullabilityAndAccessorMismatches()
    {
        AxamlMvvmMemberPlanEntry[] plan = new AxamlMvvmMemberPlanner().Plan(
            [
                new AxamlMvvmMemberRequirement("OptionalName", "string", IsCommand: false),
                new AxamlMvvmMemberRequirement("ReadOnlyName", "string", IsCommand: false),
                new AxamlMvvmMemberRequirement("WrongKind", "IRelayCommand", IsCommand: true),
            ],
            [
                new AxamlMvvmSemanticMember("OptionalName", "string?", IsCommand: false, IsWritable: true),
                new AxamlMvvmSemanticMember("ReadOnlyName", "string", IsCommand: false, IsWritable: false),
                new AxamlMvvmSemanticMember("WrongKind", "string", IsCommand: false, IsWritable: true),
            ],
            createMissingMembers: true).ToArray();

        Assert.IsTrue(plan.All(static entry => entry.Action == AxamlMvvmMemberPlanAction.Conflict));
        Assert.IsTrue(plan.All(static entry => entry.Diagnostic is not null));
    }

    [TestMethod]
    public void Plan_ReportsDuplicateBindingTargetsAsConflict()
    {
        AxamlMvvmMemberPlanEntry[] plan = new AxamlMvvmMemberPlanner().Plan(
            [
                new AxamlMvvmMemberRequirement("Name", "string", IsCommand: false),
                new AxamlMvvmMemberRequirement("Name", "string", IsCommand: false),
            ],
            [],
            createMissingMembers: true).ToArray();

        Assert.AreEqual(AxamlMvvmMemberPlanAction.Create, plan[0].Action);
        Assert.AreEqual(AxamlMvvmMemberPlanAction.Conflict, plan[1].Action);
    }

    [TestMethod]
    public void Plan_UsesAnalyzedAxamlBindingsAgainstResolvedViewModelMembers()
    {
        const string markup = """
            <Window xmlns="https://github.com/avaloniaui">
              <TextBox Text="{Binding Name, Mode=TwoWay}" />
              <Button Command="{Binding SaveCommand}" />
            </Window>
            """;

        AxamlMvvmMemberRequirement[] requirements = new AxamlMvvmBindingAnalyzer()
            .Analyze(markup)
            .ToArray();
        AxamlMvvmMemberPlanEntry[] plan = new AxamlMvvmMemberPlanner().Plan(
            requirements,
            [
                new AxamlMvvmSemanticMember("Name", "string", IsCommand: false, IsWritable: true),
                new AxamlMvvmSemanticMember("SaveCommand", "IRelayCommand", IsCommand: true, IsWritable: false),
            ],
            createMissingMembers: false).ToArray();

        Assert.AreEqual(2, plan.Length);
        Assert.IsTrue(plan.All(static entry => entry.Action == AxamlMvvmMemberPlanAction.Reuse));
    }
}
