using System;
using Avalonia.Controls;
using AA14_ScreenX.ViewModels;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using ScreenX.Base;

namespace AA14_ScreenX.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new RenderViewModel(new RendererService());
    }

}
