// ***********************************************************************
// Assembly         : MVVM_40_Wizzard
// Author           : Mir
// Created          : 06-13-2024
//
// Last Modified By : Mir
// Last Modified On : 06-13-2024
// ***********************************************************************
// <copyright file="Page1ViewModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2023
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace MVVM_40_Wizzard.Models;

public class ListEntry(int value, string text)
{
    public int ID { get; } = value;
    public string Text { get; } = text;

    public override string ToString() => Text;
}
