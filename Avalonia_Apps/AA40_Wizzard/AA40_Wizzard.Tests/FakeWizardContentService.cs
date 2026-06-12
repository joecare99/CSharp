using System.Collections.Generic;
using System.Linq;
using AA40_Wizzard.Model;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace AA40_Wizzard.Tests;

internal sealed class FakeWizardContentService : IWizardContentService
{
    public string GetText(string key) => $"text:{key}";

    public IReadOnlyList<ListEntry> GetOptions(IEnumerable<int> optionIds, string resourcePrefix)
        => optionIds.Select(optionId => new ListEntry(optionId, $"{resourcePrefix}:{optionId}")).ToList();

    public Control? GetDocumentPreview(int? selectionId)
        => selectionId is null || selectionId < 0 ? null : new TextBlock { Text = $"docpreview:{selectionId}" };

    public Bitmap? GetImage(int? selectionId)
        => selectionId is null || selectionId < 0
            ? null
            : new WriteableBitmap(new PixelSize(1, 1), new Vector(96, 96), PixelFormat.Rgba8888, AlphaFormat.Premul);
}
