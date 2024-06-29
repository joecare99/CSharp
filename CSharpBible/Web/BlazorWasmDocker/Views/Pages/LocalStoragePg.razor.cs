using BlazorWasmDocker.Models;
using LocalStorage.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace BlazorWasmDocker.Views.Pages
{
    public partial class LocalStoragePg    
    {
        [Inject]
        ILocalStorageService? localStorage { get; set; }
        [Inject]
        IJSRuntime js { get; set; }
        private IJSObjectReference? module;
        private string? data;

        async Task SaveToLocalStorageAsync()
        {
            var dataInfo = new DataInfo()
            {
                Value = data,
                Length = data!.Length,
                Timestamp = DateTime.Now
            };

            await localStorage!.SetItemAsync<DataInfo?>(
                "localStorageData",
                dataInfo);
        }

        protected async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                module = await js.InvokeAsync<IJSObjectReference>
                        ("import", "./Views/Pages/LocalstoragePg.razor.js");
            }
        }
        async Task ReadFromLocalStorageAsync()
        {
            await OnAfterRenderAsync(module == null);

            if (module is not null)
            {
                DataInfo? savedData =
                    await localStorage!.GetItemAsync
                        <DataInfo>("localStorageData");

                string result =
                    $"localStorageData = {savedData!.Value}";

                await module.InvokeVoidAsync
                    ("showLocalStorage", result);
            }
        }

    }
}
