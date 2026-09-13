using System;
using System.IO;

namespace ConsoleLib.CommonControls;

/// <summary>Creates fresh dialog instances from reusable CXAML resources.</summary>
public sealed class DialogResourceLoader
{
    public Dialog Load(string markup, object dataContext)
    {
        if (markup is null)
            throw new ArgumentNullException(nameof(markup));
        if (dataContext is null)
            throw new ArgumentNullException(nameof(dataContext));

        return (Dialog)new CxamlLoader()
            .LoadDialog(new StringReader(markup), new CxamlLoadContext(dataContext))
            .Root;
    }
}
