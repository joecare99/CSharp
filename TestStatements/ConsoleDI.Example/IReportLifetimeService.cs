using Microsoft.Extensions.DependencyInjection;
using System;

namespace ConsoleDI.Example;

public interface IReportServiceLifetime
{
    Guid Id { get; }

    ServiceLifetime Lifetime { get; }
}