using System;
using System.Drawing;
using System.IO;
using System.Linq;
using ConsoleLib;
using ConsoleLib.Showcase.Apps;

namespace ConsoleLib.Showcase.Apps.TicTacToe;

public sealed class TicTacToeAppModule : IShowcaseAppModule
{
    public void Register(ShowcaseAppRegistrationContext context)
    {
        context.Register(new ShowcaseAppDescriptor(
            "TicTacToe",
            "Tic-Tac-Toe",
            ShowcaseAppCategory.Games,
            "A two-player Tic-Tac-Toe game.",
            new Point(4, 3),
            new Size(32, 17),
            _ => new TicTacToeViewModel(),
            (_, viewModel) => Load(viewModel)));
    }

    private static CxamlLoadResult Load(object viewModel)
    {
        var assembly = typeof(TicTacToeAppModule).Assembly;
        var resourceName = assembly.GetManifestResourceNames()
            .Single(name => name.EndsWith(".TicTacToe.cxaml", StringComparison.Ordinal));
        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException("The Tic-Tac-Toe CXAML resource is missing.");
        using var reader = new StreamReader(stream);
        return new CxamlLoader().LoadPage(reader, new CxamlLoadContext(viewModel));
    }
}
