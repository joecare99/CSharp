using System.Threading.Tasks;

namespace TestStatements.DependencyInjection
{
    internal interface IObjectProcessor
    {
        Task ProcessAsync(IItem? next);
    }
}