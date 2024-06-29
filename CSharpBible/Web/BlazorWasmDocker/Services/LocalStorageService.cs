using Microsoft.JSInterop;
using System.Text.Json;
using System.Threading.Tasks;

namespace LocalStorage.Services
{
    public class LocalStorageService : ILocalStorageService
    {
        private IJSRuntime js;
        public LocalStorageService(IJSRuntime JsRuntime)
        {
            js = JsRuntime;
        }

        public async ValueTask SetItemAsync<T>(string key, T item)
        {
            await js.InvokeVoidAsync(
                "bwdInterop.setLocalStorage",
                 key,
                 JsonSerializer.Serialize(item));
        }

        public async ValueTask<T?> GetItemAsync<T>(string key)
        {
            try
            {
                var json = await js.InvokeAsync<string>
                    ("bwdInterop.getLocalStorage", key);
                return JsonSerializer.Deserialize<T>(json);
            }
            catch
            {
                return default;
            }
        }

    }
}
