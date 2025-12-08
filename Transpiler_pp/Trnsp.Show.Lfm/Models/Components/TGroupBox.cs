using System;
using System.Windows.Media;

namespace Trnsp.Show.Lfm.Models.Components;

/// <summary>
/// Represents a TGroupBox component (labeled container).
/// </summary>
public partial class TGroupBox : LfmComponentBase
{
    public TGroupBox()
    {
        Height = 105;
        Width = 185;
        Color = Color.FromRgb(240, 240, 240);
    }
}
