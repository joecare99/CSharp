namespace ConsoleLib.Showcase.Apps;

/// <summary>Explicitly registers one reusable application feature with a host.</summary>
public interface IShowcaseAppModule
{
    void Register(ShowcaseAppRegistrationContext context);
}
