using System.Threading.Tasks;

namespace TestStatements.DependencyInjection
{
    internal interface IObjectRelay
    {
        Task RelayAsync(IItem next);
    }
}