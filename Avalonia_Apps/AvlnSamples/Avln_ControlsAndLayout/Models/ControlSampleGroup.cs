using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Avln_ControlsAndLayout.Models;

/// <summary>
/// Groups related controls and layout samples for the sample library navigation.
/// </summary>
/// <param name="Title">The group title.</param>
/// <param name="Samples">The contained samples.</param>
public sealed class ControlSampleGroup(string title, IEnumerable<ControlSample> samples)
{
    public string Title { get; } = title;

    public ObservableCollection<ControlSample> Samples { get; } = new(samples.ToArray());
}
