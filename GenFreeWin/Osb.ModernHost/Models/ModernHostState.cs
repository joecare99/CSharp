using Osb.Core.Tenancy;

namespace Osb.ModernHost.Models;

public sealed record ModernHostState(
    TenantPathConfiguration? Tenant,
    string? ErrorMessage)
{
    public bool IsReady => Tenant != null && ErrorMessage == null;
}
