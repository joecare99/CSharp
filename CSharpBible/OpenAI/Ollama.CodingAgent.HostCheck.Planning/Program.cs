using System;
using Ollama.CodingAgent;

namespace Ollama.CodingAgent.HostCheck.Planning;

internal static class Program
{
    private static int Main(string[] args)
    {
        string prompt = args.Length > 0
            ? string.Join(" ", args)
            : "Build and test the coding agent and summarize next actions.";

        Console.WriteLine("== Planning HostCheck ==");
        Console.WriteLine($"Prompt: {prompt}");
        Console.WriteLine();

        PlanState plan = SubtaskPlanner.CreateInitialPlan(prompt);
        Console.WriteLine(PlanStateRenderer.Render(plan));
        Console.WriteLine();

        bool driftSignal = GoalDriftAnalyzer.IsDriftDetected(
            plan.GoalContract,
            plan.Subtasks[0],
            "operation was canceled");
        Console.WriteLine($"Drift signal (sample failure output): {driftSignal}");
        Console.WriteLine();

        Console.WriteLine("Malformed input checks:");
        try
        {
            _ = SubtaskPlanner.CreateInitialPlan(string.Empty);
            Console.WriteLine("Unexpected: empty prompt accepted.");
            return 2;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Expected failure for empty prompt: {ex.GetType().Name}");
        }

        try
        {
            _ = new GoalContract(string.Empty, []);
            Console.WriteLine("Unexpected: invalid goal contract accepted.");
            return 3;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Expected failure for malformed goal contract: {ex.GetType().Name}");
        }

        return 0;
    }
}
