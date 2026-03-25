using System;

namespace Trnsp.Show.Lfm.Services.Interfaces
{
    public interface IObjectResolver
    {
        void RegisterObject(string name, object instance);
        void ResolveOrDefer(string name, object requestingComponent, Action<object> linkAction);
    }
}