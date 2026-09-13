using System;

namespace Config.Service.Tests;

public class SensitiveSectionProvider : IConfigSectionProvider
{
    public string Name => "Sensitive";
    public string DisplayName => "Sensible Konfiguration";
    public int Order => 0;
    public Type ModelType => typeof(TestModel);

    public string? Description => throw new NotImplementedException();

    public object CreateModel() => new TestModel();
}
