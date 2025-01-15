// ***********************************************************************
// Assembly         : Avalonia_App02
// Author           : Mir
// Created          : 01-11-2025
//
// Last Modified By : Mir
// Last Modified On : 01-12-2025
// ***********************************************************************
// <copyright file="TemplateModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.ComponentModel;

namespace Avalonia_App02.Models.Interfaces;

public interface ITemplateModel :INotifyPropertyChanged
{
    DateTime Now { get; }
}