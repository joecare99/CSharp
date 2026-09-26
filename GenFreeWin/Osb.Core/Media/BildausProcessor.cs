using System;
using System.Collections.Generic;
using System.Drawing;
using Osb.Core.Imaging;

namespace Osb.Core.Media;

/// <summary>
/// Orchestrates reading picture entries, resolving their paths and loading/scaling images.
/// Returns plain result objects without any UI side-effects.
/// </summary>
public sealed class BildausResult
{
    public BildausEntry Entry { get; set; } = null!;

    public Bitmap? Image { get; set; }

    public string Caption { get; set; } = string.Empty;

    public string Remark { get; set; } = string.Empty;
}

public sealed class BildausProcessor
{
    private readonly IBildausRepository _repository;
    private readonly string _pictureBaseDirectory;

    public BildausProcessor(IBildausRepository repository, string pictureBaseDirectory)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _pictureBaseDirectory = pictureBaseDirectory ?? string.Empty;
    }

    public IEnumerable<BildausResult> Process(BildausKind kind, int ownerNumber, BildausOptions options)
    {
        foreach (var entry in _repository.EnumerateEntries(kind, ownerNumber))
        {
            if (!BildausFilter.MatchesOptions(entry, options)) continue;

            // Resolve path and try to load the image
            var fullPath = BildausPathResolver.ResolvePath(entry, _pictureBaseDirectory);

            Bitmap? bitmap = null;
            try
            {
                bitmap = BildausImageLoader.LoadBitmap(fullPath);

                // If target height is provided use ImageLayoutCalculator
                if (options.TargetHeight > 0)
                {
                    var size = ImageLayoutCalculator.CalculateSizeForHeight(new ImageSize(bitmap.Width, bitmap.Height), options.TargetHeight);
                    var resized = new Bitmap(size.Width, size.Height);
                    using (var g = Graphics.FromImage(resized))
                    {
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.DrawImage(bitmap, 0, 0, size.Width, size.Height);
                    }

                    bitmap.Dispose();
                    bitmap = resized;
                }
            }
            catch (Exception)
            {
                // Swallow here; the UI adapter will decide whether to report errors.
                bitmap = null;
            }

            var (caption, remark) = BildausCaptionBuilder.Build(entry);

            yield return new BildausResult
            {
                Entry = entry,
                Image = bitmap,
                Caption = caption,
                Remark = remark
            };
        }
    }
}
