using Trnsp.Show.Lfm.Models.Components;
using TranspilerLib.Pascal.Models;

namespace Trnsp.Show.Lfm.Services;

/// <summary>
/// Factory interface for creating LFM components.
/// </summary>
public interface IComponentFactory
{
    /// <summary>
    /// Creates a component from an LfmObject.
    /// </summary>
    LfmComponentBase CreateComponent(LfmObject lfmObject);

    /// <summary>
    /// Creates a component tree from a root LfmObject.
    /// </summary>
    LfmComponentBase? CreateComponentTree(LfmObject? rootObject);
}
