using SharpHack.Base.Model;

namespace SharpHack.Base.Interfaces;

public interface IContainerItem
{
    IList<Item> Items { get; }
}
