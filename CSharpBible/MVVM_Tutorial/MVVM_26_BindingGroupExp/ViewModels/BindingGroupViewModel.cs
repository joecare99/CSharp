﻿// ***********************************************************************
// Assembly         : MVVM_26_BindingGroupExp
// Author           : Mir
// Created          : 10-23-2022
//
// Last Modified By : Mir
// Last Modified On : 10-23-2022
// ***********************************************************************
// <copyright file="BindingGroupViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2020
// </copyright>
// <summary></summary>
// ***********************************************************************
using MVVM.ViewModel;
using System;
using System.Windows.Controls;
using System.Windows.Data;

/// <summary>
/// The ViewModels namespace.
/// </summary>
/// <autogeneratedoc />
namespace MVVM_26_BindingGroupExp.ViewModels;

/// <summary>
/// Class BindingGroupViewModel.
/// Implements the <see cref="BaseViewModel" />
/// </summary>
/// <seealso cref="BaseViewModel" />
/// <autogeneratedoc />
public class BindingGroupViewModel : BaseViewModel
{
    #region Properties
    /// <summary>
    /// The description
    /// </summary>
    /// <autogeneratedoc />
    private string _description="Bumlux";

    /// <summary>
    /// The price
    /// </summary>
    /// <autogeneratedoc />
    private decimal _price=(decimal)0.00;

    /// <summary>
    /// The date
    /// </summary>
    /// <autogeneratedoc />
    private DateTime _date=DateTime.Now.AddDays(7);

    /// <summary>
    /// Gets or sets the data binding.
    /// </summary>
    /// <value>The data binding.</value>
    /// <autogeneratedoc />
    public BindingGroup? DataGroup { get; set; } = default;

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    /// <value>The description.</value>
    /// <autogeneratedoc />
    public string Description { get => _description; set => SetProperty(ref _description, value); }

    /// <summary>
    /// Gets or sets the price.
    /// </summary>
    /// <value>The price.</value>
    /// <autogeneratedoc />
    public decimal Price { get => _price; set => SetProperty(ref _price, value); }

    /// <summary>
    /// Gets or sets the offer expires.
    /// </summary>
    /// <value>The offer expires.</value>
    /// <autogeneratedoc />
    public DateTime OfferExpires { get => _date; set => SetProperty(ref _date, value); }

    /// <summary>
    /// Gets or sets the submit command.
    /// </summary>
    /// <value>The submit command.</value>
    /// <autogeneratedoc />
    public DelegateCommand SubmitCommand { get; set; }

    /// <summary>
    /// Gets or sets the cancel command.
    /// </summary>
    /// <value>The cancel command.</value>
    /// <autogeneratedoc />
    public DelegateCommand CancelCommand { get; set; }
    public Func<string, bool>? OnShowMessage { get; set; } = default;
    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="BindingGroupViewModel"/> class.
    /// </summary>
    /// <autogeneratedoc />
    public BindingGroupViewModel()
    {
        SubmitCommand = new DelegateCommand(SubmitData, canSubmitData);
        CancelCommand = new DelegateCommand(DoCancel, canCancel);
    }

    private bool canCancel(object? obj)
    {
        return true;// DataGroup?.IsDirty ?? false;
    }

    private void DoCancel(object? obj)
    {
        // Cancel the pending changes and begin a new edit transaction.
        DataGroup?.CancelEdit();
        DataGroup?.BeginEdit();
    }

    private bool canSubmitData(object? obj)
    {
        return true;// DataGroup?.IsDirty ?? false;
    }

    private void SubmitData(object? obj)
    {
        if (DataGroup?.CommitEdit() ?? false)
        {
            OnShowMessage?.Invoke("Data submitted ...");
            DataGroup?.BeginEdit();
        }
    }

    public void ItemError(object? sender, ValidationErrorEventArgs e)
    {
        if (e.Action == ValidationErrorEventAction.Added)
        {
            OnShowMessage?.Invoke(e.Error.ErrorContent.ToString());
        }
    }
}
