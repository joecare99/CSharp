using System.Collections.Generic;
using SharpHack.Base.Interfaces;

namespace SharpHack.Base.Model;

public abstract class ContainerItem : Item, IContainerItem
{
    private readonly List<Item> _items = new();

    public IList<Item> Items => _items;
}
