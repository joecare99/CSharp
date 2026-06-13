using System.Collections.Generic;

namespace AA15_Labyrinth.Model;

//12-direction grid step (projected to integer lattice)
public readonly record struct Dir(int Dx, int Dy)
{
    public static readonly Dir[] All = BuildAll();
    private static Dir[] BuildAll()
    {
        HashSet<(int, int)> list = new HashSet<(int, int)>();
        (int dx, int dy)[] baseDirs = new (int dx, int dy)[] { (2, 0), (2, 1), (1, 2) };
        foreach ((int dx, int dy) in baseDirs)
        {
            list.Add((dx, dy));
            list.Add((dy, -dx));
            list.Add((-dy, dx));
            list.Add((-dx, -dy));
        }
        Dir[] arr = new Dir[list.Count];
        int i = 0; 
        foreach ((int, int) v in list) 
            arr[i++] = new Dir(v.Item1, v.Item2);
        return arr;
    }
}
