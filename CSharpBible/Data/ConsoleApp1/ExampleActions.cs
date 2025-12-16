using System;

namespace ConsoleApp1;

// 6. Beispielaktionen und Prädikate
public class ExampleActions
{
    public class ActionExample : IAction
    {
        public string Name => "ExampleAction";
        public string Execute(StateContext context)
        {
            // Beispiel für eine Aktion
            Console.WriteLine($"Executing {Name} in state {context.CurrentState}");
            context.AddData("LastAction", Name);
            return "SUCCESS";
        }
    }

    public class ConditionalAction : IAction
    {
        public string Name => "ConditionalAction";
        public string Execute(StateContext context)
        {
            if (context.GetData<string>("RequiredCondition") == "Met")
            {
                Console.WriteLine($"Executing {Name} in state {context.CurrentState}");
                return "SUCCESS";
            }
            return "FAILED";
        }
    }

    public class StateChangeAction : IAction
    {
        public string Name => "StateChangeAction";
        public string Execute(StateContext context)
        {
            context.AddData("StateChangeRequested", true);
            return "TRANSITION";
        }
    }

    public class SaveStateAction : IAction
    {
        public string Name => "SaveStateAction";
        public string Execute(StateContext context)
        {
            Console.WriteLine("Saving state...");
            context.AddData("StateSaved", DateTime.Now.ToString());
            return "SAVE_NEEDED";
        }
    }

    // Beispielprädikate
    public class DataPredicate : IStatePredicate
    {
        public string Key { get; }
        public string Value { get; }
        public string Message => $"Check for {Key} == {Value}";

        public DataPredicate(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public bool Check(StateContext context)
        {
            var data = context.GetData<string>(Key);
            return data == Value;
        }
    }

    public class TimeBasedPredicate : IStatePredicate
    {
        public string Message => "Time-based condition";

        public bool Check(StateContext context)
        {
            // Beispiel für zeitbasierte Bedingung
            var lastActionTime = context.GetData<DateTime?>("LastActionTime");
            return lastActionTime.HasValue &&
                   DateTime.Now - lastActionTime.Value > TimeSpan.FromMinutes(5);
        }
    }
}
