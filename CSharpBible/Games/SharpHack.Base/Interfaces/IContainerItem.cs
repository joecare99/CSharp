using SharpHack.Base.Model;
using System.Collections.Generic;

namespace SharpHack.Base.Interfaces;

public interface IContainerItem
{
    IList<IItem> Items { get; }
}
