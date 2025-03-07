﻿// ***********************************************************************
// Assembly         : MVVM_AllExamples
// Author           : Mir
// Created          : 05-19-2023
//
// Last Modified By : Mir
// Last Modified On : 05-19-2023
// ***********************************************************************
// <copyright file="AllExampleModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
using CommunityToolkit.Mvvm.ComponentModel;
using MVVM_AllExamples.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;

/// <summary>
/// The Models namespace.
/// </summary>
/// <autogeneratedoc />
namespace MVVM_AllExamples.Models;

/// <summary>
/// Class AllExampleModel.
/// Implements the <see cref="ObservableObject" />
/// Implements the <see cref="MVVM_AllExamples.Models.IAllExampleModel" />
/// </summary>
/// <seealso cref="ObservableObject" />
/// <seealso cref="MVVM_AllExamples.Models.IAllExampleModel" />
/// <autogeneratedoc />
public partial class AllExampleModel : ObservableObject, IAllExampleModel
{
    #region Properties
    /// <summary>
    /// The timer
    /// </summary>
    /// <autogeneratedoc />
    private readonly Timer _timer;
    /// <summary>
    /// Gets or sets the get now.
    /// </summary>
    /// <value>The get now.</value>
    /// <autogeneratedoc />
    public static Func<DateTime> GetNow { get; set; } = () => DateTime.Now;
    /// <summary>
    /// Gets the now.
    /// </summary>
    /// <value>The now.</value>
    /// <autogeneratedoc />
    public DateTime Now { get => GetNow(); }

    public List<ExItem> Examples { get; } = [
        ("Template", typeof(MVVM_00_Template.Views.TemplateView), null),
        ("CTTemplate", typeof(MVVM_00a_CTTemplate.Views.TemplateView), null),
        ("NotifyChangeView", typeof(MVVM_03_NotifyChange.Views.NotifyChangeView), null),
        ("DelegateCommandView", typeof(MVVM_04_DelegateCommand.Views.DelegateCommandView), null),
        ("RelayCommandView", typeof(MVVM_04a_CTRelayCommand.Views.RelayCommandView),null ),
        ("CommandParCalculatorView",typeof(MVVM_05_CommandParCalculator.Views.CommandParCalculatorView),null),
        ("CommandParCalculatorView2",typeof(MVVM_05_CommandParCalculator.Views.Page1),null),
        ("CurrencyView", typeof(MVVM_06_Converters.View.CurrencyView), null),
        ("Currency2View", typeof(MVVM_06_Converters_2.View.Currency2View), null),
        ("CurrencyView3", typeof(MVVM_06_Converters_3.View.CurrencyView3), null),
        ("DialogView", typeof(MVVM_09_DialogBoxes.Views.DialogView), null),
        ("DialogWindow2", typeof(MVVM_09a_CTDialogBoxes.Views.DialogView), null),
        ("UserControlView", typeof(MVVM_16_UserControl1.Views.UserControlView), null),
        ("UserControlView2", typeof(MVVM_16_UserControl2.Views.UserControlView), null),
        ("DataPointsView",typeof(MVVM_17_1_CSV_Laden.Views.DataPointsView),null),
        ("DateDifView",typeof(MVVM_18_MultiConverters.View.DateDifView),null),
        ("PersonView",typeof(MVVM_19_FilterLists.Views.PersonView),null),
        ("SysDialogsView",typeof(MVVM_20_Sysdialogs.Views.SysDialogsView),null),
        ("ButtonsView",typeof(MVVM_21_Buttons.Views.ButtonsView),null),
        ("WpfCapView",typeof(MVVM_22_WpfCap.Views.WpfCapView),null),
        ("WpfCapView4",typeof(MVVM_22_CTWpfCap.Views.WpfCapView),null),
        ("UserControlView",typeof(MVVM_24_UserControl.Views.UserControlView),null),
        ("UserControlView2",typeof(MVVM_24a_CTUserControl.Views.UserControlView),null),
        ("UserControlView3",typeof(MVVM_24b_UserControl.Views.UserControlView),null),
        ("UserControlView4",typeof(MVVM_24c_CTUserControl.Views.UserControlView),null),   
        ("RichTextView",typeof(MVVM_25_RichTextEdit.Views.RichTextEditView),null),   
        ("BindingGroupView",typeof(MVVM_26_BindingGroupExp.Views.BindingGroupView),null),   

        ("Events_to_Commands",typeof(MVVM_33_Events_To_Commands.Views.EventBindingView),null),
        ("Events_to_Commands2",typeof(MVVM_33a_CTEvents_To_Commands.Views.EventBindingView),null),
        ("BindingEventArgs",typeof(MVVM_34_BindingEventArgs.Views.EventBindingView),null),
        ("BindingEventArgs2",typeof(MVVM_34a_CTBindingEventArgs.Views.EventBindingView),null),
        ("CommunityToolkit",typeof(MVVM_35_CommunityToolkit.Views.CommunitToolkitView),null),
        ("CommunityToolkitSavesWork",typeof(MVVM_36_ComToolKtSavesWork.Views.CommunityToolkit2View),null),
        ("TreeView",typeof(MVVM_37_TreeView.Views.BooksTreeView),null),
        ("DependencyInjection",typeof(MVVM_38_CTDependencyInjection.Views.DependencyInjectionView),null),
        ("MultiModel",typeof(MVVM_39_MultiModelTest.Views.MultiModelMainView),null),
        ("Wizzard",typeof(MVVM_40_Wizzard.Views.WizzardView),null),
        ("SuDoKu",typeof(MVVM_41_Sudoku.Views.SudokuView),null),
        ("SyncAsyncParallel",typeof(SyncAsyncParallel.Views.SyncAsyncView),null),

        
    ]; 
    #endregion

    #region Methods
    /// <summary>
    /// Initializes a new instance of the <see cref="AllExampleModel"/> class.
    /// </summary>
    /// <autogeneratedoc />
    public AllExampleModel()
    {
        _timer = new(250d);
        _timer.Elapsed += (s, e) => OnPropertyChanged(nameof(Now));
        _timer.Start();

        foreach (var ex in Examples)
            try
            {
                Debug.WriteLine($"{ex.Description} {ex.ExType}");
                var desc = new Dictionary<string, string?>();
                Type? t = ex.ExType.Assembly.GetTypes().First((t) => t.Name.EndsWith(nameof(Resources)));
                if (t != null)
                {
                    foreach (var prop in t.GetProperties())
                        if (prop.PropertyType == typeof(string))
                        {
                            Debug.WriteLine($"  {prop.Name} {prop.PropertyType} ");
                            desc[prop.Name] = (string?)prop.GetValue(null);
                        }
                    ex.Additionals = desc;
                }
            }
            catch { }   
    }

#if !NET5_0_OR_GREATER
    /// <summary>
    /// Finalizes an instance of the <see cref="MainWindowViewModel" /> class.
    /// </summary>
    ~AllExampleModel()
    {
        _timer.Stop();
        return;
    }
#endif
    #endregion
}
