using System.Collections.Generic;
using SharpHack.Base.Interfaces;

namespace SharpHack.Base.Model;

public abstract class ContainerItem : Item, IContainerItem
{
    private readonly IList<IItem> _items = [];

    public IList<IItem> Items => _items;
}
