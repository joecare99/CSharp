using System;
using Avalonia.Controls;
using Avalonia;
using Avalonia.VisualTree;
using AA19_FilterLists.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using MsBox.Avalonia;

namespace AA19_FilterLists.Views;

public partial class PersonView : UserControl
{
 public PersonView()
 {
 InitializeComponent();
 if (Design.IsDesignMode) return;
 var vm = App.Services.GetRequiredService<PersonViewViewModel>();
 DataContext = vm;
 vm.MissingData += (_, __) => ShowError();
 }

 private async void ShowError()
 {
 var top = TopLevel.GetTopLevel(this);
 if (top is Window win)
 {
 await MessageBoxManager.GetMessageBoxStandard("Hinweis", "Bitte einen Vornamen oder Nachnamen eingeben.")
 .ShowWindowDialogAsync(win);
 }
 }
}
