using System.Threading.Tasks;

namespace TestStatements.DependencyInjection
{
    internal interface IObjectStore
    {
        Task<IItem?> GetNextAsync();
        Task<IItem?> MarkAsync(IItem? next);
    }
}