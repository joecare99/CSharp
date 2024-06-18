using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sudoku_Base.Models.Interfaces;

namespace Sudoku_Base.Models;

public class UndoInformation : IUndoInformation
{
    public WeakReference<ISudokuField> Field { get; }
    public IList<(object? ov, object? nv)> list { get; }

    public UndoInformation(ISudokuField field, IList<(object? ov, object? nv)> list)
    {
        Field = new WeakReference<ISudokuField>(field);
        this.list = list;
    }

    public void Redo()
    {
        foreach (var (ov, nv) in list)
        {
            var xf = Field.TryGetTarget(out var f);
            if (ov is bool b && nv is bool b2 && xf && f!.IsPredefined == b)
            {
                f.IsPredefined = b2;
            }
            else if (ov is int i && xf && f!.Value == i || ov == null && xf)
            {
                f.Value = nv as int?;
            }
            else if (nv is IList<int> li && xf)
            {
                f.PossibleValues.Clear();
                foreach (var l in li)
                {
                    f.PossibleValues.Add(l);
                }
            }
        }
    }

    public void Undo()
    {
        foreach (var (ov, nv) in list)
        {
            var xf = Field.TryGetTarget(out var f);
            if (nv is bool b && ov is bool b2 && xf && f!.IsPredefined == b)
            {
                f.IsPredefined = b2;
            }
            else if (nv is int i && xf && f!.Value == i || nv == null && xf)
            {
                f.Value = ov as int?;
            }
            else if (ov is IList<int> li && xf)
            {
                f.PossibleValues.Clear();
                foreach (var l in li)
                {
                    f.PossibleValues.Add(l);
                }
            }
        }
    }

    public void TryUpdateNewValue(object newValue)
    {
        int i2;
        switch (newValue)
        {
            case bool b:
                var le = list.FirstOrDefault(l => l.ov is bool);
                if ((i2 = list.IndexOf(le)) >= 0)
                {
                    list[i2] = (le.ov, b);
                }
                break;
            case int i:
            case null:
                var le2 = list.FirstOrDefault(l => l.ov is int?);
                if ((i2 = list.IndexOf(le2)) >= 0)
                {
                    list[i2] = (le2.ov, newValue);
                }

                break;
            case IEnumerable<int> li:
                var le3 = list.FirstOrDefault(l => l.ov is IEnumerable<int>);
                if ((i2 = list.IndexOf(le3)) >= 0)
                {
                    list[i2] = (le3.ov, newValue);
                }
                break;
            default:
                break;
        }
    }
}
