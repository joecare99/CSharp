using System;
using System.Threading.Tasks;

namespace SyncAsyncParallel.Model;

public interface ISyncAsyncModel
{
    Task<long> Download_async(Action<string> actS);
    Task<long> Download_async_para(Action<string> actS);
    long Download_sync(Action<string> actS);
}