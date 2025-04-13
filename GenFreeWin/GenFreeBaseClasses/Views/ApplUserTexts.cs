using Gen_FreeWin;
using GenFree.Interfaces.UI;
using System.Collections;
using System.Collections.Generic;

namespace GenFree.Views;
public class ApplUserTexts : IApplUserTexts
{
    private readonly List<string> _texte = new(560);

    public ApplUserTexts()
    {
        var l = new string[560];
        l.Initialize();
        _texte.AddRange(l);
    }

    public string this[object Idx]
    {
        get => Idx switch
        {
            int i => _texte[i],
            EUserText e => _texte[(int)e] ?? e.ToString(),
            _ => string.Empty
        };
        set
        {
            if (Idx is int i)
            {
                _texte[i] = value;
            }
            else if (Idx is EUserText e)
            {
                _texte[(int)e] = value;
            }
        }
    }

    public string this[int index] { get => this[(object)index]; set => this[(object)index] = value; }

    public int Count => _texte.Count;

    public bool IsReadOnly => false;

    public void Add(string item) => throw new System.NotImplementedException();

    public void Clear() => _texte.Clear();

    public bool Contains(string item) => _texte.Contains(item);

    public void CopyTo(string[] array, int arrayIndex) => _texte.CopyTo(array, arrayIndex);

    public IEnumerator<string> GetEnumerator() => _texte.GetEnumerator();

    public int IndexOf(string item) => _texte.IndexOf(item);

    public void Insert(int index, string item) => throw new System.NotImplementedException();

    public bool Remove(string item) => throw new System.NotImplementedException();

    public void RemoveAt(int index) => throw new System.NotImplementedException();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_texte).GetEnumerator();
}