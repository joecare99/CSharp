// ***********************************************************************
// Assembly         : AA06_ValueConverter2
// Author           : Mir
// Created          : 01-11-2025
//
// Last Modified By : Mir
// Last Modified On : 01-12-2025
// ***********************************************************************
// <copyright file="ValueConverterModel.cs" company="JC-Soft">
//     Copyright © JC-Soft 2025
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;

namespace AA06_ValueConverter2.Models.Interfaces;

public interface ISysTime
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
    DateTime Today { get; }
}
