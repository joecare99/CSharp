using System;
using System.Collections.Generic;

namespace SharpHack.Engine.Pathfinding;

public class SimplePriorityQueue<TElement, TPriority> where TPriority : IComparable<TPriority>
{
    private readonly List<(TElement Element, TPriority Priority)> _nodes = new();

    public int Count => _nodes.Count;

    public void Enqueue(TElement element, TPriority priority)
    {
        _nodes.Add((element, priority));
        var i = _nodes.Count - 1;
        while (i > 0)
        {
            var parent = (i - 1) / 2;
            if (_nodes[parent].Priority.CompareTo(_nodes[i].Priority) <= 0) break;
            (_nodes[parent], _nodes[i]) = (_nodes[i], _nodes[parent]);
            i = parent;
        }
    }

    public TElement Dequeue()
    {
        if (_nodes.Count == 0) throw new InvalidOperationException("Queue ist leer.");
        
        var result = _nodes[0].Element;
        _nodes[0] = _nodes[_nodes.Count - 1];
        _nodes.RemoveAt(_nodes.Count - 1);

        var i = 0;
        while (true)
        {
            var left = i * 2 + 1;
            var right = i * 2 + 2;
            var smallest = i;

            if (left < _nodes.Count && _nodes[left].Priority.CompareTo(_nodes[smallest].Priority) < 0)
                smallest = left;
            if (right < _nodes.Count && _nodes[right].Priority.CompareTo(_nodes[smallest].Priority) < 0)
                smallest = right;

            if (smallest == i) break;
            (_nodes[i], _nodes[smallest]) = (_nodes[smallest], _nodes[i]);
            i = smallest;
        }

        return result;
    }
}