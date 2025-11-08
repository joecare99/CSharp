using System;
using System.Collections.Generic;
using System.Numerics;

namespace DataAnalysis.Core.Models;

public static partial class DbScan
{

    public static List<List<Vector2>> Cluster(IReadOnlyList<Vector2> points, double eps, int minPts)
    {
        var clusters = new List<List<Vector2>>();
        if (points.Count == 0)
            return clusters;
        var visited = new bool[points.Count];
        var assigned = new int[points.Count];
        Array.Fill(assigned, -1);
        var eps2 = eps * eps;

        for (int i = 0; i < points.Count; i++)
        {
            if (visited[i])
                continue;
            visited[i] = true;
            var neighbors = RegionQuery(points, i, eps2);
            if (neighbors.Count < minPts)
            {
                continue; // noise
            }
            var cluster = new List<Vector2>();
            clusters.Add(cluster);
            ExpandCluster(points, i, neighbors, cluster, visited, assigned, eps2, minPts, clusters.Count - 1);
        }
        return clusters;
    }

    private static void ExpandCluster(IReadOnlyList<Vector2> points, int index, List<int> neighbors, List<Vector2> cluster,
    bool[] visited, int[] assigned, double eps2, int minPts, int clusterId)
    {
        cluster.Add(points[index]);
        assigned[index] = clusterId;
        for (int nIdx = 0; nIdx < neighbors.Count; nIdx++)
        {
            int j = neighbors[nIdx];
            if (!visited[j])
            {
                visited[j] = true;
                var neighbors2 = RegionQuery(points, j, eps2);
                if (neighbors2.Count >= minPts)
                {
                    // merge neighbors2 into neighbors
                    foreach (var k in neighbors2)
                    {
                        if (!neighbors.Contains(k))
                            neighbors.Add(k);
                    }
                }
            }
            if (assigned[j] == -1)
            {
                cluster.Add(points[j]);
                assigned[j] = clusterId;
            }
        }
    }

    private static List<int> RegionQuery(IReadOnlyList<Vector2> pts, int i, double eps2)
    {
        var res = new List<int>();
        var p = pts[i];
        for (int j = 0; j < pts.Count; j++)
        {
            if (j == i)
                continue;
            var q = pts[j];
            var dx = p.X - q.X;
            var dy = p.Y - q.Y;
            var d2 = dx * dx + dy * dy;
            if (d2 <= eps2)
                res.Add(j);
        }
        return res;
    }
}
