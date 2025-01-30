// ***********************************************************************
// Assembly         : SyncAsyncParallel
// Author           : Mir
// Created          : 12-26-2021
//
// Last Modified By : Mir
// Last Modified On : 12-29-2021
// ***********************************************************************
// <copyright file="SyncAsyncViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2021
// </copyright>
// <summary></summary>
// ***********************************************************************
using MVVM.ViewModel;
using SyncAsyncParallel.Model;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace SyncAsyncParallel.ViewModels;

/// <summary>
/// Class SyncAsyncViewModel.
/// Implements the <see cref="BaseViewModelCT" />
/// </summary>
/// <seealso cref="BaseViewModelCT" />
public partial class SyncAsyncViewModel(ISyncAsyncModel iModel) : BaseViewModelCT
{
    /// <summary>
    /// The information text
    /// </summary>
    [ObservableProperty]
    private string _infoText="";

    private ISyncAsyncModel _iModel = iModel;

    /// <summary>
    /// Initializes a new instance of the <see cref="SyncAsyncViewModel"/> class.
    /// </summary>
    public SyncAsyncViewModel():this(new SyncAsyncModel())
    {
    }


    /// <summary>
    /// Downloads the synchronize.
    /// </summary>
    [RelayCommand]
    private void Download_sync()
    {
        var ElapsedMilliseconds = _iModel.Download_sync((s)=> InfoText =s);
        InfoText += $"Total Execution Time {ElapsedMilliseconds}ms";
    }

    /// <summary>
    /// Downloads the asynchronous.
    /// </summary>
    [RelayCommand]
    async Task Download_async()
    {
        long watch = await _iModel.Download_async((s) => InfoText = s);
        InfoText += $"Total Execution Time {watch}ms";
    }

    /// <summary>
    /// Downloads the asynchronous para.
    /// </summary>
    [RelayCommand]
    async Task Download_async_para()
    {
        long watch = await _iModel.Download_async_para((s) => InfoText = s);
        InfoText += $"Total Execution Time {watch}ms";
    }

}
