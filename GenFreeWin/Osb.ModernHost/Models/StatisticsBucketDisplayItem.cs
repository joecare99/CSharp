namespace Osb.ModernHost.Models;

public sealed record StatisticsBucketDisplayItem(string Label, int Count)
{
    public string DisplayText => $"{Label}: {Count}";

    public override string ToString() => DisplayText;
}
