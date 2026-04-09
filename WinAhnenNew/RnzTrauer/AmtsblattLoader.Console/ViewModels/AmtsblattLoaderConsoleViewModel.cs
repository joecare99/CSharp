using AmtsblattLoader.Console.Views;
using RnzTrauer.Core;

namespace AmtsblattLoader.Console.ViewModels;

/// <summary>
/// Coordinates the Amtsblatt console workflow.
/// </summary>
public sealed class AmtsblattLoaderConsoleViewModel
{
    private readonly ConsoleOutputView _view;

    /// <summary>
    /// Initializes a new instance of the <see cref="AmtsblattLoaderConsoleViewModel"/> class.
    /// </summary>
    public AmtsblattLoaderConsoleViewModel(ConsoleOutputView xView)
    {
        _view = xView;
    }

    /// <summary>
    /// Runs the Amtsblatt loader workflow.
    /// </summary>
    public void Run(AmtsblattConfig xConfig)
    {
        using var xWebHandler = new AmtsblattWebHandler(xConfig);
        xWebHandler.InitPage();
        var iOffset = 1;
        var iDayDelta = 0;
        while (iDayDelta <= 400)
        {
            var dtCurrent = DateOnly.FromDateTime(DateTime.Today).AddDays(-(iDayDelta + iOffset));
            iDayDelta += 7;
            var iWeek = (dtCurrent.DayOfYear - 1) / 7;
            var sStart = $"{xConfig.Url}-{iWeek:00}-{dtCurrent.Year:0000}";
            _view.WriteLine($"Load: {sStart}");
            xWebHandler.GetData1(sStart);
        }
    }
}
