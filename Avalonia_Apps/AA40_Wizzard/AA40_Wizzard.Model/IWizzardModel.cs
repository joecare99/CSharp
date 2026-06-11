using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AA40_Wizzard.Model;

/// <summary>
/// Defines the wizard state shared across the application.
/// </summary>
public interface IWizzardModel : INotifyPropertyChanged
{
    /// <summary>
    /// Gets the current time from the configured clock.
    /// </summary>
    DateTime Now { get; }

    /// <summary>
    /// Gets or sets the main selection identifier.
    /// </summary>
    int MainSelection { get; set; }

    /// <summary>
    /// Gets the available main options.
    /// </summary>
    IList<int> MainOptions { get; }

    /// <summary>
    /// Gets or sets the sub selection identifier.
    /// </summary>
    int SubSelection { get; set; }

    /// <summary>
    /// Gets the available sub options.
    /// </summary>
    IList<int> SubOptions { get; }

    /// <summary>
    /// Gets or sets the first additional selection identifier.
    /// </summary>
    int Additional1 { get; set; }

    /// <summary>
    /// Gets or sets the second additional selection identifier.
    /// </summary>
    int Additional2 { get; set; }

    /// <summary>
    /// Gets or sets the third additional selection identifier.
    /// </summary>
    int Additional3 { get; set; }

    /// <summary>
    /// Gets the available additional options.
    /// </summary>
    IList<int> AdditOptions { get; }
}
