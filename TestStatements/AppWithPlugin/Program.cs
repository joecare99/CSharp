// ------------------------------------------------------------
// Entry point for the console application.
// The Program class contains the static Main method that drives
// execution. This pattern is common in .NET console apps.
// ------------------------------------------------------------
namespace AppWithPlugin;

/// <summary>
/// The entry point for the console application.
/// </summary>
public class Program
{
    // --------------------------------------------------------
    // Main method – orchestrates initialization and execution.
    // --------------------------------------------------------
    public static void Main(string[] args)
    {
        // 1️⃣ Create the application instance.
        // Replace 'Model.AppWithPlugin' with the actual namespace/assembly
        // where your core App class resides.
        var app = new Model.AppWithPlugin();

        // 2️⃣ Initialize the app – typically loads plugins, configures services,
        // registers event handlers, etc. Pass any command‑line arguments here.
        app.Initialize(args);

        // 3️⃣ Run the primary workflow of the application.
        // This is where the core business logic lives; you can swap this
        // call for a different entry point if needed.
        app.Main(args);
    }
}
