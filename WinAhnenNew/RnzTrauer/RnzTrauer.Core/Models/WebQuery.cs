using OpenQA.Selenium;
using System.Collections.Generic;

namespace RnzTrauer.Core;

/// <summary>
/// Describes a recursive Selenium query used to capture HTML fragments into dictionaries.
/// </summary>
public sealed class WebQuery
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WebQuery"/> class.
    /// </summary>
    public WebQuery(string sName, By bySelector, params WebQuery[] arrChildren)
    {
        Name = sName;
        By = bySelector;
        Children = arrChildren;
    }

    /// <summary>
    /// Gets the target property name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the Selenium selector.
    /// </summary>
    public By By { get; }

    /// <summary>
    /// Gets the nested child queries.
    /// </summary>
    public IReadOnlyList<WebQuery> Children { get; }
}
