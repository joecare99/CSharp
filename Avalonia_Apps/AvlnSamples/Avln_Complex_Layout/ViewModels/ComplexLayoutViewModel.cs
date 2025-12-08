// ***********************************************************************
// Assembly : Avln_Complex_Layout
// Author : Migration
// Created :2025-11-01
// ***********************************************************************
using Avalonia.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avln.ComplexLayout.Properties;

namespace Avln.ComplexLayout.ViewModels;

public partial class ComplexLayoutViewModel : BaseViewModelCT
{
 [ObservableProperty]
 private string _messageText = string.Empty;

 [ObservableProperty]
 private string _messageTitle = Resources.txtMsgTitle;

 [ObservableProperty]
 private bool _showMessage = false;

 [RelayCommand]
 private void Button1()
 {
 MessageText = Resources.txtBtn1React;
 ShowMessage = true;
 }

 [RelayCommand]
 private void Button2()
 {
 MessageText = Resources.txtBtn2React;
 ShowMessage = true;
 }

 [RelayCommand]
 private void Msg_OK()
 {
 ShowMessage = false;
 }
}
