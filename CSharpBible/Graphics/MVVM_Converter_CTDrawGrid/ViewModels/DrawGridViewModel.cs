using BaseLib.Helper;
using CommunityToolkit.Mvvm.Input;
using BaseLib.Helper;
using MVVM.ViewModel;
using MVVM_Converter_CTDrawGrid.Models.Interfaces;
using System;
using System.Reflection;

namespace MVVM_Converter_CTDrawGrid.ViewModel;

/// <summary>
/// Class DrawGridViewModel.
/// Implements the <see cref="BaseViewModel" />
/// </summary>
/// <seealso cref="BaseViewModel" />
public partial class DrawGridViewModel:BaseViewModelCT
{
    private IDrawGridModel _drawGridModel;

    /// <summary>
    /// Gets or sets the show client.
    /// </summary>
    /// <value>The show client.</value>
    public Func<string, BaseViewModel?>? ShowClient { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DrawGridViewModel"/> class.
    /// </summary>
    public DrawGridViewModel(): this(IoC.GetRequiredService<IDrawGridModel>())
    {
    }
    public DrawGridViewModel(IDrawGridModel drawGridModel)
    {
        this._drawGridModel = drawGridModel;
    }

    /// <summary>Gets the plot frame source.</summary>
    /// <value>The plot frame source.</value>
    public string PlotFrameSource => $"/{Assembly.GetExecutingAssembly().GetName().Name};component/Views/PlotFrame.xaml";

    /// <summary>
    /// Gets or sets the load level command.
    /// </summary>
    /// <value>The load level command.</value>
    [RelayCommand]
    private void LoadLevel() 
        => _drawGridModel.LoadLevel();
    /// <summary>
    /// Gets or sets the next level command.
    /// </summary>
    /// <value>The next level command.</value>
    [RelayCommand]
    private void NextLevel() 
        => _drawGridModel.NextLevel();
    /// <summary>
    /// Gets or sets the previous level command.
    /// </summary>
    /// <value>The previous level command.</value>
    [RelayCommand]
    private void PrevLevel() 
        => _drawGridModel.PrevLevel();

}
